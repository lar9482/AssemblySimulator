using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compiler.Assemble.Instruction;

public class JmpRegInst : Inst {
    public string reg { get; }

    public JmpRegInst(string reg, string instName)
    : base(instName) {
        this.reg = reg;
    }
}