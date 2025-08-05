using UIL.Binding;

namespace UIL.Optimizations;

public sealed class GvnPass : IPass
{
    public string Name => "GVN";
    public BoundBlockStatement Run(BoundBlockStatement root) => root;
}
