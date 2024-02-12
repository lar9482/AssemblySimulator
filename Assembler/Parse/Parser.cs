using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Compiler.Assembler.Tokens;
using Compiler.Assembler.Instruction;

namespace Compiler.Assembler.Parse;

public class Parser {
    private Queue<Token> tokenQueue;
    
    public Parser(Queue<Token> tokenQueue) {
        this.tokenQueue = tokenQueue;
    } 

    public List<Inst> parse() {
        List<Inst> instructions = new List<Inst>();

        switch(tokenQueue.Peek().type) {
            case TokenType.mov_Inst:
            case TokenType.add_Inst:
            case TokenType.sub_Inst:
            case TokenType.mult_Inst:
            case TokenType.div_Inst:
            case TokenType.and_Inst:
            case TokenType.or_Inst:
            case TokenType.xor_Inst:
            case TokenType.not_Inst:
            case TokenType.nor_Inst:
            case TokenType.sllv_Inst:
            case TokenType.srav_Inst:
                instructions.Add(parseRegInst());
                break;
            case TokenType.movI_Inst:
            case TokenType.addI_Inst:
            case TokenType.subI_Inst:
            case TokenType.multI_Inst:
            case TokenType.divI_Inst:
            case TokenType.andI_Inst:
            case TokenType.orI_Inst:
            case TokenType.xorI_Inst:
            case TokenType.sll_Inst:
            case TokenType.sra_Inst:
                instructions.Add(parseImmInst());
                break;
            case TokenType.EOF:
                return instructions;
            default:
                throw new Exception(String.Format("Line {0}: {1} is not a valid instruction name",
                    tokenQueue.Peek().lineCount, tokenQueue.Peek().lexeme
                ));
        }

        List<Inst> nextInstructions = parse();
        return instructions.Concat(nextInstructions).ToList();
    }

    private RegInst parseRegInst() {
        Token opcodeToken = consume(tokenQueue.Peek().type);
        Token reg1Token = parseRegister();
        consume(TokenType.comma);
        Token reg2Token = parseRegister();

        return new RegInst(
            reg1Token.lexeme,
            reg2Token.lexeme,
            opcodeToken.lexeme,
            InstType.RegInst
        );
    }

    private ImmInst parseImmInst() {
        Token opcodeToken = consume(tokenQueue.Peek().type);
        Token regToken = parseRegister();
        consume(TokenType.comma);
        Token integerToken = consume(TokenType.integer);

        return new ImmInst(
            regToken.lexeme,
            Int32.Parse(integerToken.lexeme),
            opcodeToken.lexeme,
            InstType.ImmInst
        );
    }

    private Token parseRegister() {
        switch(tokenQueue.Peek().type) {
            case TokenType.rZERO_Reg:
            case TokenType.r1_Reg:
            case TokenType.r2_Reg:
            case TokenType.r3_Reg:
            case TokenType.r4_Reg:
            case TokenType.r5_Reg:
            case TokenType.r6_Reg:
            case TokenType.r7_Reg:
            case TokenType.r8_Reg:
            case TokenType.r9_Reg:
            case TokenType.r10_Reg:
            case TokenType.r11_Reg:
            case TokenType.r12_Reg:
            case TokenType.r13_Reg:
            case TokenType.r14_Reg:
            case TokenType.r15_Reg:
            case TokenType.r16_Reg:
            case TokenType.rSP_Reg:
            case TokenType.rFP_Reg:
            case TokenType.rPC_Reg:    
            case TokenType.rRET_Reg:
            case TokenType.rHI_Reg: 
            case TokenType.rLO_Reg: 
                return consume(tokenQueue.Peek().type);
            default:
                throw new Exception(String.Format("Line {0}: {1} is not a valid register name", 
                    tokenQueue.Peek().lineCount, 
                    tokenQueue.Peek().lexeme)
                );
        }
    }

    private Token consume(TokenType currTokenType) {
        Token expectedToken = tokenQueue.Peek();
        if (expectedToken.type == currTokenType) {
            return tokenQueue.Dequeue();
        } else {
            throw new Exception(String.Format("{0} does not match with the expected token {1}", 
                currTokenType.ToString(), expectedToken.type.ToString()
            ));
        }
    }
}