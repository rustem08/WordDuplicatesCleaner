using System.Threading;
using System.Threading.Tasks;

namespace DuplicatesRemover.FileProccessing
{
    public interface IDuplicateWordCleaner
    {
        /// <summary>
        /// Removes duplicates from output file and saves result to another file
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <param name="outputPath">output path</param>
        /// <param name="token">cancellation token</param>
        /// <returns>Task</returns>
        Task RemoveDuplicates(string filePath, string outputPath, CancellationToken token);
    }
}