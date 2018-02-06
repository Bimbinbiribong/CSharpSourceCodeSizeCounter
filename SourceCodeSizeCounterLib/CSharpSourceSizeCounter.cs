using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SourceCodeSizeCounterLib
{
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    ///     Instance of this class' purpose is to count C sharp source files sizes.
    /// </summary>
    public class CSharpSourceSizeCounter
    {
        readonly DirectoryInfo sourceDirectory;
        static readonly Regex designerRegex = new Regex(@"\.Designer\.cs$", RegexOptions.Compiled);
        static readonly Regex generatedRegex = new Regex(@"\.Generated\.cs$", RegexOptions.Compiled);
        static readonly Regex csRegex = new Regex(@"\.cs$", RegexOptions.Compiled);

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
            return Task.Run(() => Count(sourceDirectory));
        }

        /// <summary>
        ///     Counts size of C sharp source files and returns its sum in bytes.
        /// </summary>
        /// <returns></returns>
        public long Count()
        {
            return Count(sourceDirectory);
        }

        long Count(DirectoryInfo dir)
        {
            long sourceLengths = 0;

            DirectoryInfo[] subDirectories = null;
            try
            {
                subDirectories = dir.GetDirectories(); // get sub-directories
            }
            catch (UnauthorizedAccessException)
            {
                return 0;
            }

            foreach (DirectoryInfo directory in subDirectories)
                sourceLengths += Count(directory); // count files in its subdirectories

            var files = dir.GetFiles();

            var filteredFiles = from file in files.AsParallel()
                                where csRegex.IsMatch(file.Name) // takes .cs files
                                where !designerRegex.IsMatch(file.Name) // filters out .Designer.cs files
                                where !generatedRegex.IsMatch(file.Name) // filters out .Generated.cs
                                where !file.Name.StartsWith("TemporaryGeneratedFile_") // filters out generated files
                                where file.Name != "AssemblyInfo.cs"
                                where file.Name != "Reference.cs"
                                select file;

            sourceLengths += (from file in filteredFiles select file.Length).DefaultIfEmpty().Sum(); // counts Size

            return sourceLengths;
        }

        /// <summary>
        /// Asynchronously counts lines of C# source files (non-generated) and returns it.
        /// </summary>
        /// <returns></returns>
        public async Task<long> GetLinesCount()
        {
            return await GetLinesCount(sourceDirectory);
        }

        private async Task<long> GetLinesCount(DirectoryInfo dir)
        {
            Debug.WriteLine($"Recursed to directory {dir.Name}");

            long linesCount = 0;

            DirectoryInfo[] subDirectories;
            try
            {
                subDirectories = dir.GetDirectories(); // get sub-directories
            }
            catch (UnauthorizedAccessException)
            {
                // return 0 if cannot access
                var tcs = new TaskCompletionSource<long>();
                tcs.SetResult(0);
                return await tcs.Task;
            }

            Task<long>[] subdirectoriesCountingTasks = new Task<long>[subDirectories.Length];
            for (int i = 0; i < subdirectoriesCountingTasks.Length; i++)
            {
                subdirectoriesCountingTasks[i] = GetLinesCount(subDirectories[i]);
            }

            // task ending when every subdirectory is searched
            var subDirectoriesLinesTasks = Task.WhenAll(subdirectoriesCountingTasks);

            var files = dir.GetFiles();

            var filteredFiles = from file in files.AsParallel()
                                where csRegex.IsMatch(file.Name) // takes .cs files
                                where !designerRegex.IsMatch(file.Name) // filters out .Designer.cs files
                                where !generatedRegex.IsMatch(file.Name) // filters out .Generated.cs
                                where !file.Name.StartsWith("TemporaryGeneratedFile_") // filters out generated files
                                where file.Name != "AssemblyInfo.cs"
                                where file.Name != "Reference.cs"
                                select file;

            // parallely count lines in those files
            var filesForEach = Parallel.ForEach(filteredFiles, file =>
            {
                Debug.WriteLine($"{file.Name} reading started");
                using (StreamReader reader = new StreamReader(file.Open(FileMode.Open)))
                {
                    while (reader.ReadLine() != null)
                    {
                        Interlocked.Increment(ref linesCount);
                    }
                }
                Debug.WriteLine($"{file.Name} reading finished");
            });
            
            return linesCount + (await subDirectoriesLinesTasks).Sum();
        }
    }
}