﻿using Compiler.Assembler;
using Compiler.Machine;

int startProgramAddress = 0;
string assemblyFilePath = "./AssemblyTests/regInst.asm";
string binFilePath = "./AssemblyTests/Output/regInst.out";

Assembler assembler = new Assembler(startProgramAddress);
assembler.assembleFile(assemblyFilePath, binFilePath);

Machine machine = new Machine(startProgramAddress);
machine.loadProgram(binFilePath);
machine.runProgram();
Console.WriteLine();