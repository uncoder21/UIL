namespace UIL.Syntax;

public enum SyntaxKind
{
    // Tokens
    IdentifierToken,
    NumberToken,
    PlusToken,
    MinusToken,
    StarToken,
    SlashToken,
    OpenParenToken,
    CloseParenToken,
    OpenBraceToken,
    CloseBraceToken,
    CommaToken,
    SemicolonToken,
    IntKeyword,
    ReturnKeyword,
    EndOfFileToken,

    // Nodes
    CompilationUnit,
    MethodDeclaration,
    Parameter,
    Type,
    BlockStatement,
    ReturnStatement,
    ExpressionStatement,
    BinaryExpression,
    LiteralExpression,
    IdentifierName
}
