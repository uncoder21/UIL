using System;
using System.Linq;
using UIL.Binding;
using UIL.Codegen;
using UIL.IL;
using UIL.Optimizations;
using UIL.Syntax;

namespace UIL.Tests;

/// <summary>
/// Provides helper functionality for compiling source code through the entire
/// pipeline and returning the emitted IL as a normalized string. Tests can use
/// this to verify that high level code results in the expected instructions.
/// </summary>
internal static class TestCompiler
{
    /// <summary>
    /// Compiles the provided <paramref name="source"/> text and returns the
    /// string representation of the final emitted IL. Any diagnostics produced
    /// during parsing or binding will cause an <see cref="InvalidOperationException"/>.
    /// </summary>
    public static string EmitIL(string source)
    {
        var tree = SyntaxTree.Parse(source);
        if (tree.Diagnostics.Any())
            throw new InvalidOperationException("Parser reported diagnostics: " + string.Join(", ", tree.Diagnostics));

        var binder = new Binder();
        var body = binder.BindMethod(tree.Root.Member, out var method);
        if (binder.Diagnostics.Any())
            throw new InvalidOperationException("Binder reported diagnostics: " + string.Join(", ", binder.Diagnostics));

        BoundBlockStatement optimized = body;
        IPass[] passes =
        {
            new SsaPass(),
            new DeadCodeEliminationPass(),
            new LicmPass(),
            new SccpPass(),
            new GvnPass()
        };

        foreach (var pass in passes)
            optimized = pass.Run(optimized);

        var builder = new ILBuilder();
        var emitter = new ILEmitter();
        emitter.EmitMethod(method, optimized, builder);

        return Normalize(builder.ToString());
    }

    /// <summary>
    /// Normalizes line endings and trims extraneous whitespace so IL strings can
    /// be compared regardless of platform differences.
    /// </summary>
    public static string Normalize(string text) => text.Replace("\r\n", "\n").Trim();
}

