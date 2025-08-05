namespace UIL.Symbols;

public class TypeSymbol : Symbol
{
    private TypeSymbol(string name) : base(name) { }

    public override SymbolKind Kind => SymbolKind.Type;

    public static readonly TypeSymbol Int = new("int");
}
