﻿using Compiler.Assembler;
using Compiler.Machine;

int startProgramAddress = 0;
string assemblyFilePath = "./AssemblyTests/immInst.asm";
string binFilePath = "./AssemblyTests/Output/immInst.out";

Assembler assembler = new Assembler(startProgramAddress);
assembler.assembleFile(assemblyFilePath, binFilePath);

Machine machine = new Machine(startProgramAddress);
machine.loadProgram(binFilePath);
Console.WriteLine();