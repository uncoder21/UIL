using System.Linq;
using UIL.Syntax;
using UIL.Binding;
using UIL.Codegen;
using UIL.IL;
using UIL.Instrumentation;
using UIL.Optimizations;

var source = "int Add(int a, int b) { return a + b; }";
var tree = SyntaxTree.Parse(source);
if (tree.Diagnostics.Any())
{
    Console.WriteLine("Parse diagnostics:");
    foreach (var d in tree.Diagnostics)
        Console.WriteLine(d);
}

var instrumentation = new ConsoleInstrumentation();
IBinder binder = new Binder(instrumentation);
binder.BindCompilationUnit(tree.Root);
var body = binder.BindMethod((MethodDeclarationSyntax)tree.Root.Members.Single(), out var method);
if (binder.Diagnostics.Any())
{
    Console.WriteLine("Bind diagnostics:");
    foreach (var d in binder.Diagnostics)
        Console.WriteLine(d);
}

BoundBlockStatement optimized = body;
IPass[] passes = { new SsaPass(), new DeadCodeEliminationPass(), new LicmPass(), new SccpPass(), new GvnPass() };
foreach (var pass in passes)
    optimized = pass.Run(optimized);

var builder = new ILBuilder();
IEmitter emitter = new ILEmitter(instrumentation);
emitter.EmitMethod(method, optimized, builder);

Console.WriteLine("Generated IL:");
Console.WriteLine(builder.ToString());
