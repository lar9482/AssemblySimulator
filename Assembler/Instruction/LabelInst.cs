using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Compiler.Assembler.Instruction;

public class LabelInst : Inst {

    public LabelInst(string label) : base(label){
    }
}