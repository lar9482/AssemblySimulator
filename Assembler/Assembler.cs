using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Compiler.Assembler.Tokens;
namespace Compiler.Assembler;

public class Assembler {
    public static void assembleFile(string filePath) {
        TokenType test = TokenType.add_Inst;
        Console.WriteLine(filePath);
    }

    private static void assemble(string content) {

    }
}
