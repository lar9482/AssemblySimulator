using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

using Compiler.Assembler.Instruction.AssembledInst;
namespace Compiler.Assembler.Instruction;

public class LabelInst : Inst {

    public LabelInst(string label, InstType type) : base(label, type){
    }

    public override AsmInst assembleInst() {
        throw new NotImplementedException();
    }
}