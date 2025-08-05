using UIL.Diagnostics;

namespace UIL.Syntax;

public sealed class SyntaxToken : SyntaxNode
{
    public override SyntaxKind Kind { get; }
    public string Text { get; }
    public object? Value { get; }

    public SyntaxToken(SyntaxKind kind, int position, string text, object? value)
        : base(new TextSpan(position, text.Length))
    {
        Kind = kind;
        Text = text;
        Value = value;
    }
}
