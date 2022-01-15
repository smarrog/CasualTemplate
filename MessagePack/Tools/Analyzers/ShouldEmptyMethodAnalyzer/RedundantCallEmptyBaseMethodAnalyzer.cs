using System.Collections.Immutable;
using System.Linq;
using Analyzers.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.ShouldEmptyMethodAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RedundantCallEmptyBaseMethodAnalyzer : DiagnosticAnalyzer {
    private const string Id = "CMN002";

    private static readonly LocalizableString Title = "Base method should not be called";

    private static readonly LocalizableString
        Message = "Base method '{0}' should not be called in an overridden method";

    private static readonly LocalizableString Description =
        "A base method that should remain empty should not be called in an overridden method.";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    private static readonly DiagnosticDescriptor Rule = new(
        Id,
        Title,
        Message,
        Consts.Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description
    );

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSyntaxAction, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeSyntaxAction(SyntaxNodeAnalysisContext context) {
        if (context.Node is not InvocationExpressionSyntax invocationSyntax) {
            return;
        }

        var semanticModel = context.SemanticModel;
        var methodSymbol = ModelExtensions.GetSymbolInfo(semanticModel, invocationSyntax).Symbol as IMethodSymbol;

        if (methodSymbol == null ||
            !methodSymbol.IsVirtual ||
            !methodSymbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == nameof(ShouldBeEmptyAttribute))) {
            return;
        }

        var containingMethod = invocationSyntax.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        if (containingMethod == null) {
            return;
        }

        var containingMethodSymbol =
            ModelExtensions.GetDeclaredSymbol(semanticModel, containingMethod) as IMethodSymbol;
        if (containingMethodSymbol == null || !containingMethodSymbol.IsOverride) {
            return;
        }

        if (invocationSyntax.Expression is not MemberAccessExpressionSyntax memberAccessExpression ||
            memberAccessExpression.Expression is not BaseExpressionSyntax) {
            return;
        }

        var diagnostic = Diagnostic.Create(Rule, invocationSyntax.GetLocation(), methodSymbol.Name);
        context.ReportDiagnostic(diagnostic);
    }
}