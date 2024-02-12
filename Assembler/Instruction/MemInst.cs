using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Compiler.Assembler.Instruction.AssembledInst;
namespace Compiler.Assembler.Instruction;

public class MemInst : Inst {
    private string reg;
    private string memReg;
    private int offset;

    public MemInst(string reg, string memReg, int offset, string instName, InstType type) 
    : base(instName, type) {
        this.reg = reg;
        this.memReg = memReg;
        this.offset = offset;
    }
    
    public override AsmInst assembleInst() {
        throw new NotImplementedException();
    }
}