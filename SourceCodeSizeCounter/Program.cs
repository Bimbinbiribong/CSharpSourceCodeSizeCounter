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
            CSharpSourceSizeCounter c = new CSharpSourceSizeCounter(args[0]);
            long filesCountBytes = c.Count();
            Console.WriteLine($"Bytes count: {filesCountBytes}");
            Console.WriteLine($"Kilobytes count: {filesCountBytes / 1024} (truncated)");
        }
    }
}
