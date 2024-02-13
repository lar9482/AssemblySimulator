using System.IO;
using System.Collections;

using Compiler.Assembler.Tokens;
using Compiler.Assembler.Lex;
using Compiler.Assembler.Parse;
using Compiler.Assembler.Instruction;
namespace Compiler.Assembler;

public class Assembler {

    private int baseAddress;
    private Dictionary<string, int> labelAddresses;

    private const int instSize = 4;

    public Assembler(int baseAddress) {
        this.baseAddress = baseAddress;
        this.labelAddresses = new Dictionary<string, int>();
    }
    
    public void assembleFile(string filePath) {
        StreamReader sr = new StreamReader(filePath);
        List<Inst> instructions = parseProgram(sr.ReadToEnd());
        computeLabelAddresses(instructions);

        List<string> assembledInstructions = new List<string>();
        foreach (Inst instruction in instructions) {
            var test = instruction.GetType().Name;
            Console.WriteLine();
            switch (instruction.GetType().Name) {
                case "LabelInst":
                    assembledInstructions.Add(
                        assembleLabelInst(
                            (LabelInst) instruction
                        )
                    );
                    break;
                case "RegInst":
                    assembledInstructions.Add(
                        assembleRegInst(
                            (RegInst) instruction
                        )
                    );
                    break;
                case "ImmInst":
                    break;
                case "MemInst":
                    break;
                case "JmpRegInst":
                    break;
                case "JmpLabel":
                    break;
                case "JmpBranch":
                    break;
                default:
                    break;
            }
        }
    }

    private List<Inst> parseProgram(string content) {
        Lexer lexer = new Lexer();
        Queue<Token> tokens = lexer.lexProgram(content);
        
        Parser parser = new Parser(tokens);
        return parser.parse();
    }

    private void computeLabelAddresses(List<Inst> instructions) {
        for(int i = 0; i < instructions.Count; i++) {
            Inst instruction = instructions[i];

            if (instruction.GetType() == typeof(LabelInst)) {
                labelAddresses.Add(instruction.instName, baseAddress + i*instSize);
            }
        }
    }

    /*
    * Target format:
    * ooooooss sssttttt aaaaaaaa aaaaaaaa
    *
    * o: opcode binary
    * s: reg1 binary
    * t: reg2 binary
    * a: placeholder zeros.
    */
    private string assembleRegInst(RegInst instruction) {
        int opcodeBin = assembleOpcode(instruction.instName);
        int reg1Bin = assembleRegister(instruction.reg1);
        int reg2Bin = assembleRegister(instruction.reg2);

        int bin = opcodeBin << 26;
        bin += (reg1Bin << 21);
        bin += (reg2Bin << 16);

        string hexString = bin.ToString("X8");
        return hexString;
    }
    /**
    */

    private string assembleImmInst(ImmInst instruction) {
        return "";
    }

    private string assembleLabelInst(LabelInst instruction) {
        int binAddress = labelAddresses[instruction.instName];
        return binAddress.ToString("X8");
    }

    private int assembleRegister(string register) {
        switch (register) {
            case "rZERO": return 0;
            case "r1": return 1;
            case "r2": return 2;
            case "r3": return 3;
            case "r4": return 4;
            case "r5": return 5;
            case "r6": return 6;
            case "r7": return 7;
            case "r8": return 8;
            case "r9": return 9;
            case "r10": return 10;
            case "r11": return 11;
            case "r12": return 12;
            case "r13": return 13;
            case "r14": return 14;
            case "r15": return 15;
            case "r16": return 16;
            case "rSP": return 17;
            case "rFP": return 18;
            case "rPC":   return 19;  
            case "rRET": return 20;
            case "rHI":  return 21;
            case "rLO": return 22;
            default:
                throw new Exception(String.Format("{0} is not a valid register", register));
        }
    }

    private int assembleOpcode(string opcode) {
        switch(opcode) {
            case "mov": return 0;
            case "add": return 1;
            case "sub": return 2;
            case "mult": return 3;
            case "div": return 4;
            case "and": return 5;
            case "or": return 6;
            case "xor": return 7;
            case "not": return 8;
            case "nor": return 9;
            case "sllv": return 10;
            case "srav": return 11;
            case "movI": return 12;
            case "addI": return 13;
            case "subI": return 14;
            case "mulI": return 15;
            case "divI": return 16;
            case "andI": return 17;
            case "orI": return 18;
            case "xorI": return 19;
            case "sll": return 20;
            case "sra": return 21;
            case "bEq":  return 22;
            case "bNe": return 23;
            case "jmp": return 24;
            case "jmpL": return 25;
            case "jmpL_Reg": return 26;
            case "jmpReg": return 27;
            case "lb": return 28;
            case "lw": return 29;
            case "sb": return 30;
            case "sw": return 31;
            default:
                throw new Exception(String.Format("{0} is not a valid opcode", opcode));
        }
    }

    private void printBytes(byte[] bytes) {
        Array.Reverse(bytes);
        foreach (byte b in bytes) {   
            Console.Write(Convert.ToString(b, 2).PadLeft(8, '0')); // Print each byte in binary format
        }
    }
}
