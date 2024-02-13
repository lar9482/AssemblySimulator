using Compiler.Assembler;

int baseAddress = 1000;
Assembler assembler = new Assembler(baseAddress);
assembler.assembleFile("./AssemblyTests/inst.asm");
