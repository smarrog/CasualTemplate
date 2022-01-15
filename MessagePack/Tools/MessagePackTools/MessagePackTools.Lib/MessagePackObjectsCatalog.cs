using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace MessagePackTools {
    public class MessagePackObjectsCatalog {
        private const string _sharedCsproj = "Shared.csproj";
        private const string _messagePackObjectattributeFile = "MessagePackObjectAttribute.cs";
        
        private readonly Dictionary<string, AsmDefInfo> _asmDefPathToAsmDefInfo;

        private readonly bool _isDebug = false;

        private readonly IReadOnlyDictionary<string, FileInfo> _filePathToFileInfo;
        
        private Dictionary<string, int>? _asmDefPathToHash = null;
        public Dictionary<string, int> AsmdefPathToHash => _asmDefPathToHash ??= CrateHashCodes();

        public MessagePackObjectsCatalog(Dictionary<string, AsmDefInfo> asmDefPathToAsmDefInfo,
            Dictionary<string, FileInfo> filePathToFileInfo, bool isDebug) {
            _isDebug = isDebug;
            _asmDefPathToAsmDefInfo = asmDefPathToAsmDefInfo;
            _filePathToFileInfo = filePathToFileInfo;
        }
        
        private Dictionary<string, int> CrateHashCodes() {
            var cw = Stopwatch.StartNew();

            var asmDefPathToHash = CreateOwnHashCodes();
            ModifyByHashesFromTypeDependencies(asmDefPathToHash);

            if (_isDebug) {
                Console.WriteLine($"Hashes created in {cw.ElapsedMilliseconds}ms");
            }

            return asmDefPathToHash;
        }

        private Dictionary<string, int> CreateOwnHashCodes() {
            var dict = new Dictionary<string, int>();
            var hashGenerator = new HashGenerator(this);

            Parallel.ForEach(_asmDefPathToAsmDefInfo, (pair) => {
                var hashes = hashGenerator.GenerateHashCode(pair.Value.Types);
                var combineHashCodes = HashGenerator.CombineHashCodes(hashes);

                lock (dict) {
                    dict.Add(pair.Key, combineHashCodes);
                }
            });

            return dict;
        }

        private void ModifyByHashesFromTypeDependencies(Dictionary<string, int> asmDefPathToHash) {
            foreach (var (asmdefPath, asmDefInfo) in _asmDefPathToAsmDefInfo) {
                if (asmDefInfo.RefTypes.Count <= 0) {
                    continue;
                }

                var refHashes = asmDefInfo.RefTypes.Select(refType => {
                    var refAsmDefInfo = _asmDefPathToAsmDefInfo[refType.AsmDefPath];
                    return refAsmDefInfo.Types.FirstOrDefault(x => x.Type.Name == refType.Type.Name)?.Hash ?? -1;
                });

                var hash = asmDefPathToHash[asmdefPath];
                asmDefPathToHash[asmdefPath] = HashGenerator.CombineHashCodes(refHashes.Append(hash));
            }
        }

        public void SaveReportToFile(string catalogFile) {
            var builder = new StringBuilder();
            foreach (var (asmdefPath, files) in _asmDefPathToAsmDefInfo) {
                builder.AppendLine($"{asmdefPath} - {AsmdefPathToHash[asmdefPath]}");
                builder.Append("\tReferences:");
                if (files.AsmDefReferenceNames.Count > 0) {
                    builder.Append(Environment.NewLine);
                    foreach (var filesReference in files.AsmDefReferenceNames) {
                        builder.AppendLine($"\t\t{filesReference}");
                    }
                }
                else {
                    builder.Append($" none{Environment.NewLine}");
                }

                builder.AppendLine("\tMP Types:");
                foreach (var typeInfo in files.Types) {
                    builder.AppendLine($"\t\t{typeInfo.Type.ToDisplayString()} : {typeInfo.Hash}");
                }

                builder.AppendLine("\tMP Ref Types:");
                foreach (var pair in files.RefTypes) {
                    builder.AppendLine($"\t\t{pair}");
                }
            }

            Console.WriteLine("write to " + catalogFile);
            File.WriteAllText(catalogFile, builder.ToString());
        }

        public void UpdateHashByType(ITypeSymbol typeSymbol, int hash) {
            var sourceTreeFilePath = typeSymbol.Locations.First().SourceTree.FilePath;
            if (!_filePathToFileInfo.TryGetValue(sourceTreeFilePath, out var fileInfo)) {
                return;
            }

            var asmDefPath = fileInfo.AsmDefPath;
            var asmDefInfo = _asmDefPathToAsmDefInfo[asmDefPath];
            var typeInfo = asmDefInfo.Types.FirstOrDefault(x => x.Type == typeSymbol);
            if (typeInfo == null) {
                return;
            }

            typeInfo.Hash = hash;
        }
        
        public void GenerateProjects(string projectsDir) {
            if (Directory.Exists(projectsDir)) {
                Directory.Delete(projectsDir, true);
            }
            
            CreateDirectory(projectsDir);
            
            GenerateSharedProject(projectsDir);
            
            foreach (var (asmDefPath, asmDefInfo) in _asmDefPathToAsmDefInfo) {
               GenerateProject(asmDefPath, asmDefInfo, projectsDir); 
            }
        }

        private static void CreateDirectory(string projectsDir) {
            if (!Directory.Exists(projectsDir)) {
                Directory.CreateDirectory(projectsDir);
            }
        }

        private void GenerateSharedProject(string projectDir) {
            var attributesPath = Path.Combine(projectDir, _messagePackObjectattributeFile);
            
            var project = new XElement("Project",
                new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup",
                    new XElement("OutputType", "Library"),
                    new XElement("TargetFramework", "netstandard2.1"),
                    new XElement("LangVersion", "9")
                )
            );
            
            var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), project);
            var outputPath = Path.Combine(projectDir,  _sharedCsproj);

            File.WriteAllText( attributesPath, PseudoCompilation.DummyAnnotation);
            document.Save(outputPath);

            if (_isDebug) {
                Console.WriteLine($"csproj file has been created: {outputPath}");
            }
        }

        private void GenerateProject(string asmDefPath, AsmDefInfo asmDefInfo, string projectsDir) {
            var asmDefDir = Path.GetDirectoryName(asmDefPath)!;
            
            var project = new XElement("Project",
                new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup",
                    new XElement("OutputType", "Library"),
                    new XElement("TargetFramework", "netstandard2.1"),
                    new XElement("LangVersion", "9")
                ),
                new XElement("ItemGroup",
                    new XElement("ProjectReference",
                        new XAttribute("Include",
                            Path.GetFullPath(Path.Combine(projectsDir, _sharedCsproj)))
                    ),
                    asmDefInfo.AsmDefReferenceNames.Select(asmDefRefName =>
                        new XElement("ProjectReference",
                            new XAttribute("Include",
                                Path.GetFullPath(Path.Combine(projectsDir, asmDefRefName + ".csproj")))
                        )
                    ),
                    new XElement("Compile", new XAttribute("Include", Path.GetFullPath(Path.Combine(asmDefDir, @"**/*.cs")))),
                    new XElement("Compile", new XAttribute("Remove", Path.GetFullPath(Path.Combine(projectsDir, _messagePackObjectattributeFile))))
                )
            );

            var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), project);
            var outputPath = Path.Combine(projectsDir, Path.GetFileNameWithoutExtension(asmDefPath) + ".csproj");

            document.Save(outputPath);

            if (_isDebug) {
                Console.WriteLine($"csproj file has been created: {outputPath}");
            }
        }
    }
}