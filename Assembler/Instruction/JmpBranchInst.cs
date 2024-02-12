using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compiler.Assembler.Instruction;

public class JmpBranchInst {
    private string reg1;
    private string reg2;

    private string label;

    public JmpBranchInst(string reg1, string reg2, string label) {
        this.reg1 = reg1;
        this.reg2 = reg2;
        this.label = label;
    }
}