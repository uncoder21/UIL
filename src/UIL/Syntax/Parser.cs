using UIL.Diagnostics;

namespace UIL.Syntax;

/// <summary>
/// A very small recursive descent parser for the UIL language.  The previous
/// version of this file was heavily corrupted which resulted in numerous
/// compilation errors and a completely non‑functional lexer.  This
/// implementation focuses on the minimal set of features exercised by the
/// tests: namespaces, type declarations, methods and simple expressions.
/// </summary>
public sealed class Parser
{
    private readonly string _text;
    private readonly DiagnosticBag _diagnostics = new();
    private readonly List<SyntaxToken> _tokens = new();
    private int _position; // reused for tokenization and parsing

    public Parser(string text)
    {
        _text = text;

        // Tokenize the input immediately so the parser can freely peek ahead
        // without worrying about the underlying characters.
        while (true)
        {
            var token = ReadToken();
            _tokens.Add(token);
            if (token.Kind == SyntaxKind.EndOfFileToken)
                break;
        }
    }

    public DiagnosticBag Diagnostics => _diagnostics;

    // ---------------------------------------------------------------------
    // Tokenization
    // ---------------------------------------------------------------------

    private SyntaxToken ReadToken()
    {
        if (_position >= _text.Length)
            return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, string.Empty, null);

        // Skip whitespace
        if (char.IsWhiteSpace(_text[_position]))
        {
            while (_position < _text.Length && char.IsWhiteSpace(_text[_position]))
                _position++;
            return ReadToken();
        }

        var start = _position;
        var ch = _text[_position];

        // Numbers
        if (char.IsDigit(ch))
        {
            while (_position < _text.Length && char.IsDigit(_text[_position]))
                _position++;
            var text = _text.Substring(start, _position - start);
            int.TryParse(text, out var value);
            return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
        }

        // Identifiers / keywords
        if (char.IsLetter(ch))
        {
            while (_position < _text.Length && char.IsLetter(_text[_position]))
                _position++;
            var text = _text.Substring(start, _position - start);
            var kind = text switch
            {
                "int" => SyntaxKind.IntKeyword,
                "return" => SyntaxKind.ReturnKeyword,
                "class" => SyntaxKind.ClassKeyword,
                "interface" => SyntaxKind.InterfaceKeyword,
                "enum" => SyntaxKind.EnumKeyword,
                "namespace" => SyntaxKind.NamespaceKeyword,
                _ => SyntaxKind.IdentifierToken
            };
            return new SyntaxToken(kind, start, text, null);
        }

