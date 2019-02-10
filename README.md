# CSharpSourceCodeSizeCounter
Console Application which accepts name of source directory in first parameter (args[0]) and writes C# source code files written by human (not generated) size on the output (optionally number of lines). Searches recursively.

Input:
  - first parameter: non-escaped path from which you want to start recursive count
  - second (optional) parameter: -l (line counting)

Output:
  - size of C# source files in bytes and kilobytes (truncated)
  - (optional) number of lines in the sources
  
In this project C# source file written by human is defined as C# source file whose name isn't "AssemblyInfo.cs" or "Reference.cs", doesn't end with ".Designer.cs" or ".Generated.cs", doesn't start with "TemporaryGeneratedFile_".
Every other file ending with ".cs" is defined as C# source file.

## Example
Input:
```
./SourceCodeSizeCounter.exe ".\SolutionDirectory" -l
```

Output:
```
Bytes count: 852858
Kilobytes count: 832 (truncated)
Number of lines: 24873
```
