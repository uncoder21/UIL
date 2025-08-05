using UIL.Diagnostics;

namespace UIL.Syntax;

public sealed class CompilationUnitSyntax : SyntaxNode
{
    public IReadOnlyList<MemberDeclarationSyntax> Members { get; }
public MethodDeclarationSyntax Member => (MethodDeclarationSyntax)Members[0];
    public SyntaxToken EndOfFileToken { get; }

    public CompilationUnitSyntax(IReadOnlyList<MemberDeclarationSyntax> members, SyntaxToken eof)
        : base(new TextSpan(members.Count > 0 ? members[0].Span.Start : eof.Span.Start, eof.Span.End - (members.Count > 0 ? members[0].Span.Start : eof.Span.Start)))
    {
        Members = members;
        EndOfFileToken = eof;
    }

    public override SyntaxKind Kind => SyntaxKind.CompilationUnit;
}
