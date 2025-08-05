using UIL.Diagnostics;
using UIL.Symbols;
using UIL.Syntax;

namespace UIL.Binding;

public interface IBinder
{
    DiagnosticBag Diagnostics { get; }
    void BindCompilationUnit(CompilationUnitSyntax syntax);
    BoundBlockStatement BindMethod(MethodDeclarationSyntax syntax, out MethodSymbol symbol);
}
