using UIL.Diagnostics;

namespace UIL.Syntax;

public sealed class ClassDeclarationSyntax : MemberDeclarationSyntax
{
    public SyntaxToken ClassKeyword { get; }
    public SyntaxToken Identifier { get; }
    public SyntaxToken? LessThanToken { get; }
    public IReadOnlyList<TypeParameterSyntax> TypeParameters { get; }
    public SyntaxToken? GreaterThanToken { get; }
    public SyntaxToken OpenBraceToken { get; }
    public IReadOnlyList<MemberDeclarationSyntax> Members { get; }
    public SyntaxToken CloseBraceToken { get; }

    public ClassDeclarationSyntax(
        IReadOnlyList<AnnotationSyntax> annotations,
        SyntaxToken classKeyword,
        SyntaxToken identifier,
        SyntaxToken? lessThanToken,
        IReadOnlyList<TypeParameterSyntax> typeParameters,
        SyntaxToken? greaterThanToken,
        SyntaxToken openBraceToken,
        IReadOnlyList<MemberDeclarationSyntax> members,
        SyntaxToken closeBraceToken)
        : base(new TextSpan(classKeyword.Span.Start, closeBraceToken.Span.End - classKeyword.Span.Start), annotations)
    {
        ClassKeyword = classKeyword;
        Identifier = identifier;
        LessThanToken = lessThanToken;
        TypeParameters = typeParameters;
        GreaterThanToken = greaterThanToken;
        OpenBraceToken = openBraceToken;
        Members = members;
        CloseBraceToken = closeBraceToken;
    }

    public override SyntaxKind Kind => SyntaxKind.ClassDeclaration;
}

