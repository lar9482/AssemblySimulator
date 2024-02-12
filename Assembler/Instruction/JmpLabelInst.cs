using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compiler.Assembler.Instruction;

public class JmpLabelInst {
    private string label;

    public JmpLabelInst(string label) {
        this.label = label;
    }
}