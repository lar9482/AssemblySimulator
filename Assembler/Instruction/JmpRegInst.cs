using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compiler.Assembler.Instruction;

public class JmpRegInst : Inst {
    private string reg;

    public JmpRegInst(string reg, string instName)
    : base(instName) {
        this.reg = reg;
    }
}