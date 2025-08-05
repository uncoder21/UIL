using UIL.Symbols;

namespace UIL.Binding;

public abstract class BoundExpression : BoundNode
{
    public TypeSymbol Type { get; }
    public ConstantValue? ConstantValue { get; }
    public Nullability Nullability { get; }

    protected BoundExpression(TypeSymbol type, ConstantValue? constant, Nullability nullability)
    {
        Type = type;
        ConstantValue = constant;
        Nullability = nullability;
    }
}
