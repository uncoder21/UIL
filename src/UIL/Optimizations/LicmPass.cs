using UIL.Binding;

namespace UIL.Optimizations;

public sealed class LicmPass : IPass
{
    public string Name => "LICM";
    public BoundBlockStatement Run(BoundBlockStatement root) => root;
}
