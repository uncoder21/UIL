using UIL.Symbols;

namespace UIL.Binding;

public sealed class BoundParameterExpression : BoundExpression
{
    public ParameterSymbol Parameter { get; }

    public BoundParameterExpression(ParameterSymbol parameter)
        : base(parameter.Type, null, Nullability.Unknown)
    {
        Parameter = parameter;
    }

    public override BoundNodeKind Kind => BoundNodeKind.ParameterExpression;
}
