using System.Collections.Generic;
using UIL.Diagnostics;
using UIL.Symbols;
using UIL.Syntax;

namespace UIL.Binding;

public interface IBinder
{
    DiagnosticBag Diagnostics { get; }
    void BindCompilationUnit(CompilationUnitSyntax syntax);
    IReadOnlyDictionary<string, TypeSymbol> Types { get; }
    BoundBlockStatement BindMethod(MethodDeclarationSyntax syntax, out MethodSymbol symbol);
}
