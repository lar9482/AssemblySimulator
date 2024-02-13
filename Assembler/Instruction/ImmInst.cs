using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Compiler.Assembler.Instruction.AssembledInst;
namespace Compiler.Assembler.Instruction;

public class ImmInst : Inst {
    public string reg { get; }
    public int integer { get; }

    public ImmInst(string reg, int integer, string instName, InstType type) 
    : base(instName, type) {
        this.reg = reg;
        this.integer = integer;
    }

    public override AsmInst assembleInst() {
        throw new NotImplementedException();
    }
}