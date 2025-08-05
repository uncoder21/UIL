using UIL.Diagnostics;

namespace UIL.Syntax;

public sealed class EnumDeclarationSyntax : MemberDeclarationSyntax
{
    public SyntaxToken EnumKeyword { get; }
    public SyntaxToken Identifier { get; }
    public SyntaxToken OpenBraceToken { get; }
    public IReadOnlyList<SyntaxToken> Members { get; }
    public SyntaxToken CloseBraceToken { get; }

    public EnumDeclarationSyntax(
        IReadOnlyList<AnnotationSyntax> annotations,
        SyntaxToken enumKeyword,
        SyntaxToken identifier,
        SyntaxToken openBraceToken,
        IReadOnlyList<SyntaxToken> members,
        SyntaxToken closeBraceToken)
        : base(new TextSpan(enumKeyword.Span.Start, closeBraceToken.Span.End - enumKeyword.Span.Start), annotations)
    {
        EnumKeyword = enumKeyword;
        Identifier = identifier;
        OpenBraceToken = openBraceToken;
        Members = members;
        CloseBraceToken = closeBraceToken;
    }

    public override SyntaxKind Kind => SyntaxKind.EnumDeclaration;
}

