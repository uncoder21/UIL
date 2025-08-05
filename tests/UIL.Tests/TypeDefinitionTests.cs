using System.Linq;
using UIL.Binding;
using UIL.Syntax;
using Xunit;

namespace UIL.Tests;

public class TypeDefinitionTests
{
    [Fact]
    public void BindCompilationUnit_RegistersTypesWithGenerics()
    {
        var source = "namespace N { [Annot] class C<T> { } interface I<U> { } enum E { A, B } }";
        var tree = SyntaxTree.Parse(source);
        var binder = new Binder();
        binder.BindCompilationUnit(tree.Root);
        Assert.Contains("N.C", binder.Types.Keys);
        Assert.Contains("N.I", binder.Types.Keys);
        Assert.Contains("N.E", binder.Types.Keys);
        Assert.Single(binder.Types["N.C"].TypeParameters);
    }
}

