using DuplicatesRemover.FileProccessing;
using System.Threading;
using System.Threading.Tasks;

namespace DuplicatesCleaner.FileProcessor
{
    /// <summary>
    /// Presents a FileHandler
    /// </summary>
    public sealed class FileHandler : IFileHandler
    {
        private readonly IDuplicateWordCleaner _duplicateCleaner;

        /// <summary>
        /// Creates an instance of FileHandler 
        /// </summary>
        /// <param name="duplicateCleaner">duplicate cleaner</param>
        public FileHandler(IDuplicateWordCleaner duplicateCleaner)
        {
            _duplicateCleaner = duplicateCleaner;
        }

        /// <inheritdoc/>
        public Task HandleFile(string filePath, string outputPath, CancellationToken token)
        {
            return _duplicateCleaner.RemoveDuplicates(filePath, outputPath, token);
        }
    }
}