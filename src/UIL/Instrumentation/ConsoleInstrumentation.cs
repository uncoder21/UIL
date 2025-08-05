using UIL.Binding;
using UIL.IL;

namespace UIL.Instrumentation;

public sealed class ConsoleInstrumentation : IInstrumentation
{
    public void OnNodeBound(BoundNode node)
        => Console.WriteLine($"[bind] {node.Kind}");

    public void OnInstructionEmitted(ILInstruction instruction)
        => Console.WriteLine($"[emit] {instruction.Opcode}{(instruction.Operand is null ? string.Empty : " " + instruction.Operand)}");
}
