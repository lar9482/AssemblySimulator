namespace Compiler.Machine;

public class Machine {
    public Machine(int regPCAddress) {
        this.regPC = regPCAddress;
        this.RAM = new byte[memorySize];
    }

    public void loadProgram(string filePath) {
        
    }

    public void runProgram() {

    }

    private const int memorySize = 0xFFFFFF;
    private byte[] RAM;
    private int regPC;
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