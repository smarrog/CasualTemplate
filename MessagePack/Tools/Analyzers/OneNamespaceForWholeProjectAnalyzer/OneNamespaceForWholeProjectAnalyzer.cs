using System.Collections.Immutable;
using Analyzers.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.OneNamespaceForWholeProjectAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class OneNamespaceForWholeProjectAnalyzer : DiagnosticAnalyzer {
    private const string Id = "CMN004";

    private static readonly LocalizableString Title = "Namespace should be the same as the root namespace";

    private static readonly LocalizableString
        Message = "Namespace '{0}' should be the same as the root namespace '{1}'";

    private static readonly LocalizableString Description =
        "Namespace should be the same as the root namespace.";

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
        context.RegisterSyntaxNodeAction(AnalyzeNamespace, SyntaxKind.NamespaceDeclaration);
    }

    private void AnalyzeNamespace(SyntaxNodeAnalysisContext context) {
        var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;
        var namespaceName = namespaceDeclaration.Name.ToString();

        var expectedNamespace = GetExpectedNamespace(context);
        if (string.IsNullOrEmpty(expectedNamespace) 
            || namespaceName.StartsWith(expectedNamespace)  
            || namespaceName.Contains("Assembly-CSharp")) {
            return;
        }

        var diagnostic = Diagnostic.Create(
            Rule,
            namespaceDeclaration.Name.GetLocation(),
            namespaceName,
            expectedNamespace);

        context.ReportDiagnostic(diagnostic);
    }
    
    private static string GetExpectedNamespace(SyntaxNodeAnalysisContext context) 
        => context.Compilation.Assembly.Name;
}