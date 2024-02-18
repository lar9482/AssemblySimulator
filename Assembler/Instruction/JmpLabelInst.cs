using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compiler.Assemble.Instruction;

public class JmpLabelInst : Inst {
    public string label { get; }

    public JmpLabelInst(string label, string instName)
    : base(instName) {
        this.label = label;
    }
}