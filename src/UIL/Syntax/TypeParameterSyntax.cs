using UIL.Diagnostics;

namespace UIL.Syntax;

public sealed class TypeParameterSyntax : SyntaxNode
{
    public SyntaxToken Identifier { get; }

    public TypeParameterSyntax(SyntaxToken identifier)
        : base(identifier.Span)
    {
        Identifier = identifier;
    }

    public override SyntaxKind Kind => SyntaxKind.TypeParameter;
}

