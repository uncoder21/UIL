using System.Text;
using UIL.Diagnostics;

namespace UIL.Syntax;

public sealed class Parser
{
    private readonly string _text;
    private readonly DiagnosticBag _diagnostics = new();
    private readonly List<SyntaxToken> _tokens = new();
    private int _position;

    public Parser(string text)
    {
        _text = text;
        while (true)
        {
            var token = NextToken();
            if (token.Kind != SyntaxKind.EndOfFileToken)
                _tokens.Add(token);
            else
            {
                _tokens.Add(token);
                break;
            }
        }
    }

    public DiagnosticBag Diagnostics => _diagnostics;

    private SyntaxToken Peek(int offset)
    {
        var index = _position + offset;
        if (index >= _tokens.Count)
            return _tokens[^1];
        return _tokens[index];
    }

    private SyntaxToken Current => Peek(0);

    private SyntaxToken NextToken()
    {
        if (_position >= _tokens.Count)
        {
            var token = ReadToken();
            return token;
        }
        var current = _tokens[_position];
        _position++;
        return current;
    }

    private SyntaxToken ReadToken()
    {
        if (_position >= _text.Length)
            return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "", null);

        var start = _position;
        if (char.IsDigit(_text[_position]))
        {
            while (_position < _text.Length && char.IsDigit(_text[_position]))
                _position++;
            var length = _position - start;
            var text = _text.Substring(start, length);
            int value = int.Parse(text);
            return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
        }

        if (char.IsLetter(_text[_position]))
        {
            while (_position < _text.Length && char.IsLetter(_text[_position]))
                _position++;
            var length = _position - start;
            var text = _text.Substring(start, length);
            var kind = text switch
            {
                "int" => SyntaxKind.IntKeyword,
                "return" => SyntaxKind.ReturnKeyword,
                _ => SyntaxKind.IdentifierToken
            };
            return new SyntaxToken(kind, start, text, null);
        }

        switch (_text[_position])
        {
            case '+':
                _position++;
                return new SyntaxToken(SyntaxKind.PlusToken, start, "+", null);
            case '-':
                _position++;
                return new SyntaxToken(SyntaxKind.MinusToken, start, "-", null);
            case '*':
                _position++;
                return new SyntaxToken(SyntaxKind.StarToken, start, "*", null);
            case '/':
                _position++;
                return new SyntaxToken(SyntaxKind.SlashToken, start, "/", null);
            case '(':
                _position++;
                return new SyntaxToken(SyntaxKind.OpenParenToken, start, "(", null);
            case ')':
                _position++;
                return new SyntaxToken(SyntaxKind.CloseParenToken, start, ")", null);
            case '{':
                _position++;
                return new SyntaxToken(SyntaxKind.OpenBraceToken, start, "{", null);
            case '}':
                _position++;
                return new SyntaxToken(SyntaxKind.CloseBraceToken, start, "}", null);
            case ',':
                _position++;
                return new SyntaxToken(SyntaxKind.CommaToken, start, ",", null);
            case ';':
                _position++;
                return new SyntaxToken(SyntaxKind.SemicolonToken, start, ";", null);
            default:
                _diagnostics.Report(new DiagnosticInfo(DiagnosticCategory.Syntax, DiagnosticCode.UnexpectedToken, DiagnosticSeverity.Error, $"Bad character '{_text[_position]}'"), new TextLocation("", new TextSpan(_position,1)));
                _position++;
                return new SyntaxToken(SyntaxKind.IdentifierToken, start, _text.Substring(start,1), null);
        }
    }

    private SyntaxToken MatchToken(SyntaxKind kind)
    {
        if (Current.Kind == kind)
            return NextToken();
        _diagnostics.Report(new DiagnosticInfo(DiagnosticCategory.Syntax, DiagnosticCode.UnexpectedToken, DiagnosticSeverity.Error, $"Expected {kind}, found {Current.Kind}"), new TextLocation("", Current.Span));
        return new SyntaxToken(kind, Current.Span.Start, string.Empty, null);
    }

    public CompilationUnitSyntax ParseCompilationUnit()
    {
        _position = 0; // reset to beginning of token list
        var method = ParseMethodDeclaration();
        var eof = MatchToken(SyntaxKind.EndOfFileToken);
        return new CompilationUnitSyntax(method, eof);
    }

    private MethodDeclarationSyntax ParseMethodDeclaration()
    {
        var type = MatchToken(SyntaxKind.IntKeyword);
        var id = MatchToken(SyntaxKind.IdentifierToken);
        var openParen = MatchToken(SyntaxKind.OpenParenToken);
        var parameters = new List<ParameterSyntax>();
        while (Current.Kind != SyntaxKind.CloseParenToken && Current.Kind != SyntaxKind.EndOfFileToken)
        {
            var pType = MatchToken(SyntaxKind.IntKeyword);
            var pId = MatchToken(SyntaxKind.IdentifierToken);
            parameters.Add(new ParameterSyntax(pType, pId));
            if (Current.Kind == SyntaxKind.CommaToken)
                NextToken();
            else
                break;
        }
        var closeParen = MatchToken(SyntaxKind.CloseParenToken);
        var body = ParseBlockStatement();
        return new MethodDeclarationSyntax(type, id, openParen, parameters, closeParen, body);
    }

    private BlockStatementSyntax ParseBlockStatement()
    {
        var openBrace = MatchToken(SyntaxKind.OpenBraceToken);
        var statements = new List<StatementSyntax>();
        while (Current.Kind != SyntaxKind.CloseBraceToken && Current.Kind != SyntaxKind.EndOfFileToken)
            statements.Add(ParseStatement());
        var closeBrace = MatchToken(SyntaxKind.CloseBraceToken);
        return new BlockStatementSyntax(openBrace, statements, closeBrace);
    }

    private StatementSyntax ParseStatement()
    {
        return Current.Kind switch
        {
            SyntaxKind.ReturnKeyword => ParseReturnStatement(),
            _ => ParseReturnStatement() // fallback
        };
    }

    private ReturnStatementSyntax ParseReturnStatement()
    {
        var keyword = MatchToken(SyntaxKind.ReturnKeyword);
        var expression = ParseExpression();
        var semicolon = MatchToken(SyntaxKind.SemicolonToken);
        return new ReturnStatementSyntax(keyword, expression, semicolon);
    }

    private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
    {
        ExpressionSyntax left = ParsePrimaryExpression();
        while (true)
        {
            var precedence = GetBinaryOperatorPrecedence(Current.Kind);
            if (precedence == 0 || precedence <= parentPrecedence)
                break;
            var operatorToken = NextToken();
            var right = ParseExpression(precedence);
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }
        return left;
    }

    private static int GetBinaryOperatorPrecedence(SyntaxKind kind)
        => kind switch
        {
            SyntaxKind.PlusToken or SyntaxKind.MinusToken => 1,
            SyntaxKind.StarToken or SyntaxKind.SlashToken => 2,
            _ => 0
        };

    private ExpressionSyntax ParsePrimaryExpression()
    {
        return Current.Kind switch
        {
            SyntaxKind.OpenParenToken => ParseParenthesizedExpression(),
            SyntaxKind.IdentifierToken => new IdentifierNameSyntax(NextToken()),
            _ => new LiteralExpressionSyntax(MatchToken(SyntaxKind.NumberToken)),
        };
    }

    private ExpressionSyntax ParseParenthesizedExpression()
    {
        var open = MatchToken(SyntaxKind.OpenParenToken);
        var expression = ParseExpression();
        var close = MatchToken(SyntaxKind.CloseParenToken);
        return expression;
    }
}
