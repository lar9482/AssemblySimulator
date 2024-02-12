using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Compiler.Assembler.Tokens;

namespace Compiler.Assembler.Parse;

public class Parser {
    private Queue<Token> tokenQueue;

    public Parser(Queue<Token> tokenQueue) {
        this.tokenQueue = tokenQueue;
    } 

    public void parse() {

    }
}