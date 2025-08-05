using UIL.Binding;
using UIL.IL;

namespace UIL.Instrumentation;

public interface IInstrumentation
{
    void OnNodeBound(BoundNode node);
    void OnInstructionEmitted(ILInstruction instruction);
}
