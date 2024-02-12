using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Compiler.Assembler.Instruction.AssembledInst;

namespace Compiler.Assembler.Instruction;

public class JmpRegInst : Inst {
    private string reg;

    public JmpRegInst(string reg, string instName, InstType type)
    : base(instName, type) {
        this.reg = reg;
    }

    public override AsmInst assembleInst() {
        throw new NotImplementedException();
    }
}