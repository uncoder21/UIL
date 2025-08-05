using UIL.Binding;
using UIL.IL;
using UIL.Symbols;
using UIL.Instrumentation;

namespace UIL.Codegen;

public sealed class ILEmitter : IEmitter
{
    private readonly IInstrumentation? _instrumentation;

    public ILEmitter(IInstrumentation? instrumentation = null)
    {
        _instrumentation = instrumentation;
    }

    public void EmitMethod(MethodSymbol method, BoundBlockStatement body, ILBuilder builder)
    {
        foreach (var statement in body.Statements)
            EmitStatement(statement, builder);
    }

    private void EmitStatement(BoundStatement statement, ILBuilder builder)
    {
        switch (statement)
        {
            case BoundReturnStatement ret:
                EmitExpression(ret.Expression, builder);
                EmitInstruction(builder, ILOpcode.Ret);
                break;
            default:
                throw new NotSupportedException($"Statement '{statement.Kind}' not supported");
        }
    }

    private void EmitExpression(BoundExpression expression, ILBuilder builder)
    {
        switch (expression)
        {
            case BoundLiteralExpression l:
                EmitInstruction(builder, ILOpcode.LdcI4, (int)l.Value);
                break;
            case BoundParameterExpression p:
                EmitInstruction(builder, ILOpcode.LdArg, p.Parameter.Index);
                break;
            case BoundBinaryExpression b:
                EmitExpression(b.Left, builder);
                EmitExpression(b.Right, builder);
                if (b.OperatorKind == Syntax.SyntaxKind.PlusToken)
                    EmitInstruction(builder, ILOpcode.Add);
                else
                    throw new NotSupportedException($"Operator '{b.OperatorKind}' not supported");
                break;
            default:
                throw new NotSupportedException($"Expression '{expression.Kind}' not supported");
        }
    }

    private void EmitInstruction(ILBuilder builder, ILOpcode opcode, int? operand = null)
    {
        builder.Emit(opcode, operand);
        _instrumentation?.OnInstructionEmitted(new ILInstruction(opcode, operand));
    }
}
