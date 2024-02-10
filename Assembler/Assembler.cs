using System.IO;
using System.Collections;

using Compiler.Assembler.Tokens;
using Compiler.Assembler.LexicalAnalysis;

namespace Compiler.Assembler;

public class Assembler {
    public static void assembleFile(string filePath) {
        StreamReader sr = new StreamReader(filePath);
        string programContent = sr.ReadToEnd();
        
        assemble(programContent);
    }

    private static void assemble(string content) {
        Lexer lexer = new Lexer();
        Queue<Token> tokens = lexer.lexProgram(content);
    }
}
