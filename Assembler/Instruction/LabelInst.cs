using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Compiler.Assemble.Instruction;

public class LabelInst : Inst {
    public string label { get; }
    
    public LabelInst(string label, string instName) : base(instName){
        this.label = label;
    }
}