using UIL.Diagnostics;

namespace UIL.Syntax;

public sealed class NamespaceDeclarationSyntax : MemberDeclarationSyntax
{
    public SyntaxToken NamespaceKeyword { get; }
    public SyntaxToken Identifier { get; }
    public SyntaxToken OpenBraceToken { get; }
    public IReadOnlyList<MemberDeclarationSyntax> Members { get; }
    public SyntaxToken CloseBraceToken { get; }

    public NamespaceDeclarationSyntax(
        IReadOnlyList<AnnotationSyntax> annotations,
        SyntaxToken namespaceKeyword,
        SyntaxToken identifier,
        SyntaxToken openBraceToken,
        IReadOnlyList<MemberDeclarationSyntax> members,
        SyntaxToken closeBraceToken)
        : base(new TextSpan(namespaceKeyword.Span.Start, closeBraceToken.Span.End - namespaceKeyword.Span.Start), annotations)
    {
        NamespaceKeyword = namespaceKeyword;
        Identifier = identifier;
        OpenBraceToken = openBraceToken;
        Members = members;
        CloseBraceToken = closeBraceToken;
    }

    public override SyntaxKind Kind => SyntaxKind.NamespaceDeclaration;
}

