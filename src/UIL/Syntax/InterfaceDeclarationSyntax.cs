using UIL.Diagnostics;

namespace UIL.Syntax;

public sealed class InterfaceDeclarationSyntax : MemberDeclarationSyntax
{
    public SyntaxToken InterfaceKeyword { get; }
    public SyntaxToken Identifier { get; }
    public SyntaxToken? LessThanToken { get; }
    public IReadOnlyList<TypeParameterSyntax> TypeParameters { get; }
    public SyntaxToken? GreaterThanToken { get; }
    public SyntaxToken OpenBraceToken { get; }
    public IReadOnlyList<MemberDeclarationSyntax> Members { get; }
    public SyntaxToken CloseBraceToken { get; }

    public InterfaceDeclarationSyntax(
        IReadOnlyList<AnnotationSyntax> annotations,
        SyntaxToken interfaceKeyword,
        SyntaxToken identifier,
        SyntaxToken? lessThanToken,
        IReadOnlyList<TypeParameterSyntax> typeParameters,
        SyntaxToken? greaterThanToken,
        SyntaxToken openBraceToken,
        IReadOnlyList<MemberDeclarationSyntax> members,
        SyntaxToken closeBraceToken)
        : base(new TextSpan(interfaceKeyword.Span.Start, closeBraceToken.Span.End - interfaceKeyword.Span.Start), annotations)
    {
        InterfaceKeyword = interfaceKeyword;
        Identifier = identifier;
        LessThanToken = lessThanToken;
        TypeParameters = typeParameters;
        GreaterThanToken = greaterThanToken;
        OpenBraceToken = openBraceToken;
        Members = members;
        CloseBraceToken = closeBraceToken;
    }

    public override SyntaxKind Kind => SyntaxKind.InterfaceDeclaration;
}

