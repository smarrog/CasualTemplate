using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MessagePackTools {
    public static class Utils {
        public static object GetKeyFirstParamValue(AttributeSyntax attr) {
            return (attr.ArgumentList!.Arguments.First().Expression as LiteralExpressionSyntax)!.Token.Value!;
        }

        public static object GetKeyFirstParamValue(AttributeData attr) {
            return attr.ConstructorArguments.First().Value!;
        }

        public static IOrderedEnumerable<(TDeclarationSyntax TypeDec, int Attr)> GetMemberInfoExistsKeyAttribute<TDeclarationSyntax>(TypeDeclarationSyntax typeSymbol)
            where TDeclarationSyntax : MemberDeclarationSyntax {

            try {
                return typeSymbol.Members
                    .OfType<TDeclarationSyntax>()
                    .Select(prop => (Prop: prop, Attr: prop.AttributeLists
                        .SelectMany(attrList => attrList.Attributes)
                        .FirstOrDefault(attr => attr.Name.ToString().Contains("Key"))))
                    .Where(x => x.Attr != null)
                    .Select(x => (x.Prop, Attr: (int)GetKeyFirstParamValue(x.Attr!)))
                    .OrderBy(x => x.Attr);
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public static IOrderedEnumerable<(TTypeSymbol Type, int Attr)> GetMemberInfoExistsKeyAttribute<TTypeSymbol>(ITypeSymbol typeSymbol)
            where TTypeSymbol : ISymbol {
            return typeSymbol.GetMembers()
                .OfType<TTypeSymbol>()
                .Select(prop => (Type: prop, Attr: prop.GetAttributes()
                    .FirstOrDefault(attr => attr.AttributeClass?.Name == "KeyAttribute")))
                .Where(x => x.Attr != null)
                .Select(x => (x.Type, Attr: (int)Utils.GetKeyFirstParamValue(x.Attr!)))
                .OrderBy(x => x.Attr);
        }
    }
}