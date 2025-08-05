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
    ClassKeyword,
    InterfaceKeyword,
    EnumKeyword,
    NamespaceKeyword,
    LessThanToken,
    GreaterThanToken,
    OpenBracketToken,
    CloseBracketToken,
    EndOfFileToken,

    // Nodes
    CompilationUnit,
    NamespaceDeclaration,
    ClassDeclaration,
    InterfaceDeclaration,
    EnumDeclaration,
    MethodDeclaration,
    Parameter,
    TypeParameter,
    Annotation,
    Type,
    BlockStatement,
    ReturnStatement,
    ExpressionStatement,
    BinaryExpression,
    LiteralExpression,
    IdentifierName
}
