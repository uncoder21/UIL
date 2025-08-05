namespace UIL.Symbols;

public class ParameterSymbol : Symbol
{
    public TypeSymbol Type { get; }
    public int Index { get; }

    public ParameterSymbol(string name, TypeSymbol type, int index) : base(name)
    {
        Type = type;
        Index = index;
    }

    public override SymbolKind Kind => SymbolKind.Parameter;
}
