using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compiler.Assembler.Instruction.AssembledInst;

public struct AsmInst {
    private byte byteAddress1;
    private byte byteAddress2;
    private byte byteAddress3;
    private byte byteAddress4;

    private byte byteContent1;
    private byte byteContent2;
    private byte byteContent3;
    private byte byteContent4;

    public AsmInst(
        byte byteAddress1, 
        byte byteAddress2, 
        byte byteAddress3, 
        byte byteAddress4,
        byte byteContent1,
        byte byteContent2,
        byte byteContent3,
        byte byteContent4
    ) {
        this.byteAddress1 = byteAddress1;
        this.byteAddress2 = byteAddress2;
        this.byteAddress3 = byteAddress3;
        this.byteAddress4 = byteAddress4;

        this.byteContent1 = byteContent1;
        this.byteContent2 = byteContent2;
        this.byteContent3 = byteContent3;
        this.byteContent4 = byteContent4;
    }
}