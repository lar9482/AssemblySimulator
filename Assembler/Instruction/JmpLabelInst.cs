using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Compiler.Assembler.Instruction.AssembledInst;

namespace Compiler.Assembler.Instruction;

public class JmpLabelInst : Inst {
    private string label;

    public JmpLabelInst(string label, string instName, InstType type)
    : base(instName, type) {
        this.label = label;
    }

    public override AsmInst assembleInst() {
        throw new NotImplementedException();
    }
}