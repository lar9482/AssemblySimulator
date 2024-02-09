namespace Compiler.Assembler.Tokens;

public class Token {
    private string lexeme;
    private int lineCount;
    private TokenType type;

    public Token(string lexeme, int lineCount, TokenType type) {
        this.lexeme = lexeme;
        this.lineCount = lineCount;
        this.type = type;
    }
}