using DuplicatesRemover.FileProccessing;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DuplicatesCleaner.FileProcessor
{
    public sealed class CommonLogic
    {
        private SemaphoreSlim _semaphore;

        /// <summary>
        /// Inits common logic item
        /// </summary>
        /// <param name="semaphoreThreadCount">semaphore thread count</param>
        public void Init(int semaphoreThreadCount)
        {
            _semaphore = new SemaphoreSlim(semaphoreThreadCount);
        }

        /// <summary>
        /// Executes common library logic
        /// </summary>
        /// <param name="filePaths">input files</param>
        /// <param name="outputPath">output path</param>
        /// <param name="uniqueWordCollection">unique word collection bucket</param>
        /// <param name="token">cancellation token</param>
        /// <returns>Task</returns>
        public async Task Execute(IReadOnlyCollection<string> filePaths, string outputPath, IUniqueWordCollection uniqueWordCollection, CancellationToken token)
        {
            if (_semaphore == null)
            {
                throw new ArgumentNullException("Use Init method for semaphore defining");
            }

            IDuplicateWordCleaner duplicateCleaner = new DuplicateWordCleaner(uniqueWordCollection);
            IFileHandler fileHandler = new FileHandler(duplicateCleaner);

            foreach (var filePath in filePaths)
            {
                await _semaphore.WaitAsync(token).ConfigureAwait(false);
                if (token.IsCancellationRequested)
                {
                    return;
                }

                _ = Task.Run(async () =>
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"Can start threads count: {_semaphore.CurrentCount}");
                        System.Diagnostics.Debug.WriteLine($"Thread start id: {Thread.CurrentThread.ManagedThreadId}");
                        await fileHandler.HandleFile(filePath, outputPath, token);
                    }
                    finally
                    {
                        _semaphore.Release();
                        System.Diagnostics.Debug.WriteLine($"Thread finish id: {Thread.CurrentThread.ManagedThreadId}");
                    }
                });
            }
        }
    }
}