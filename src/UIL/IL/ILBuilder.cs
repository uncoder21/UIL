using System.Text;

namespace UIL.IL;

public sealed class ILBuilder
{
    private readonly List<ILInstruction> _instructions = new();

    public int Emit(ILOpcode opcode, int? operand = null)
    {
        _instructions.Add(new ILInstruction(opcode, operand));
        return _instructions.Count - 1;
    }

    public IReadOnlyList<ILInstruction> Instructions => _instructions;

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < _instructions.Count; i++)
        {
            var instr = _instructions[i];
            var line = instr.Operand is null ? instr.Opcode.ToString().ToLower() : $"{instr.Opcode.ToString().ToLower()} {instr.Operand}";
            sb.AppendLine(line);
        }
        return sb.ToString();
    }
}
