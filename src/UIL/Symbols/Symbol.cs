namespace UIL.Symbols;

public abstract class Symbol : ISymbol
{
    public string Name { get; }
    public abstract SymbolKind Kind { get; }

    protected Symbol(string name)
    {
        Name = name;
    }

    public override string ToString() => Name;
}
