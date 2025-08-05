using System.Linq;
using UIL.Syntax;
using UIL.Binding;
using UIL.Codegen;
using UIL.IL;
using UIL.Optimizations;
using Xunit;

public class CompilationTests
{
    [Fact]
    public void CompleteCompilation_EmitsExpectedIL()
    {
        var source = "int Add(int a, int b) { return a + b; }";
        var tree = SyntaxTree.Parse(source);
        Assert.False(tree.Diagnostics.Any(), "Parser reported diagnostics: " + string.Join(", ", tree.Diagnostics));

        var binder = new Binder();
        binder.BindCompilationUnit(tree.Root);
        var methodSyntax = Assert.IsType<MethodDeclarationSyntax>(tree.Root.Members.Single());
        var body = binder.BindMethod(methodSyntax, out var method);
        Assert.False(binder.Diagnostics.Any(), "Binder reported diagnostics: " + string.Join(", ", binder.Diagnostics));

        BoundBlockStatement optimized = body;
        IPass[] passes = { new SsaPass(), new DeadCodeEliminationPass(), new LicmPass(), new SccpPass(), new GvnPass() };
        foreach (var pass in passes)
            optimized = pass.Run(optimized);

        var builder = new ILBuilder();
        var emitter = new ILEmitter();
        emitter.EmitMethod(method, optimized, builder);

        var expected = new[]
        {
            new ILInstruction(ILOpcode.LdArg, 0),
            new ILInstruction(ILOpcode.LdArg, 1),
            new ILInstruction(ILOpcode.Add),
            new ILInstruction(ILOpcode.Ret),
        };
        Assert.Equal(expected, builder.Instructions);
    }
}
