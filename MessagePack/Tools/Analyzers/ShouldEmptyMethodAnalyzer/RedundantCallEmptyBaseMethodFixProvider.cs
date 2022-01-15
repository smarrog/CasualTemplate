using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Analyzers.ShouldEmptyMethodAnalyzer;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RedundantCallEmptyBaseMethodFixProvider)), Shared]
public class RedundantCallEmptyBaseMethodFixProvider : CodeFixProvider {
    private const string Title = "Remove redundant call to base method";

    public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("CMN002");

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context) {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null) {
            return;
        }

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var invocationNode = root.FindNode(diagnosticSpan) as InvocationExpressionSyntax;
        if (invocationNode == null) {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => RemoveInvocationAsync(context.Document, invocationNode, c),
                equivalenceKey: Title),
            diagnostic);
    }

    private static async Task<Document> RemoveInvocationAsync(Document document, InvocationExpressionSyntax invocationNode, CancellationToken cancellationToken) {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null) {
            return document;
        }

        var expressionStatement = invocationNode.Parent as ExpressionStatementSyntax;
        return RemoveNode(expressionStatement != null ? expressionStatement : invocationNode);
        
        Document RemoveNode(SyntaxNode node) {
            var newRoot = root.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);
            newRoot = Formatter.Format(newRoot, Formatter.Annotation, document.Project.Solution.Workspace);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
