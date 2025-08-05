using UIL.Binding;

namespace UIL.Optimizations;

public sealed class SsaPass : IPass
{
    public string Name => "SSA";
    public BoundBlockStatement Run(BoundBlockStatement root) => root;
}
