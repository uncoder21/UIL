using UIL.Binding;

namespace UIL.Optimizations;

public sealed class DeadCodeEliminationPass : IPass
{
    public string Name => "DCE";
    public BoundBlockStatement Run(BoundBlockStatement root) => root;
}
