namespace UIL.Diagnostics;

public sealed record Diagnostic(DiagnosticInfo Info, TextLocation? Location = null)
{
    public override string ToString() => Location is null ? Info.ToString() : $"{Info} at {Location.Value.FilePath}[{Location.Value.Span.Start}-{Location.Value.Span.End}]";
}
