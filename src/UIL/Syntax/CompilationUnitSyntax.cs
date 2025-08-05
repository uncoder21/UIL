using UIL.Diagnostics;

namespace UIL.Syntax;

public sealed class CompilationUnitSyntax : SyntaxNode
{
    public MethodDeclarationSyntax Member { get; }
    public SyntaxToken EndOfFileToken { get; }

    public CompilationUnitSyntax(MethodDeclarationSyntax member, SyntaxToken eof)
        : base(new TextSpan(member.Span.Start, eof.Span.End - member.Span.Start))
    {
        Member = member;
        EndOfFileToken = eof;
    }

    public override SyntaxKind Kind => SyntaxKind.CompilationUnit;
}
