

namespace Compiler.Assemble.Instruction;

public abstract class Inst {
    public string instName { get; }

    public Inst(string instName) {
        this.instName = instName;
    }
}