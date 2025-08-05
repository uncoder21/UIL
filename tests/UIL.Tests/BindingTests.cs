using System.Linq;
using UIL.Binding;
using UIL.Diagnostics;
using UIL.Syntax;
using Xunit;

namespace UIL.Tests;

public class BindingTests
{
    [Fact]
    public void BindMethod_UndefinedName_ReportsDiagnostic()
    {
        var source = "int M() { return x; }";
        var tree = SyntaxTree.Parse(source);
        var binder = new Binder();
        var method = (MethodDeclarationSyntax)tree.Root.Members.Single();
        binder.BindMethod(method, out _);
        Assert.Contains(binder.Diagnostics, d => d.Info.Code == DiagnosticCode.UndefinedName);
    }

    [Fact]
    public void BindMethod_DeeplyNestedBinaryExpression_BindsWithoutOverflow()
    {
        const int depth = 10000;
        var expr = string.Join("+", Enumerable.Range(0, depth));
        var source = $"int M() {{ return {expr}; }}";
        var tree = SyntaxTree.Parse(source);
        var binder = new Binder();
        var method = (MethodDeclarationSyntax)tree.Root.Members.Single();
        var body = binder.BindMethod(method, out _);
        var ret = Assert.IsType<BoundReturnStatement>(body.Statements.Single());
        var literal = Assert.IsType<BoundLiteralExpression>(ret.Expression);
        Assert.Equal(depth * (depth - 1) / 2, (int)literal.Value);
    }

    [Fact]
    public void BindMethod_ConstantFolding_SupportsMultipleOperators()
    {
        var source = "int M() { return 10 - 2 * 3 + 4 / 2; }";
        var tree = SyntaxTree.Parse(source);
        var binder = new Binder();
        var method = (MethodDeclarationSyntax)tree.Root.Members.Single();
        var body = binder.BindMethod(method, out _);
        var ret = Assert.IsType<BoundReturnStatement>(body.Statements.Single());
        var literal = Assert.IsType<BoundLiteralExpression>(ret.Expression);
        Assert.Equal(6, (int)literal.Value);
    }

    [Fact]
    public void BindMethod_DivisionByZeroConstant_ReportsDiagnostic()
    {
        var source = "int M() { return 10 / 0; }";
        var tree = SyntaxTree.Parse(source);
        var binder = new Binder();
        var method = (MethodDeclarationSyntax)tree.Root.Members.Single();
        binder.BindMethod(method, out _);
        Assert.Contains(binder.Diagnostics, d => d.Info.Code == DiagnosticCode.DivisionByZero);
    }
}
