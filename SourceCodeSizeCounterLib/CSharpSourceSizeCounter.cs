using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SourceCodeSizeCounterLib
{
    /// <summary>
    /// Instance of this class' purpose is to count C sharp source files sizes.
    /// </summary>
    public class CSharpSourceSizeCounter
    {
        readonly DirectoryInfo sourceDirectory;

        /// <summary>
        ///     Constructs an instance of object counting c sharp source files size.
        /// </summary>
        /// <param name="path">Path for source directory from which starts recursive file size counting</param>
        public CSharpSourceSizeCounter(string path)
        {
            sourceDirectory = new DirectoryInfo(path);
        }

        /// <summary>
        ///     Asynchronously counts size of C sharp source files and returns its sum in bytes
        /// </summary>
        /// <returns></returns>
        public Task<long> CountAsync()
        {
            return Task.Run(() => CountAsync(sourceDirectory));
        }

        async Task<long> CountAsync(DirectoryInfo dir)
        {
            long sourceLengths = 0;

            var subDirectories = dir.GetDirectories(); // get sub-directories
            var tasks = new Task<long>[subDirectories.Length];

            for (int i = 0; i < tasks.Length; i++)
                tasks[i] = CountAsync(subDirectories[i]); // count files in its subdirectories

            var files = dir.GetFiles();

            Regex designerRegex = new Regex(@"\.Designer\.cs$");
            Regex csRegex = new Regex(@"\.cs$");

            var filteredFiles = from file in files.AsParallel()
                where !designerRegex.IsMatch(file.Name) // filters out .Designer.cs files
                where csRegex.IsMatch(file.Name) // takes .cs files
                where !file.Name.StartsWith("TemporaryGeneratedFile_") // filters out generated files
                where file.Name != "AssemblyInfo.cs"
                select file;

            sourceLengths += (from file in filteredFiles select file.Length).DefaultIfEmpty().Sum(); // counts Size


            await Task.WhenAll(tasks);
            sourceLengths += (from task in tasks select task.Result).Sum();

            return sourceLengths;
        }
    }
}