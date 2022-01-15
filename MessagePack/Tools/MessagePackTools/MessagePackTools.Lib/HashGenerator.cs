using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MessagePackTools {
    public class HashGenerator {
        private MessagePackObjectsCatalog _catalog;

        public HashGenerator(MessagePackObjectsCatalog catalog) {
            _catalog = catalog;
        }

        public IEnumerable<int> GenerateHashCode(SortedSet<TypeInfo> mpTypes) {
            return mpTypes.Select(x => {
                var hash = GenerateHashCode(x.Type);
                _catalog.UpdateHashByType(x.Type, hash);
                return hash;
            });
        }

        private int GenerateHashCode(ITypeSymbol typeSymbol) {
            var combinedString = ProcessClassAndBaseClasses(typeSymbol);

            var bytes = Encoding.UTF8.GetBytes(combinedString);
            var hash = CombineHashCodes(bytes);

            return hash;
        }

        private string ProcessClassAndBaseClasses(ITypeSymbol typeSymbol) {
            var combinedStringBuilder = new StringBuilder();
            combinedStringBuilder.Append(typeSymbol.ToDisplayString());

            ProcessClassMembers(typeSymbol, combinedStringBuilder);

            var classSymbol = typeSymbol;
            while (classSymbol?.BaseType != null
                   && classSymbol.BaseType.TypeKind == TypeKind.Class
                   && classSymbol.BaseType.GetAttributes().Any(x => x.AttributeClass?.Name == "MessagePackObjectAttribute")
                  ) {

                ProcessClassMembers(classSymbol.BaseType, combinedStringBuilder);
                
                classSymbol = classSymbol.BaseType;
            }

            return combinedStringBuilder.ToString();
        }
        
        private void ProcessClassMembers(ITypeSymbol typeSymbol,
            StringBuilder combinedStringBuilder) {
            var typeDeclarationSyntax =
                (typeSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as TypeDeclarationSyntax)!;

            var keyProperties =
                Utils.GetMemberInfoExistsKeyAttribute<PropertyDeclarationSyntax>(typeDeclarationSyntax)
                    .Select(x => {
#if DEBUG
                    Console.WriteLine($"[Prop] {typeDeclarationSyntax.Identifier.Text}: " +
                                      $"{x.TypeDec.Type.ToString()} " +
                                      $"{x.TypeDec.Identifier.Text} " +
                                      $"{x.Attr.ToString()}");
#endif
                        return string.Join("", new List<string>(3) {
                            x.TypeDec.Type.ToString(),
                            x.TypeDec.Identifier.Text,
                            x.Attr.ToString()
                        });
                    });

            var keyFields =
                Utils.GetMemberInfoExistsKeyAttribute<FieldDeclarationSyntax>(typeDeclarationSyntax)
                    .Select(x => {
#if DEBUG
                        Console.WriteLine($"[Field] {typeDeclarationSyntax.Identifier.Text}: " +
                                          $"{x.TypeDec.Declaration.Type.ToString()} " +
                                          $"{x.TypeDec.Declaration.Variables.First().Identifier.Text} " +
                                          $"{x.Attr.ToString()}");
#endif
                        return string.Join("", new List<string>(3) {
                            x.TypeDec.Declaration.Type.ToString(),
                            x.TypeDec.Declaration.Variables.First().Identifier.Text,
                            x.Attr.ToString()
                        });
                    });

            combinedStringBuilder.Append(string.Join("", keyProperties.Concat(keyFields)));
        }

        public static int CombineHashCodes(IEnumerable<int> hashes) {
            unchecked {
                var result = 17;
                var counter = 0;
                foreach (var hash in hashes) {
                    result = result * 23 + hash;
                    counter++;
                }

                return counter > 0 ? result : 0;
            }
        }

        public static int CombineHashCodes(IReadOnlyCollection<byte> hashes) {
            if (hashes.Count <= 0) {
                return 0;
            }

            unchecked {
                var result = 17;
                foreach (var hash in hashes) {
                    result = result * 23 + hash;
                }

                return result;
            }
        }
    }
}