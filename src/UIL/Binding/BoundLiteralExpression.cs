using UIL.Symbols;

namespace UIL.Binding;

public sealed class BoundLiteralExpression : BoundExpression
{
    public object Value { get; }

    public BoundLiteralExpression(object value, TypeSymbol type)
        : base(type, new ConstantValue(value), Nullability.NotNull)
    {
        Value = value;
    }

    public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
}
