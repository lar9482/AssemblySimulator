using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compiler.Assemble.Instruction;

public class ImmInst : Inst {
    public string reg { get; }
    public int integer { get; }

    public ImmInst(string reg, int integer, string instName) 
    : base(instName) {
        this.reg = reg;
        this.integer = integer;
    }
}