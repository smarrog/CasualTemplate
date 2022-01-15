using System.Collections.Immutable;
using System.Linq;
using Analyzers.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.ShouldEmptyMethodAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class HaveToCallBaseMethodAnalyzer : DiagnosticAnalyzer {
    private const string Id = "CMN003";

    private static readonly LocalizableString Title = "Base method should be called";

    private static readonly LocalizableString
        Message = "Base method '{0}' should be called in an overridden method";

    private static readonly LocalizableString Description =
        "A base method that should be called in an overridden method, because it have been overriden in base class.";

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
        context.RegisterSyntaxNodeAction(AnalyzeSyntaxAction, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeSyntaxAction(SyntaxNodeAnalysisContext context) {
        if (context.Node is not MethodDeclarationSyntax methodDeclaration) {
            return;
        }

        var semanticModel = context.SemanticModel;
        var containingMethodSymbol =
            ModelExtensions.GetDeclaredSymbol(semanticModel, methodDeclaration) as IMethodSymbol;

        if (containingMethodSymbol == null || !containingMethodSymbol.IsOverride) {
            return;
        }

        var baseMethodSymbol = containingMethodSymbol.OverriddenMethod;
        if (baseMethodSymbol == null) {
            return;
        }

        //Проверяем, что в иерархии есть виртуальный метод с аттрибутом ShouldBeEmptyAttribute
        var currentMethod = baseMethodSymbol;
        while (currentMethod != null && !currentMethod.IsVirtual) {
            currentMethod = currentMethod.OverriddenMethod;
        }
        
        if (currentMethod == null ||
            SymbolEqualityComparer.Default.Equals(baseMethodSymbol, currentMethod)) {
            return;
        }
        
        var hasAttribute = currentMethod.GetAttributes()
            .Any(attr => attr.AttributeClass?.Name == nameof(ShouldBeEmptyAttribute));
        
        if (!hasAttribute) {
            return;
        }

        // Проверяем, что текущий метод вызывает базовый метод
        bool isOverridenMethodCalled = false;
        foreach (var descendantNode in methodDeclaration.Body?.DescendantNodes() ?? Enumerable.Empty<SyntaxNode>()) {
            if (descendantNode is not InvocationExpressionSyntax invocationExpression ||
                invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccess ||
                memberAccess.Expression is not BaseExpressionSyntax) {
                continue;
            }

            var symbol = ModelExtensions.GetSymbolInfo(semanticModel, invocationExpression).Symbol as IMethodSymbol;
            
            if (symbol != null &&
                SymbolEqualityComparer.Default.Equals(symbol.ReducedFrom ?? symbol, baseMethodSymbol)) {
                isOverridenMethodCalled = true;
                break;
            }
        }

        if (!isOverridenMethodCalled) {
            var diagnostic = Diagnostic.Create(Rule, methodDeclaration.Identifier.GetLocation(), baseMethodSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}