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

    private const int instSizeByte = 4;

    public Assembler(int baseAddress) {
        this.baseAddress = baseAddress;
        this.labelAddresses = new Dictionary<string, int>();
    }
    
    public void assembleFile(string filePathInput, string filePathOutput) {
        StreamReader sr = new StreamReader(filePathInput);
        List<Inst> instructions = parseProgram(sr.ReadToEnd());
        computeLabelAddresses(instructions);

        List<string> assembledInstructions = new List<string>();
        for (int i = 0; i < instructions.Count; i++) {
            Inst instruction = instructions[i];

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
                    assembledInstructions.Add(
                        assembleImmInst(
                            (ImmInst) instruction
                        )
                    );
                    break;
                case "MemInst":
                    assembledInstructions.Add(
                        assembleMemInst(
                            (MemInst) instruction
                        )
                    );
                    break;
                case "JmpRegInst":
                    assembledInstructions.Add(
                        assembleJmpRegInst(
                            (JmpRegInst) instruction
                        )
                    );
                    break;
                case "JmpLabelInst":
                    assembledInstructions.Add(
                        assembleJmpLabelInst(
                            (JmpLabelInst) instruction, i
                        )
                    );
                    break;
                case "JmpBranchInst":
                    assembledInstructions.Add(
                        assembleJmpBranchInst(
                            (JmpBranchInst) instruction, i
                        )
                    );
                    break;
                case "InterruptInst":
                    assembledInstructions.Add(
                        assembleInterruptInst(
                            (InterruptInst) instruction
                        )
                    );
                    break;
                default:
                    throw new Exception("Unexpected instruction seen");
            }
        }

        saveAssembledProgram(assembledInstructions, filePathOutput);
    }

    private void saveAssembledProgram(List<string> assembledInstructions, string filePath) {

        using (StreamWriter writer = new StreamWriter(filePath)) {
            foreach (string str in assembledInstructions) {
                writer.WriteLine(str);
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
                labelAddresses.Add(instruction.instName, baseAddress + i*instSizeByte);
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
        bin += reg1Bin << 21;
        bin += reg2Bin << 16;
        return bin.ToString("X8");
    }

    /*
     * Target format:
     * ooooooss sssdiiii iiiiiiii iiiiiiii
     *
     * o: opcode binary
     * s: reg binary
     * d: sign of the immediate. 1 is negative. 0 is positive
     * i: The immediate number binary. Represented as a two's complement.
     */
    private string assembleImmInst(ImmInst instruction) {
        //These numbers define the bounds of a number that can fit in 20 binary digits,
        //which is the allocated space for an immmediate number
        if (instruction.integer > 1048575 || instruction.integer < -1048575) {
            throw new Exception(String.Format(
                "Invalid immediate instruction. It must inbetween 1048575 and -1048575"
            ));
        }

        int opcodeBin = assembleOpcode(instruction.instName);
        int regBin = assembleRegister(instruction.reg);
        int signBin;
        int immBin;
        if (instruction.integer < 0) {
            signBin = 1;
            immBin = ~instruction.integer+1;
        } else {
            signBin = 0;
            immBin = instruction.integer;
        }
        int bin = opcodeBin << 26;
        bin += regBin << 21;
        bin += signBin << 20;
        bin += immBin;
        
        return bin.ToString("X8");
    }

    /*
     * Target format:
     * ooooooss sssttttt diiiiiii iiiiiiii
     *
     * o: opcode binary
     * s: reg binary
     * t: memReg binary
     * d: sign of the offset. 1 is negative. 0 is positive
     * i: The offset number binary. Represented as a two's complement.
     */
    private string assembleMemInst(MemInst instruction) {
        //These numbers define the bounds of a number that can fit in 15 binary digits,
        //which is the allocated space for an immmediate number
        if (instruction.offset > 32767 || instruction.offset < -32767) {
            throw new Exception("Invalid memory instruction. Offset must be inbetween 32767 and -32767");
        }

        int opcodeBin = assembleOpcode(instruction.instName);
        int regBin = assembleRegister(instruction.reg);
        int memRegBin = assembleRegister(instruction.memReg);
        int signBin;
        int offsetBin;
        if (instruction.offset < 0) {
            signBin = 1;
            offsetBin = ~instruction.offset+1;
        } else {
            signBin = 0;
            offsetBin = instruction.offset;
        }
        int bin = opcodeBin << 26;
        bin += regBin << 21;
        bin += memRegBin << 16;
        bin += signBin << 15;
        bin += offsetBin;

        return bin.ToString("X8");
    }

    /*
     * Target format:
     * oooooodi iiiiiiii iiiiiiii iiiiiiii
     *
     * o: opcode binary
     * d: sign of the offset. 1 is negative. 0 is positive
     * i: The jump number binary. Represented as a two's complement.
     */
    private string assembleJmpLabelInst(JmpLabelInst instruction, int place) {
        int opcodeBin = assembleOpcode(instruction.instName);

        int currentAddress = instSizeByte*place + baseAddress;
        int labelAddress = labelAddresses[instruction.label];
        int jumpOffsetBin = (labelAddress - (currentAddress - 4)) >> 2;

        //These numbers define the bounds of a number that can fit in 25 binary digits,
        //which is the allocated space for an immmediate number
        if (jumpOffsetBin > 33554431 || jumpOffsetBin < -33554431) {
            throw new Exception("Invalid jump label instruction. jump offset must be inbetween 33554431 and -33554431");
        }

        int sign;
        if (jumpOffsetBin < 0) {
            sign = 1;
            jumpOffsetBin = ~jumpOffsetBin+1;
        } else {
            sign = 0;
        }
        
        int bin = opcodeBin << 26;
        bin += sign << 25;
        bin += jumpOffsetBin;
        return bin.ToString("X8");
    }

    /*
     * Target format:
     * ooooooss sss00000 00000000 00000000
     *
     * o: opcode binary
     * s: reg binary
     * 0: placeholder zeros
     */
    private string assembleJmpRegInst(JmpRegInst instruction) {
        int opcodeBin = assembleOpcode(instruction.instName);
        int regBin = assembleRegister(instruction.reg);
        int bin = opcodeBin << 26;
        bin += regBin << 21;

        return bin.ToString("X8");
    }

    /*
     * Target format:
     * ooooooss sssttttt diiiiiii iiiiiiii
     *
     * o: opcode binary
     * s: reg1 binary
     * t: reg2 binary
     * d: sign of the offset. 1 is negative. 0 is positive
     * i: The jump offset number binary. Represented as a two's complement.
     */
    private string assembleJmpBranchInst(JmpBranchInst instruction, int place) {
        int opcodeBin = assembleOpcode(instruction.instName);
        int reg1Bin = assembleRegister(instruction.reg1);
        int reg2Bin = assembleRegister(instruction.reg2);

        int currentAddress = instSizeByte*place + baseAddress;
        int labelAddress = labelAddresses[instruction.label];
        int jumpOffsetBin = (labelAddress - (currentAddress - 4)) >> 2;
        //These numbers define the bounds of a number that can fit in 15 binary digits,
        //which is the allocated space for an immmediate number
        if (jumpOffsetBin > 32767 || jumpOffsetBin < -32767) {
            throw new Exception("Invalid jump branch instruction. Offset must be inbetween 32767 and -32767");
        }

        int sign;
        if (jumpOffsetBin < 0) {
            sign = 1;
            jumpOffsetBin = ~jumpOffsetBin+1;
        } else {
            sign = 0;
        }
        
        int bin = opcodeBin << 26;
        bin += reg1Bin << 21;
        bin += reg2Bin << 16;
        bin += sign << 15;
        bin += jumpOffsetBin;
        
        return bin.ToString("X8");
    }

    /*
     * Target format:
     * oooooocc ccc00000 00000000 00000000
     *
     * o: opcode binary
     * s: command binary
     * 0: placeholder zeros
     */
    private string assembleInterruptInst(InterruptInst instruction) {
        int opcodeBin = assembleOpcode(instruction.instName);
        int commandBin = assembleInterruptCommand(instruction.command);

        int bin = opcodeBin << 26;
        bin += commandBin << 21;
        
        return bin.ToString("X8");
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
            case "multI": return 15;
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
            case "interrupt": return 32;
            default:
                throw new Exception(String.Format("{0} is not a valid opcode", opcode));
        }
    }

    private int assembleInterruptCommand(string command) {
        switch(command) {
            case "halt":
                return 0;
            default:
                throw new Exception("Unrecognized interrupt command");
        }
    }

    private void printBytes(byte[] bytes) {
        Array.Reverse(bytes);
        foreach (byte b in bytes) {   
            Console.Write(Convert.ToString(b, 2).PadLeft(8, '0')); // Print each byte in binary format
        }
    }
}
