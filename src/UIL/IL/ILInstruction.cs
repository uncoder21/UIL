namespace UIL.IL;

public readonly record struct ILInstruction(ILOpcode Opcode, int? Operand = null);
