using UIL.Diagnostics;

namespace UIL.Syntax;

public abstract class MemberDeclarationSyntax : SyntaxNode
{
    public IReadOnlyList<AnnotationSyntax> Annotations { get; }

    protected MemberDeclarationSyntax(TextSpan span, IReadOnlyList<AnnotationSyntax> annotations)
        : base(span)
    {
        Annotations = annotations;
    }
}

