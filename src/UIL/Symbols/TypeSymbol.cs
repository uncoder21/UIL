using System.Collections.Generic;

namespace UIL.Symbols;

public class TypeSymbol : Symbol
{
    public IReadOnlyList<string> TypeParameters { get; }

    public TypeSymbol(string name, IReadOnlyList<string>? typeParameters = null)
        : base(name)
    {
        TypeParameters = typeParameters ?? Array.Empty<string>();
    }

    public override SymbolKind Kind => SymbolKind.Type;

    public static readonly TypeSymbol Int = new("int");
}
