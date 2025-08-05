using System.Collections;
using UIL.Diagnostics;

namespace UIL.Diagnostics;

public sealed class DiagnosticBag : IEnumerable<Diagnostic>
{
    private readonly List<Diagnostic> _diagnostics = new();

    public void Report(DiagnosticInfo info, TextLocation? location = null)
        => _diagnostics.Add(new Diagnostic(info, location));

    public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
