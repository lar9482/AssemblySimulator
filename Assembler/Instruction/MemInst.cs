using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compiler.Assembler.Instruction;

public class MemInst : Inst {
    public string reg { get; }
    public string memReg { get; }
    public int offset { get; }

    public MemInst(string reg, string memReg, int offset, string instName) 
    : base(instName) {
        this.reg = reg;
        this.memReg = memReg;
        this.offset = offset;
    }
}