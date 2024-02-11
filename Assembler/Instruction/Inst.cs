using Compiler.Assembler.Instruction.AssembledInst;

namespace Compiler.Assembler.Instruction;

public abstract class Inst {
    private string instName;
    private InstType type;

    public Inst(string instName, InstType type) {
        this.instName = instName;
        this.type = type;
    }

    public abstract AsmInst assembleInst();
}