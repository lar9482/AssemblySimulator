using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Compiler.Machine;

public class Machine {

    public Machine(int startProgramAddress) {
        this.startProgramAddress = startProgramAddress;
        this.RAM = new byte[MEMORY_SIZE];
    }

    public void loadProgram(string filePath) {
        string[] hexStrings = File.ReadAllLines(filePath);
        for (int i = 0; i < hexStrings.Length; i++) {
            string hexString = hexStrings[i];

            byte[] decodedHex = BitConverter.GetBytes(Convert.ToInt32(hexString, 16));
            
            // The register program counter will be adjusted if it comes across a main label.
            if (decodedHex.SequenceEqual(MAIN_LABEL)) {
                regPC = WORD_BYTE_SIZE * i;
            }

            /*
             * NOTE: decodedHex[3] is the most significant byte. (Has the opcode)
             * Likewise, decodedHex[0] is the least significant byte
             */
            RAM[startProgramAddress + i*WORD_BYTE_SIZE] = decodedHex[0];
            RAM[startProgramAddress + i*WORD_BYTE_SIZE+1] = decodedHex[1];
            RAM[startProgramAddress + i*WORD_BYTE_SIZE+2] = decodedHex[2];
            RAM[startProgramAddress + i*WORD_BYTE_SIZE+3] = decodedHex[3];
        }
    }

    public void runProgram() {
        byte[] currInstruction = {0, 0, 0, 0};

        while (!currInstruction.SequenceEqual(HALT_INST)) {
            currInstruction = fetchInstruction();
            DecodedInst inst = decodeInstruction(currInstruction);
            regPC += WORD_BYTE_SIZE;
        }
    }

    private byte[] fetchInstruction() {
        byte[] instruction = new byte[4];
        instruction[0] = RAM[regPC];
        instruction[1] = RAM[regPC+1];
        instruction[2] = RAM[regPC+2];
        instruction[3] = RAM[regPC+3];

        return instruction;
    }

    private DecodedInst decodeInstruction(byte[] instruction) {
        int opcode = decodeOpcode(instruction);
        int reg1 = decodeFirstRegister(instruction);
        int reg2 = decodeSecondRegister(instruction);
        int imm = decodeImmediate(instruction);
        int smallJumpOffset = decodeSmallJumpOffset(instruction);
        int largeJumpOffset = decodeLargeJumpOffset(instruction);

        return new DecodedInst(
            opcode,
            reg1,
            reg2,
            imm,
            smallJumpOffset,
            largeJumpOffset
        );
    }

    /*
     * Decoding
     * XXXXXX00 00000000 00000000 00000000
     */
    private int decodeOpcode(byte[] instruction) {
        byte mostSignByte = instruction[WORD_BYTE_SIZE-1];
        return (mostSignByte >> 2);
    }

    /*
     * Decoding
     * 000000XX XXX00000 00000000 00000000
     */
    private int decodeFirstRegister(byte[] instruction) {
        byte firstByte = instruction[WORD_BYTE_SIZE-1];
        byte secondByte = instruction[WORD_BYTE_SIZE-2];

        byte reg1FirstByteMask = 0x3;
        byte reg1SecondByteMask = 0xE0;
        return ((firstByte & reg1FirstByteMask) << 3) + ((secondByte & reg1SecondByteMask) >> 5);
    }

    /*
     * Decoding
     * 00000000 000XXXXX 00000000 00000000
     */
    private int decodeSecondRegister(byte[] instruction) {
        byte secondByte = instruction[WORD_BYTE_SIZE-2];
        byte reg2Mask = 0x1F;
        return secondByte & reg2Mask;
    }

    /*
     * Decoding
     * 00000000 000diiii iiiiiiii iiiiiiii
     */
    private int decodeImmediate(byte[] instruction) {
        byte secondByte = instruction[WORD_BYTE_SIZE-2];
        byte thirdByte = instruction[WORD_BYTE_SIZE-3];
        byte fourthByte = instruction[WORD_BYTE_SIZE-4];
        
        byte secondByteSignMask = 0x10;
        byte secondByteImmSign = 0xF;

        int sign = (secondByte & secondByteSignMask) >> 4;
        int imm = ((secondByte & secondByteImmSign) << 16) + (thirdByte << 8) + fourthByte;

        return (sign == 0) ? imm : ~imm+1;
    }

    /*
     * Decoding
     * 00000000 00000000 diiiiiii iiiiiiii
     */
    private int decodeSmallJumpOffset(byte[] instruction) {
        byte thirdByte = instruction[WORD_BYTE_SIZE-3];
        byte fourthByte = instruction[WORD_BYTE_SIZE-4];

        byte thirdByteSignMask = 0x80;
        byte thirdByteOffsetMask = 0x7F;

        int sign = (thirdByte & thirdByteSignMask) >> 7;
        int offset = ((thirdByte & thirdByteOffsetMask) << 8) + fourthByte;

        return (sign == 0) ? offset : ~offset+1;
    }

    /*
     * Decoding:
     * 000000di iiiiiiii iiiiiiii iiiiiiii
     */
    private int decodeLargeJumpOffset(byte[] instruction) {
        byte firstByte = instruction[WORD_BYTE_SIZE-1];
        byte secondByte = instruction[WORD_BYTE_SIZE-2];
        byte thirdByte = instruction[WORD_BYTE_SIZE-3];
        byte fourthByte = instruction[WORD_BYTE_SIZE-4];

        byte firstByteSignMask = 0x2;
        byte firstByteOffsetMask = 0x1;

        int sign = (firstByte & firstByteSignMask) >> 1;
        int offset = (
            ((firstByte & firstByteOffsetMask) << 24) +
            (secondByte << 16) +
            (thirdByte << 8) +
            (fourthByte)
        );

        return (sign == 0) ? offset : ~offset+1;
    }

    private const int WORD_BYTE_SIZE = 4;
    private const int MEMORY_SIZE = 0xFFFFFF;
    private byte[] MAIN_LABEL = {0, 0, 0, 134}; //Corresponds to the instruction 0x86000000, which is the main label encoding
    private byte[] HALT_INST = {0, 0, 0, 128}; //Corresponds to the instruction 0x80000000, which is the halt instruction.

    private int startProgramAddress; 
    private byte[] RAM;
    private int regPC = 0;
    private int rZERO_Reg = 0;
    private int r1_Reg = 0;
    private int r2_Reg = 0;
    private int r3_Reg = 0;
    private int r4_Reg = 0;
    private int r5_Reg = 0;
    private int r6_Reg = 0;
    private int r7_Reg = 0;
    private int r8_Reg = 0;
    private int r9_Reg = 0;
    private int r10_Reg = 0;
    private int r11_Reg = 0;
    private int r12_Reg = 0;
    private int r13_Reg = 0;
    private int r14_Reg = 0;
    private int r15_Reg = 0;
    private int r16_Reg = 0;
    private int rSP_Reg = 0;
    private int rFP_Reg = 0;
    private int rRET_Reg = 0;
    private int rHI_Reg = 0;
    private int rLO_Reg = 0;

    private struct DecodedInst {
        public int opcode;
        public int reg1;
        public int reg2;
        public int imm;
        public int smallJumpOffset;
        public int largeJumpOffset;

        public DecodedInst(
            int opcode, int reg1, int reg2, int imm,
            int smallJumpOffset, int largeJumpOffset
        ) {
            this.opcode = opcode;
            this.reg1 = reg1;
            this.reg2 = reg2;
            this.imm = imm;
            this.smallJumpOffset = smallJumpOffset;
            this.largeJumpOffset = largeJumpOffset;
        }
    }
}