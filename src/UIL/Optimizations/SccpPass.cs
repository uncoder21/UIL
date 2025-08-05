using UIL.Binding;

namespace UIL.Optimizations;

public sealed class SccpPass : IPass
{
    public string Name => "SCCP";
    public BoundBlockStatement Run(BoundBlockStatement root) => root;
}
