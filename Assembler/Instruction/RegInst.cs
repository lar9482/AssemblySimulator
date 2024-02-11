using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Compiler.Assembler.Instruction.AssembledInst;
namespace Compiler.Assembler.Instruction;

public class RegInst : Inst {
    private string reg1;
    private string reg2;

    public RegInst(string reg1, string reg2, string instName, InstType type) 
    : base(instName, type) {
        this.reg1 = reg1;
        this.reg2 = reg2;
    }

    public override AsmInst assembleInst() {
        throw new NotImplementedException();
    }
}