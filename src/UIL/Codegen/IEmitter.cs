using UIL.Binding;
using UIL.IL;
using UIL.Symbols;

namespace UIL.Codegen;

public interface IEmitter
{
    void EmitMethod(MethodSymbol method, BoundBlockStatement body, ILBuilder builder);
}
