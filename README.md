# CSharpSourceCodeSizeCounter
Console Application which accepts name of source directory in first parameter (args[0]) and writes C# source code files written by human (not generated) size on the output. Searches recursively.

Input:
  - first parameter: non-escaped path from which you want to start recursive count

Output:
  - size of C# source files in bytes and kilobytes (truncated)
  
In this project C# source file written by human is defined as C# source file whose name isn't "AssemblyInfo.cs", doesn't end with ".Designer.cs" and doesn't start with "TemporaryGeneratedFile_". Every other file ending with ".cs" is defined as C# source file.
