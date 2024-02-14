using Compiler.Assembler;

int baseAddress = 0;
Assembler assembler = new Assembler(baseAddress);
assembler.assembleFile("./AssemblyTests/inst.asm");
