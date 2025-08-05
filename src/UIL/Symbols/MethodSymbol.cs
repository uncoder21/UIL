namespace UIL.Symbols;

public class MethodSymbol : Symbol
{
    public TypeSymbol ReturnType { get; }
    public IReadOnlyList<ParameterSymbol> Parameters { get; }

    public MethodSymbol(string name, TypeSymbol returnType, IReadOnlyList<ParameterSymbol> parameters)
        : base(name)
    {
        ReturnType = returnType;
        Parameters = parameters;
    }

    public override SymbolKind Kind => SymbolKind.Method;
}
