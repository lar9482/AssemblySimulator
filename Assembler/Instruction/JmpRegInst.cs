using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compiler.Assembler.Instruction;

public class JmpRegInst {
    private string reg;

    public JmpRegInst(string reg) {
        this.reg = reg;
    }
}