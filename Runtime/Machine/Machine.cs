namespace Compiler.Runtime;

public class Machine {

    public Machine(int startProgramAddress) {
        this.startProgramAddress = startProgramAddress;
        this.RAM = new byte[MEMORY_SIZE];
        this.registers = new int[typeof(RegID).GetFields().Length];

        registers[RegID.rZERO] = 0;
        registers[RegID.r1] = 0;
        registers[RegID.r2] = 0;
        registers[RegID.r3] = 0;
        registers[RegID.r4] = 0;
        registers[RegID.r5] = 0;
        registers[RegID.r6] = 0;
        registers[RegID.r7] = 0;
        registers[RegID.r8] = 0;
        registers[RegID.r9] = 0;
        registers[RegID.r10] = 0; 
        registers[RegID.r11] = 0; 
        registers[RegID.r12] = 0; 
        registers[RegID.r13] = 0; 
        registers[RegID.r14] = 0; 
        registers[RegID.r15] = 0; 
        registers[RegID.r16] = 0; 
        registers[RegID.rSP] = 0; 
        registers[RegID.rFP] = 0; 
        registers[RegID.rRET] = 0;
        registers[RegID.rHI] = 0; 
        registers[RegID.rLO] = 0; 
    }

    public void loadProgram(string filePath) {
        string[] hexStrings = File.ReadAllLines(filePath);
        for (int i = 0; i < hexStrings.Length; i++) {
            string hexString = hexStrings[i];

            byte[] decodedHex = BitConverter.GetBytes(Convert.ToInt32(hexString, 16));
            
            // The register program counter will be adjusted if it comes across a main label.
            if (decodedHex.SequenceEqual(MAIN_LABEL)) {
                regPC = startProgramAddress + WORD_BYTE_SIZE * i;
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
            executeInstruction(inst);
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

    private void executeInstruction(DecodedInst inst) {
        switch (inst.opcode) {
            case Opcode.mov:
                registers[inst.reg1] = registers[inst.reg2];
                break;
            case Opcode.add:
                registers[inst.reg1] += registers[inst.reg2];
                break;
            case Opcode.sub:
                registers[inst.reg1] -= registers[inst.reg2];
                break;
            case Opcode.mult:
                registers[RegID.rHI] = registers[inst.reg1] * registers[inst.reg2];
                registers[RegID.rLO] = registers[inst.reg1] * registers[inst.reg2];
                break;
            case Opcode.div:
                registers[RegID.rHI] = (int) registers[inst.reg1] / registers[inst.reg2];
                registers[RegID.rLO] = registers[inst.reg1] % registers[inst.reg2];
                break;
            case Opcode.and:
                registers[inst.reg1] = registers[inst.reg1] & registers[inst.reg2];
                break;
            case Opcode.or:
                registers[inst.reg1] = registers[inst.reg1] | registers[inst.reg2];
                break;
            case Opcode.xor:
                registers[inst.reg1] = registers[inst.reg1] ^ registers[inst.reg2];
                break;
            case Opcode.nor:
                registers[inst.reg1] = ~(registers[inst.reg1] | registers[inst.reg2]);
                break;
            case Opcode.sllv:
                registers[inst.reg1] = registers[inst.reg1] << registers[inst.reg2];
                break;
            case Opcode.srav:
                registers[inst.reg1] = registers[inst.reg1] >> registers[inst.reg2];
                break;
            case Opcode.movI:
                registers[inst.reg1] = inst.imm;
                break;
            case Opcode.addI:
                registers[inst.reg1] += inst.imm;
                break;
            case Opcode.subI:
                registers[inst.reg1] -= inst.imm;
                break;
            case Opcode.multI:
                registers[RegID.rHI] = registers[inst.reg1] * inst.imm;
                registers[RegID.rLO] = registers[inst.reg1] * inst.imm;
                break;
            case Opcode.divI:
                registers[RegID.rHI] = (int) registers[inst.reg1] / inst.imm;
                registers[RegID.rLO] = registers[inst.reg1] % inst.imm;
                break;
            case Opcode.andI:
                registers[inst.reg1] = registers[inst.reg1] & inst.imm;
                break;
            case Opcode.orI:
                registers[inst.reg1] = registers[inst.reg1] | inst.imm;
                break;
            case Opcode.xorI:
                registers[inst.reg1] = registers[inst.reg1] ^ inst.imm;
                break;
            case Opcode.sll:
                registers[inst.reg1] = registers[inst.reg1] << inst.imm;
                break;
            case Opcode.sra:
                registers[inst.reg1] = registers[inst.reg1] >> inst.imm;
                break;
            case Opcode.bEq:
                if (registers[inst.reg1] == registers[inst.reg2]) {
                    regPC += (inst.smallJumpOffset) << 2;
                }
                break;
            case Opcode.bNe:
                if (registers[inst.reg1] != registers[inst.reg2]) {
                    regPC += (inst.smallJumpOffset) << 2;
                }
                break;
            case Opcode.jmp:
                regPC += (inst.largeJumpOffset) << 2;
                break;
            case Opcode.jmpL:
                registers[RegID.rRET] = regPC;
                regPC += (inst.largeJumpOffset) << 2;
                break;
            case Opcode.jmpL_Reg:
                registers[RegID.rRET] = regPC;
                regPC = registers[inst.reg1];
                break;
            case Opcode.jmpReg:
                regPC = registers[inst.reg1];
                break;
            case Opcode.lb:
                int addressLB = registers[inst.reg2]+inst.smallJumpOffset;
                byte memByteLB = RAM[addressLB];
                registers[inst.reg1] = memByteLB;
                break;
            case Opcode.lw:
                int addressLW = registers[inst.reg2]+inst.smallJumpOffset;
                byte[] memWordLW = new byte[] {
                    RAM[addressLW],
                    RAM[addressLW+1],
                    RAM[addressLW+2],
                    RAM[addressLW+3]
                };
                registers[inst.reg1] = BitConverter.ToInt32(memWordLW, 0);
                break;
            case Opcode.sb:
                byte[] reg1BytesSB = BitConverter.GetBytes(registers[inst.reg1]);
                byte leastSignByteSB = reg1BytesSB[0];

                int addressSB = registers[inst.reg2]+inst.smallJumpOffset;
                RAM[addressSB] = leastSignByteSB;
                break;
            case Opcode.sw:
                byte[] reg1BytesSW = BitConverter.GetBytes(registers[inst.reg1]);
                int addressSW = registers[inst.reg2]+inst.smallJumpOffset;
                RAM[addressSW] = reg1BytesSW[0];
                RAM[addressSW+1] = reg1BytesSW[1];
                RAM[addressSW+2] = reg1BytesSW[2];
                RAM[addressSW+3] = reg1BytesSW[3];
                break;
            case Opcode.interrupt:
            case Opcode.label:
                break;
            default:
                throw new Exception(String.Format("Unrecognized opcode {0}", inst.opcode));
        }
    }

    private const int WORD_BYTE_SIZE = 4;
    private const int MEMORY_SIZE = 0xFFFFFF;
    private byte[] MAIN_LABEL = {0, 0, 0, 134}; //Corresponds to the instruction 0x86000000, which is the main label encoding
    private byte[] HALT_INST = {0, 0, 0, 128}; //Corresponds to the instruction 0x80000000, which is the halt instruction.

    private int startProgramAddress; 
    private byte[] RAM;
    private int regPC = 0;
    private int[] registers;
    

    /**
     * NOTE: The order/assignments of RegID and Opcode are important 
     * because they match with the instruction encodings.
     *
     * This will allow the machine to do clean pattern matching when referencing the 
     * registers and opcodes.
     * 
     */
    private struct RegID {
        public const int rZERO = 0;
        public const int r1 = 1;
        public const int r2 = 2;
        public const int r3 = 3;
        public const int r4 = 4;
        public const int r5 = 5;
        public const int r6 = 6;
        public const int r7 = 7;
        public const int r8 = 8;
        public const int r9 = 9;
        public const int r10 = 10;
        public const int r11 = 11;
        public const int r12 = 12;
        public const int r13 = 13;
        public const int r14 = 14;
        public const int r15 = 15;
        public const int r16 = 16;
        public const int rSP = 17;
        public const int rFP = 18;
        public const int rRET = 19;
        public const int rHI = 20;
        public const int rLO = 21;
        public RegID() {}
    }

    private struct Opcode {
        public const int mov = 0;
        public const int add = 1;
        public const int sub = 2;
        public const int mult = 3;
        public const int div = 4;
        public const int and = 5;
        public const int or = 6;
        public const int xor = 7;
        public const int not = 8;
        public const int nor = 9;
        public const int sllv = 10;
        public const int srav = 11;
        public const int movI = 12;
        public const int addI = 13;
        public const int subI = 14;
        public const int multI = 15;
        public const int divI = 16;
        public const int andI = 17;
        public const int orI = 18;
        public const int xorI = 19;
        public const int sll = 20;
        public const int sra = 21;
        public const int bEq = 22;
        public const int bNe = 23;
        public const int jmp = 24;
        public const int jmpL = 25;
        public const int jmpL_Reg = 26;
        public const int jmpReg = 27;
        public const int lb = 28;
        public const int lw = 29;
        public const int sb = 30;
        public const int sw = 31;
        public const int interrupt = 32;
        public const int label = 33;
        public Opcode() {}
    }

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