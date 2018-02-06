using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceCodeSizeCounterLib;

namespace SourceCodeSizeCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = args[0];

            CSharpSourceSizeCounter c = new CSharpSourceSizeCounter(path);
            long filesCountBytes = c.Count();
            Console.WriteLine($"Bytes count: {filesCountBytes}");
            Console.WriteLine($"Kilobytes count: {filesCountBytes / 1024} (truncated)");

            if (args.Length > 1 && args[1] == "-l")
            {
                long linesCount = c.GetLinesCount().Result;
                Console.WriteLine($"Number of lines: {linesCount}");
            }
        }
    }
}
