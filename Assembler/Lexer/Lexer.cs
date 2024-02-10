using System.Text.RegularExpressions;
using System.Collections;
using Compiler.Assembler.Tokens;

namespace Compiler.Assembler.LexicalAnalysis;

public class Lexer {
    private readonly Regex matchIdentifier;
    private readonly Regex matchWhitespace;
    private readonly Regex matchOneSymbol;
    private readonly Regex matchIntegers;

    private int lineCounter;

    public Lexer() {
        this.matchIdentifier = new Regex(@"\b^[a-zA-Z]{1}[a-zA-Z0-9_]*\b");
        this.matchWhitespace = new Regex(@"\b^\n|\t|\s|\r\b");
        this.matchOneSymbol = new Regex(@"^[,:]");
        this.matchIntegers = new Regex(@"^-?\b\d+\b");
        this.lineCounter = 1;
    }

    public List<Token> lexProgram(string programText) {
        List<Token> tokens = new List<Token>();
        while (programText.Length > 0) {
            Tuple<string, string> longestMatchWithType = scanLongestMatch(programText);
            string matchedLexeme = longestMatchWithType.Item1;
            string matchType = longestMatchWithType.Item2;

            switch(matchType) {
                case "matchIdentifier":
                    tokens.Add(resolveWordLexeme(matchedLexeme));
                    break;
                case "matchOneSymbol":
                    tokens.Add(resolveOneSymbolLexeme(matchedLexeme));
                    break;
                case "matchIntegers":
                    tokens.Add(
                        new Token(matchedLexeme, lineCounter, TokenType.integer)
                    );
                    break;
                case "matchWhitespace":
                    if (matchedLexeme == "\n") {
                        lineCounter++;
                    }
                    break;
                default:
                    throw new InvalidOperationException(
                        String.Format("Lexer: {0} is not a recognizable lexeme", matchedLexeme)
                    );
            }
            programText = programText.Remove(0, matchedLexeme.Length);
        }

        return tokens;
    }

    private Tuple<string, string> scanLongestMatch(string programText) {
        Dictionary<string, string> matches = new Dictionary<string, string>();
        matches.Add("matchIdentifier", matchIdentifier.Match(programText).Value);
        matches.Add("matchOneSymbol", matchOneSymbol.Match(programText).Value);
        matches.Add("matchIntegers", matchIntegers.Match(programText).Value);
        matches.Add("matchWhitespace", matchWhitespace.Match(programText).Value);

        int longestMatchLength = 0;
        string longestMatch = "";
        string matchType = "";

        foreach(KeyValuePair<string, string> regexProgramMatch in matches) {
            if (regexProgramMatch.Value.Length > longestMatchLength) {
                longestMatch = regexProgramMatch.Value;
                matchType = regexProgramMatch.Key;
                longestMatchLength = regexProgramMatch.Value.Length;
            }
        }

        return Tuple.Create<string, string>(longestMatch, matchType);
    }

    private Token resolveOneSymbolLexeme(string lexeme) {
        switch(lexeme) {
            case ",":
                return new Token(lexeme, lineCounter, TokenType.comma);
            case ":":
                return new Token(lexeme, lineCounter, TokenType.colon);
            default:
                throw new Exception(String.Format("The one symbol lexeme {0} is unrecognizable", lexeme));
        }
    }

    private Token resolveWordLexeme(string lexeme) {
        switch(lexeme) {
            default:
                return new Token(lexeme, lineCounter, TokenType.identifier);
        }
    }
}
