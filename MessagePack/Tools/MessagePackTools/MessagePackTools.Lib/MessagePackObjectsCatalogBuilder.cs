using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MessagePackTools {
    public class MessagePackObjectsCatalogBuilder {

        [Serializable]
        internal class AssemblyDefinitionAssetInfo {
            [Newtonsoft.Json.JsonProperty("references")]
            public string[]? References { get; set; }
        }

        private static readonly Regex _messagePackObjectAttributePattern = new(@"\[.*?\bMessagePackObject\b.*?\]", RegexOptions.Compiled);
        private static IReadOnlyCollection<string> EmptyCollection = new SortedSet<string>(); 
        private static CSharpParseOptions _parseOption;

        public static MessagePackObjectsCatalog Build(string[] rootDirs, bool isDebug = false) {
            var cw = Stopwatch.StartNew();

            //TODO add preprocessor symbols
            _parseOption = new CSharpParseOptions(LanguageVersion.Latest, DocumentationMode.Parse,
                SourceCodeKind.Regular /*, CleanPreprocessorSymbols(preprocessorSymbols)*/);

            var syntaxTrees = new List<SyntaxTree>();
            var assemblyPathToAsmDefInfo = new Dictionary<string, AsmDefInfo>();

            Parallel.ForEach(rootDirs, rootDir => {
                var normalizedPath = rootDir.Replace('\\', '/');
                FindAssemblies(normalizedPath, normalizedPath, assemblyPathToAsmDefInfo, syntaxTrees);
            });

            var compilationTime = Stopwatch.StartNew();
            var compilation = PseudoCompilation.CreateFromSyntaxTrees(syntaxTrees, _parseOption);
            if (isDebug) {
                Console.WriteLine($"\tCompilation built in {compilationTime.ElapsedMilliseconds}ms");
            }
            
            var filePathToFileInfo = assemblyPathToAsmDefInfo.Values
                .SelectMany(x => x.FileInfos)
                .ToDictionary(x => x.SyntaxTree.FilePath, x => x);

            FillTransitiveDependencies(assemblyPathToAsmDefInfo, isDebug);
            FillTypes(assemblyPathToAsmDefInfo, filePathToFileInfo, compilation, isDebug);
            
            var catalog = new MessagePackObjectsCatalog(assemblyPathToAsmDefInfo, filePathToFileInfo, isDebug);
            
            if (isDebug) {
                Console.WriteLine($"Catalog built in {cw.ElapsedMilliseconds}ms");
            }

            return catalog;
        }

        private static void FindAssemblies(string currentAssembly, string currentDir,
            Dictionary<string, AsmDefInfo> catalog, List<SyntaxTree> syntaxTrees) {
            var assemblies = Directory.GetFiles(currentDir, "*.asmdef");
            if (assemblies.Length > 1) {
                throw new InvalidOperationException(
                    $"Multiple .asmdef files in the same directory {currentDir} are not supported");
            }

            var assemblyPath = assemblies.FirstOrDefault();
            assemblyPath = assemblyPath == null ? currentAssembly : assemblyPath.Replace('\\', '/');

            Parallel.ForEach(Directory.EnumerateFiles(currentDir, "*.cs", SearchOption.TopDirectoryOnly)
                , file => {
                    if (Path.GetFileName(file).EndsWith("_Resolver.cs")) {
                        return;
                    }
                    
                    var content = File.ReadAllText(file, Encoding.UTF8);
                    var syntax = CSharpSyntaxTree.ParseText(content, _parseOption,file);
                    lock (syntaxTrees) {
                        syntaxTrees.Add(syntax);
                    }

                    if (!_messagePackObjectAttributePattern.IsMatch(content)) {
                        return;
                    }

                    var info = new FileInfo {
                        SyntaxTree = syntax,
                        AsmDefPath = assemblyPath
                    };
                    AddMpObject(catalog, assemblyPath, info);
                });

            Parallel.ForEach(Directory.EnumerateDirectories(currentDir, "*", SearchOption.TopDirectoryOnly)
                , subDir => FindAssemblies(assemblyPath, subDir, catalog, syntaxTrees));
        }

        private static void AddMpObject(Dictionary<string, AsmDefInfo> catalog, string asmdefPath,
            FileInfo fileInfo) {
            lock (catalog) {
                if (!catalog.TryGetValue(asmdefPath, out var data)) {
                    data = new AsmDefInfo {
                        Path = asmdefPath,
                    };
                    catalog[asmdefPath] = data;
                }

                data.FileInfos.Add(fileInfo);
            }
        }
        
        private static void WarmupSemanticModels(Dictionary<string, FileInfo>.ValueCollection fileInfos,
            CSharpCompilation compilation) {
            foreach (var fileInfo in fileInfos) {
                compilation.GetSemanticModel(fileInfo.SyntaxTree);
            }
        }
        
        private static void FillTransitiveDependencies(Dictionary<string, AsmDefInfo> asmDefPathToAsmDefInfo, bool isDebug) {
            var cw = Stopwatch.StartNew();
            
            var asmDefNameToPath = asmDefPathToAsmDefInfo.Keys.ToDictionary(Path.GetFileNameWithoutExtension, x => x);

            var asmDefPathToRefs = new Dictionary<string, List<string>>();
            foreach (var path in asmDefPathToAsmDefInfo.Keys) {
                var rawAsmDef = File.ReadAllText(path);
                var asmDef = Newtonsoft.Json.JsonConvert.DeserializeObject<AssemblyDefinitionAssetInfo>(rawAsmDef);
                if (asmDef.References == null) {
                    continue;
                }

                var refs = asmDef.References.Intersect(asmDefNameToPath.Keys).ToList();
                asmDefPathToRefs.Add(path, refs);
            }

            foreach (var (path, asmDefInfo) in asmDefPathToAsmDefInfo) {
                _ = FillTransitiveDependenciesRecursive(path, asmDefInfo, asmDefPathToRefs, asmDefPathToAsmDefInfo, asmDefNameToPath);
            }

            if (isDebug) {
                Console.WriteLine($"\tDependency graph built in {cw.ElapsedMilliseconds}ms");
            }
        }

        private static IReadOnlyCollection<string> FillTransitiveDependenciesRecursive(string path, AsmDefInfo info,
            Dictionary<string, List<string>> asmDefPathToRefs, Dictionary<string, AsmDefInfo> asmDefPathToAsmDefInfo, Dictionary<string, string> asmDefNameToPath) {
            if (info.AsmDefReferenceNames.Any()) {
                return info.AsmDefReferenceNames;
            }
            
            if (!asmDefPathToRefs.TryGetValue(path, out var refs)) {
                return EmptyCollection;
            }
            
            var allRefs = refs.ToList();
            foreach (var subRefPath in refs.Select(asmDefNameToPath.GetValueOrDefault)) {
                var subInfo = asmDefPathToAsmDefInfo[subRefPath];
                var subRefs = FillTransitiveDependenciesRecursive(subRefPath, subInfo, asmDefPathToRefs, asmDefPathToAsmDefInfo, asmDefNameToPath);
                allRefs.AddRange(subRefs);
            }

            info.AsmDefReferenceNames.UnionWith(allRefs);
            return info.AsmDefReferenceNames;
        }

        private static void FillTypes(Dictionary<string, AsmDefInfo> asmDefPathToAsmDefInfo,
            Dictionary<string, FileInfo> filePathToFileInfo, CSharpCompilation compilation, bool isDebug) {
            var cw = Stopwatch.StartNew();
            
            WarmupSemanticModels(filePathToFileInfo.Values, compilation);

            var typeExtractor = new TypeExtractor(filePathToFileInfo);
            asmDefPathToAsmDefInfo
                .SelectMany(x => x.Value.FileInfos)
                .AsParallel()
                .ForAll(fileInfo => {
                    var asmDefInfo = asmDefPathToAsmDefInfo[fileInfo.AsmDefPath];
                    var semanticModel = compilation.GetSemanticModel(fileInfo.SyntaxTree);
                    typeExtractor.FillTypes(asmDefInfo, fileInfo, semanticModel);
                });

            if (isDebug) {
                Console.WriteLine($"\tFillTypes done in {cw.ElapsedMilliseconds}ms");
            }
        }
    }
}