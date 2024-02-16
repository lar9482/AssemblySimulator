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

    }

    private const int WORD_BYTE_SIZE = 4;
    private const int MEMORY_SIZE = 0xFFFFFF;
    private byte[] MAIN_LABEL = {0, 0, 0, 134}; //Corresponds to the instruction 0x86000000, which is the main label encoding

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
}