        _position++;
        return ch switch
        {
            '+' => new SyntaxToken(SyntaxKind.PlusToken, start, "+", null),
            '-' => new SyntaxToken(SyntaxKind.MinusToken, start, "-", null),
            '*' => new SyntaxToken(SyntaxKind.StarToken, start, "*", null),
            '/' => new SyntaxToken(SyntaxKind.SlashToken, start, "/", null),
            '(' => new SyntaxToken(SyntaxKind.OpenParenToken, start, "(", null),
            ')' => new SyntaxToken(SyntaxKind.CloseParenToken, start, ")", null),
            '{' => new SyntaxToken(SyntaxKind.OpenBraceToken, start, "{", null),
            '}' => new SyntaxToken(SyntaxKind.CloseBraceToken, start, "}", null),
            ',' => new SyntaxToken(SyntaxKind.CommaToken, start, ",", null),
            ';' => new SyntaxToken(SyntaxKind.SemicolonToken, start, ";", null),
            '<' => new SyntaxToken(SyntaxKind.LessThanToken, start, "<", null),
            '>' => new SyntaxToken(SyntaxKind.GreaterThanToken, start, ">", null),
            '[' => new SyntaxToken(SyntaxKind.OpenBracketToken, start, "[", null),
            ']' => new SyntaxToken(SyntaxKind.CloseBracketToken, start, "]", null),
            _ => ReportBadCharacter(start, ch)
        };
    }

    private SyntaxToken ReportBadCharacter(int position, char ch)
    {
        _diagnostics.Report(
            new DiagnosticInfo(
                DiagnosticCategory.Syntax,
                DiagnosticCode.UnexpectedToken,
                DiagnosticSeverity.Error,
                $"Bad character '{ch}'"),
            new TextLocation(string.Empty, new TextSpan(position, 1)));

        return new SyntaxToken(SyntaxKind.IdentifierToken, position, ch.ToString(), null);
    }

    // ---------------------------------------------------------------------
    // Helpers for parsing
    // ---------------------------------------------------------------------

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
        var current = _tokens[_position];
        _position++;
        return current;
    }

    private SyntaxToken MatchToken(SyntaxKind kind)
    {
        if (Current.Kind == kind)
            return NextToken();

        _diagnostics.Report(
            new DiagnosticInfo(
                DiagnosticCategory.Syntax,
                DiagnosticCode.UnexpectedToken,
                DiagnosticSeverity.Error,
                $"Expected {kind}, found {Current.Kind}"),
            new TextLocation(string.Empty, Current.Span));
        return new SyntaxToken(kind, Current.Span.Start, string.Empty, null);
    }

    // ---------------------------------------------------------------------
    // Parsing
    // ---------------------------------------------------------------------

    public CompilationUnitSyntax ParseCompilationUnit()
    {
        // After tokenization, reset position to start consuming tokens
        _position = 0;
        var members = new List<MemberDeclarationSyntax>();
        while (Current.Kind != SyntaxKind.EndOfFileToken)
            members.Add(ParseMemberDeclaration());
        var eof = MatchToken(SyntaxKind.EndOfFileToken);
        return new CompilationUnitSyntax(members, eof);
    }

    private MemberDeclarationSyntax ParseMemberDeclaration()
    {
        var annotations = ParseAnnotations();
        return Current.Kind switch
        {
            SyntaxKind.NamespaceKeyword => ParseNamespaceDeclaration(annotations),
            SyntaxKind.ClassKeyword => ParseClassDeclaration(annotations),
            SyntaxKind.InterfaceKeyword => ParseInterfaceDeclaration(annotations),
            SyntaxKind.EnumKeyword => ParseEnumDeclaration(annotations),
            SyntaxKind.IntKeyword => ParseMethodDeclaration(annotations),
            _ => ParseMethodDeclaration(annotations) // fallback to allow error recovery
        };
    }

    private List<AnnotationSyntax> ParseAnnotations()
    {
        var list = new List<AnnotationSyntax>();
        while (Current.Kind == SyntaxKind.OpenBracketToken)
            list.Add(ParseAnnotation());
        return list;
    }

    private AnnotationSyntax ParseAnnotation()
    {
        var open = MatchToken(SyntaxKind.OpenBracketToken);
        var id = MatchToken(SyntaxKind.IdentifierToken);
        var close = MatchToken(SyntaxKind.CloseBracketToken);
        return new AnnotationSyntax(open, id, close);
    }

    private NamespaceDeclarationSyntax ParseNamespaceDeclaration(IReadOnlyList<AnnotationSyntax> annotations)
    {
        var keyword = MatchToken(SyntaxKind.NamespaceKeyword);
        var id = MatchToken(SyntaxKind.IdentifierToken);
        var openBrace = MatchToken(SyntaxKind.OpenBraceToken);
        var members = new List<MemberDeclarationSyntax>();
        while (Current.Kind != SyntaxKind.CloseBraceToken && Current.Kind != SyntaxKind.EndOfFileToken)
            members.Add(ParseMemberDeclaration());
        var closeBrace = MatchToken(SyntaxKind.CloseBraceToken);
        return new NamespaceDeclarationSyntax(annotations, keyword, id, openBrace, members, closeBrace);
    }

    private ClassDeclarationSyntax ParseClassDeclaration(IReadOnlyList<AnnotationSyntax> annotations)
    {
        var keyword = MatchToken(SyntaxKind.ClassKeyword);
        var id = MatchToken(SyntaxKind.IdentifierToken);
        var (less, typeParams, greater) = ParseTypeParameterList();
        var openBrace = MatchToken(SyntaxKind.OpenBraceToken);
        var members = new List<MemberDeclarationSyntax>();
        while (Current.Kind != SyntaxKind.CloseBraceToken && Current.Kind != SyntaxKind.EndOfFileToken)
            members.Add(ParseMemberDeclaration());
        var closeBrace = MatchToken(SyntaxKind.CloseBraceToken);
        return new ClassDeclarationSyntax(annotations, keyword, id, less, typeParams, greater, openBrace, members, closeBrace);
    }

    private InterfaceDeclarationSyntax ParseInterfaceDeclaration(IReadOnlyList<AnnotationSyntax> annotations)
    {
        var keyword = MatchToken(SyntaxKind.InterfaceKeyword);
        var id = MatchToken(SyntaxKind.IdentifierToken);
        var (less, typeParams, greater) = ParseTypeParameterList();
        var openBrace = MatchToken(SyntaxKind.OpenBraceToken);
        var members = new List<MemberDeclarationSyntax>();
        while (Current.Kind != SyntaxKind.CloseBraceToken && Current.Kind != SyntaxKind.EndOfFileToken)
            members.Add(ParseMemberDeclaration());
        var closeBrace = MatchToken(SyntaxKind.CloseBraceToken);
        return new InterfaceDeclarationSyntax(annotations, keyword, id, less, typeParams, greater, openBrace, members, closeBrace);
    }

    private EnumDeclarationSyntax ParseEnumDeclaration(IReadOnlyList<AnnotationSyntax> annotations)
    {
        var keyword = MatchToken(SyntaxKind.EnumKeyword);
        var id = MatchToken(SyntaxKind.IdentifierToken);
        var openBrace = MatchToken(SyntaxKind.OpenBraceToken);
        var members = new List<SyntaxToken>();
        while (Current.Kind != SyntaxKind.CloseBraceToken && Current.Kind != SyntaxKind.EndOfFileToken)
        {
            var memberId = MatchToken(SyntaxKind.IdentifierToken);
            members.Add(memberId);
            if (Current.Kind == SyntaxKind.CommaToken)
                NextToken();
            else
                break;
        }
        var closeBrace = MatchToken(SyntaxKind.CloseBraceToken);
        return new EnumDeclarationSyntax(annotations, keyword, id, openBrace, members, closeBrace);
    }

    private (SyntaxToken? lessToken, IReadOnlyList<TypeParameterSyntax> typeParameters, SyntaxToken? greaterToken) ParseTypeParameterList()
    {
        SyntaxToken? less = null;
        var parameters = new List<TypeParameterSyntax>();
        SyntaxToken? greater = null;

        if (Current.Kind == SyntaxKind.LessThanToken)
        {
            less = NextToken();
            while (Current.Kind != SyntaxKind.GreaterThanToken && Current.Kind != SyntaxKind.EndOfFileToken)
            {
                var id = MatchToken(SyntaxKind.IdentifierToken);
                parameters.Add(new TypeParameterSyntax(id));
                if (Current.Kind == SyntaxKind.CommaToken)
                    NextToken();
                else
                    break;
            }
            greater = MatchToken(SyntaxKind.GreaterThanToken);
        }

        return (less, parameters, greater);
    }

    private MethodDeclarationSyntax ParseMethodDeclaration(IReadOnlyList<AnnotationSyntax> annotations)
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
        return new MethodDeclarationSyntax(annotations, type, id, openParen, parameters, closeParen, body);
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
        => Current.Kind switch
        {
            SyntaxKind.ReturnKeyword => ParseReturnStatement(),
            _ => ParseReturnStatement() // simple fallback
        };

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
        => Current.Kind switch
        {
            SyntaxKind.OpenParenToken => ParseParenthesizedExpression(),
            SyntaxKind.IdentifierToken => new IdentifierNameSyntax(NextToken()),
            _ => new LiteralExpressionSyntax(MatchToken(SyntaxKind.NumberToken))
        };

    private ExpressionSyntax ParseParenthesizedExpression()
    {
        MatchToken(SyntaxKind.OpenParenToken);
        var expression = ParseExpression();
        MatchToken(SyntaxKind.CloseParenToken);
        return expression;
    }
}

