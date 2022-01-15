using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MessagePackTools {
    public class TypeExtractor {
        private Dictionary<string, FileInfo> _filePathToFileInfo;

        public TypeExtractor(Dictionary<string, FileInfo> filePathToFileInfo) {
            _filePathToFileInfo = filePathToFileInfo;
        }

        public void FillTypes(AsmDefInfo asmDefInfo, FileInfo fileInfo, SemanticModel semanticModel) {
            var syntaxNode = fileInfo.SyntaxTree.GetRoot();
            
            var mpTypes = syntaxNode.DescendantNodes()
                .OfType<TypeDeclarationSyntax>()
                .Where(x => x.AttributeLists
                    .SelectMany(y => y.Attributes)
                    .Any(z => z.Name.ToString() == "MessagePackObject")
                ).ToArray();

            var typeInfos = mpTypes.Select(x => new TypeInfo() {
                Type = semanticModel.GetDeclaredSymbol(x)!,
                Hash = 0 //default hash
            });

            lock (asmDefInfo.Types) {
                asmDefInfo.Types.UnionWith(typeInfos);
            }
            
            var refTypeInfos = new List<TypeReferenceInfo>(16);
            foreach (var typeDeclarationSyntax in mpTypes) {
                var referenceTypes = GetClassMembersTypes(typeDeclarationSyntax, semanticModel);
                foreach (var referenceType in referenceTypes) {
                    FillReferenceTypeInfos(refTypeInfos, asmDefInfo, referenceType);
                }
            }
            
            lock (asmDefInfo.RefTypes) {
                asmDefInfo.RefTypes.UnionWith(refTypeInfos);
            }
        }

        private IEnumerable<ITypeSymbol?> GetClassMembersTypes(TypeDeclarationSyntax typeDeclarationSyntax,
            SemanticModel semanticModel) {
            var keyProperties = Utils.GetMemberInfoExistsKeyAttribute<PropertyDeclarationSyntax>(typeDeclarationSyntax);
            foreach (var (prop, attr) in keyProperties) {
#if DEBUG
                Console.WriteLine($"[Prop] {typeDeclarationSyntax.Identifier.Text}: " +
                                  $"{prop.Type.ToString()} " +
                                  $"{prop.Identifier.Text} " +
                                  $"{attr.ToString()}");
#endif
                yield return semanticModel.GetTypeInfo(prop.Type).Type;
            }

            var keyFields = Utils.GetMemberInfoExistsKeyAttribute<FieldDeclarationSyntax>(typeDeclarationSyntax);
            foreach (var (field, attr) in keyFields) {
#if DEBUG
                Console.WriteLine($"[Field] {typeDeclarationSyntax.Identifier.Text}: " +
                                  $"{field.Declaration.Type.ToString()} " +
                                  $"{field.Declaration.Variables.First().Identifier.Text} " +
                                  $"{attr.ToString()}");
#endif
                yield return semanticModel.GetTypeInfo(field.Declaration.Type).Type;
            }
        }

        private void FillReferenceTypeInfos(List<TypeReferenceInfo> refTypes,
            AsmDefInfo asmDefInfo, ITypeSymbol? typeSymbol) {
            if (typeSymbol is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol) {
                foreach (var typeArg in namedTypeSymbol.TypeArguments) {
                    if (typeArg.TypeKind != TypeKind.Class && typeArg.TypeKind != TypeKind.Struct) {
                        continue;
                    }

                    AddReferenceType(refTypes, asmDefInfo, typeArg);
                }

                //еще пытаемся добавить дженерик тип
                AddReferenceType(refTypes, asmDefInfo, typeSymbol);
                return;
            }

            AddReferenceType(refTypes, asmDefInfo, typeSymbol);
        }
        
        private void AddReferenceType(List<TypeReferenceInfo> refTypes, AsmDefInfo originAsmDefInfo, ITypeSymbol typeDeclaration) {
            var referenceFilePath = typeDeclaration.DeclaringSyntaxReferences.FirstOrDefault()?.SyntaxTree.FilePath;
            if (string.IsNullOrEmpty(referenceFilePath)) {
                return;
            }

            var asmDefRefPath = GetAsmDefPathByFile(referenceFilePath);
            if (string.IsNullOrEmpty(asmDefRefPath)) {
                return;
            }

            if (asmDefRefPath == originAsmDefInfo.Path) {
                return;
            }

            refTypes.Add(new TypeReferenceInfo {
                Type = typeDeclaration,
                AsmDefPath = asmDefRefPath,
                FilePath = referenceFilePath
            });
        }

        private string GetAsmDefPathByFile(string filePath) {
            return _filePathToFileInfo.TryGetValue(filePath, out var fileInfo)
                ? fileInfo.AsmDefPath
                : string.Empty;
        }
    }
}