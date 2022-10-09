using System.Threading.Tasks;

namespace DuplicatesCleaner.FileProcessor
{
    public interface IFileHandler
    {
        /// <summary>
        /// Gets file handler task
        /// </summary>
        /// <param name="filePath">input file</param>
        /// <param name="outputPath">output path</param>
        /// <param name="token">cancellation token</param>
        /// <returns>file handler task</returns>
        Task HandleFile(string filePath, string outputPath, System.Threading.CancellationToken token);
    }
}