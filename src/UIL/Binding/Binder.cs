using System.Linq;
using UIL.Diagnostics;
using UIL.Symbols;
using UIL.Syntax;
using UIL.Instrumentation;

namespace UIL.Binding;

public sealed class Binder : IBinder
{
    private readonly Dictionary<string, Symbol> _scope = new();
    private readonly Dictionary<string, TypeSymbol> _types = new();
    private readonly DiagnosticBag _diagnostics = new();
    private readonly IInstrumentation? _instrumentation;

    public Binder(IInstrumentation? instrumentation = null)
    {
        _instrumentation = instrumentation;
    }

    public DiagnosticBag Diagnostics => _diagnostics;
    public IReadOnlyDictionary<string, TypeSymbol> Types => _types;

    public void BindCompilationUnit(CompilationUnitSyntax syntax)
    {
        foreach (var member in syntax.Members)
            BindMember(member, null);
    }

    private void BindMember(MemberDeclarationSyntax member, string? currentNamespace)
    {
        switch (member)
        {
            case NamespaceDeclarationSyntax ns:
                var nsName = currentNamespace is null ? ns.Identifier.Text : $"{currentNamespace}.{ns.Identifier.Text}";
                foreach (var m in ns.Members)
                    BindMember(m, nsName);
                break;
            case ClassDeclarationSyntax c:
                var className = currentNamespace is null ? c.Identifier.Text : $"{currentNamespace}.{c.Identifier.Text}";
                var typeParams = c.TypeParameters.Select(tp => tp.Identifier.Text).ToList();
                _types[className] = new TypeSymbol(className, typeParams);
                foreach (var m in c.Members)
                    BindMember(m, className);
                break;
            case InterfaceDeclarationSyntax i:
                var interfaceName = currentNamespace is null ? i.Identifier.Text : $"{currentNamespace}.{i.Identifier.Text}";
                var ifaceParams = i.TypeParameters.Select(tp => tp.Identifier.Text).ToList();
                _types[interfaceName] = new TypeSymbol(interfaceName, ifaceParams);
                foreach (var m in i.Members)
                    BindMember(m, interfaceName);
                break;
            case EnumDeclarationSyntax e:
                var enumName = currentNamespace is null ? e.Identifier.Text : $"{currentNamespace}.{e.Identifier.Text}";
                _types[enumName] = new TypeSymbol(enumName);
                break;
            case MethodDeclarationSyntax:
                break;
        }
    }

    public BoundBlockStatement BindMethod(MethodDeclarationSyntax syntax, out MethodSymbol symbol)
    {
        var parameters = new List<ParameterSymbol>();
        var index = 0;
        foreach (var p in syntax.Parameters)
        {
            var ps = new ParameterSymbol(p.Identifier.Text, TypeSymbol.Int, index++);
            _scope[ps.Name] = ps;
            parameters.Add(ps);
        }
        symbol = new MethodSymbol(syntax.Identifier.Text, TypeSymbol.Int, parameters);
        var body = BindBlock(syntax.Body);
        return body;
    }

    private BoundBlockStatement BindBlock(BlockStatementSyntax syntax)
    {
        var statements = new List<BoundStatement>();
        foreach (var statement in syntax.Statements)
            statements.Add(BindStatement(statement));
        var node = new BoundBlockStatement(statements);
        _instrumentation?.OnNodeBound(node);
        return node;
    }

    private BoundStatement BindStatement(StatementSyntax syntax)
        => syntax switch
        {
            ReturnStatementSyntax r => BindReturn(r),
            _ => throw new NotSupportedException()
        };

    private BoundReturnStatement BindReturn(ReturnStatementSyntax syntax)
    {
        var expr = BindExpression(syntax.Expression);
        var node = new BoundReturnStatement(expr);
        _instrumentation?.OnNodeBound(node);
        return node;
    }

    private BoundExpression BindExpression(ExpressionSyntax syntax)
        => syntax switch
        {
            LiteralExpressionSyntax l => BindLiteral(l),
            IdentifierNameSyntax i => BindIdentifier(i),
            BinaryExpressionSyntax b => BindBinary(b),
            _ => throw new NotSupportedException()
        };

    private BoundExpression BindLiteral(LiteralExpressionSyntax syntax)
    {
        var node = new BoundLiteralExpression(syntax.LiteralToken.Value ?? 0, TypeSymbol.Int);
        _instrumentation?.OnNodeBound(node);
        return node;
    }

    private BoundExpression BindIdentifier(IdentifierNameSyntax syntax)
    {
        if (!_scope.TryGetValue(syntax.Identifier.Text, out var symbol) || symbol is not ParameterSymbol p)
        {
            _diagnostics.Report(new DiagnosticInfo(DiagnosticCategory.Semantic, DiagnosticCode.UndefinedName, DiagnosticSeverity.Error, $"Undefined name '{syntax.Identifier.Text}'"), new TextLocation("", syntax.Identifier.Span));
            var bad = new BoundLiteralExpression(0, TypeSymbol.Int);
            _instrumentation?.OnNodeBound(bad);
            return bad;
        }
        var node = new BoundParameterExpression(p);
        _instrumentation?.OnNodeBound(node);
        return node;
    }

    private BoundExpression BindBinary(BinaryExpressionSyntax syntax)
    {
        var left = BindExpression(syntax.Left);
        var right = BindExpression(syntax.Right);

        if (left.ConstantValue is { } lConst && right.ConstantValue is { } rConst)
        {
            if (syntax.OperatorToken.Kind == SyntaxKind.PlusToken)
            {
                var value = (int)lConst.Value + (int)rConst.Value;
                var folded = new BoundLiteralExpression(value, TypeSymbol.Int);
                _instrumentation?.OnNodeBound(folded);
                return folded;
            }
        }
        var node = new BoundBinaryExpression(left, syntax.OperatorToken.Kind, right, TypeSymbol.Int);
        _instrumentation?.OnNodeBound(node);
        return node;
    }
}
