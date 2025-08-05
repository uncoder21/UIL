namespace UIL.Diagnostics;

public readonly record struct DiagnosticInfo(
    DiagnosticCategory Category,
    DiagnosticCode Code,
    DiagnosticSeverity Severity,
    string Message)
{
    public override string ToString() => $"[{Category}:{Code}] {Severity}: {Message}";
}
