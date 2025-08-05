using UIL.Binding;

namespace UIL.Optimizations;

public interface IPass
{
    string Name { get; }
    BoundBlockStatement Run(BoundBlockStatement root);
}
