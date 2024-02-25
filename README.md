# What is this?
This is a simple emulator for a RISC-like assembly language. 

# Dependencies
 # Language:
   - C#
 ## Frameworks:
   - Microsoft.NETCore.App (v. 8.0.0)
     
 ## Packages(These are used in the test suite)
   - coverlet.collector (v. 6.0.0)
   - Microsoft.NET.Test.Sdk (v. 17.6.0)
   - NUnit (v. 3.13.3)
   - NUnit.Analyzers (v. 3.6.1)
   - NUnit3TestAdapter (v. 4.2.1)
     
# How to install and run this project
1. Clone this repo
2. Make sure you have the correct dependencies installed
3. Navigate to [Program.cs](https://github.com/lar9482/AssemblySimulator/blob/main/Runtime/Program.cs) and change the paths `assemblyFilePath` and `binFilePath`.
   Make sure to save the file
   ```
     NOTE: 
     assemblyFilePath points to the file of your source program.
     binFilePath points to the file of the binary.
     The file placed in `binFilePath` is generated automatically by the assembler. It DOES NOT have to exist yet.

     assemblyFilePath and binFilePath will be relative to the root of the project.
     For example, to run sampleProgram.asm, change assemblyFilePath to './sampleProgram.asm' and binFilePath to './sampleProgram.out'
   ```
4. Run the command `dotnet build`
5. Run the command `dotnet run --project './Runtime/Runtime.csproj`

# Example Program(sampleProgram.asm)
```
loopBody:

    printw_int r1
    addI r1, 1
    bLt r1, r2, loopBody
    jmp exit

main:
    movI r1, 0
    movI r2, 100

    jmp loopBody

exit:
```
