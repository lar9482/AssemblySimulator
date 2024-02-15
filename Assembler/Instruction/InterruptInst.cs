using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compiler.Assembler.Instruction;

public class InterruptInst : Inst {
    public string command { get; }
    public InterruptInst(string command, string instName) : base(instName){
        this.command = command;
    }
}