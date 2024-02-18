using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Compiler.Assemble;
using Compiler.AssemblyMachine;

namespace Compiler.Emulator; 

public class Emulator {
    private int startProgramAddress = 0;
    private Assembler assembler;
    private Machine machine;

    public Emulator(int startProgramAddress = 0) {
        this.startProgramAddress = startProgramAddress;
        assembler = new Assembler(startProgramAddress);
        machine = new Machine(startProgramAddress);
    }
}
