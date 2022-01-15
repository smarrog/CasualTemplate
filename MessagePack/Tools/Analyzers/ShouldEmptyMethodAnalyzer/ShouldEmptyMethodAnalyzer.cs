using System.Collections.Immutable;
using System.Linq;
using Analyzers.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ShouldEmptyMethodAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ShouldEmptyMethodAnalyzer : DiagnosticAnalyzer {
    private const string Id = "CMN001";
    
    private static readonly LocalizableString Title = "Method should be empty";
    private static readonly LocalizableString Message = "Method '{0}' should have an empty body";
    private static readonly LocalizableString Description = "The method in an abstract class should remain empty.";
    
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    private static readonly DiagnosticDescriptor Rule = new(
        Id,
        Title,
        Message,
        Consts.Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: Description
    );
    
    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context) {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        var methodSymbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, methodDeclaration) as IMethodSymbol;

        if (methodSymbol == null) {
            return;
        }

        var containingType = methodSymbol.ContainingType;
        if (containingType == null) {
            return;
        }
        
        if (!methodSymbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == nameof(ShouldBeEmptyAttribute))) {
            return;
        }

        if (methodSymbol.DeclaredAccessibility <= Accessibility.Private
            || !methodSymbol.IsVirtual) {
            return;
        }

        var body = methodDeclaration.Body;
        if (body != null && body.Statements.Count <= 0) {
            return;
        }

        var diagnostic = Diagnostic.Create(Rule, methodDeclaration.GetLocation(), methodSymbol.Name);
        context.ReportDiagnostic(diagnostic);
    }
}