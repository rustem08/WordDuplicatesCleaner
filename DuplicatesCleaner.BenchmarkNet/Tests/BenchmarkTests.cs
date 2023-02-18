using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DuplicatesCleaner.FileProcessor;

namespace DuplicatesCleaner.Benchmark.Tests
{
    [SimpleJob(RuntimeMoniker.Net48, baseline: true)]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Mono60)]
    [RPlotExporter]
    public class BenchmarkTests
    {
        private const int timeoutMilliseconds = 6000;
        private readonly List<string> _filePathList = new List<string> { @".\TestFiles\1.txt", @".\TestFiles\2.txt", @".\TestFiles\3.txt", @".\TestFiles\4.txt", @".\TestFiles\5.txt" };

        [Benchmark]
        public async Task DuplicateWordCleanerTests()
        {
            string filePath = @".\TestFiles\1.txt";
            string outputdirName = Path.Combine(Path.GetDirectoryName(filePath), $"output{Guid.NewGuid().ToString("N")}");

            IUniqueWordCollection uniqueWord = new UniqueWordCollection();
            DuplicateWordCleaner dupicateCleaner = new DuplicateWordCleaner(uniqueWord);
            using (CancellationTokenSource cancelTokenSource = new CancellationTokenSource(timeoutMilliseconds))
            {
                await dupicateCleaner.RemoveDuplicates(filePath, outputdirName, cancelTokenSource.Token);
            }

            uniqueWord.WordDictionary.Clear();
        }

        [Benchmark]
        public void UniqueWordCollectionTryAddTests()
        {
            IUniqueWordCollection uniqueWord = new UniqueWordCollection();
            uniqueWord.TryAdd("1");
            uniqueWord.TryAdd("1");
            uniqueWord.TryAdd("2");
        }
        
        [Benchmark]
        public void FileProcessingExecuteSingleThreadTests()
        {
            string filePath = _filePathList[0];
            string outputdirName = Path.Combine(Path.GetDirectoryName(filePath), $"output{Guid.NewGuid().ToString("N")}");

            IUniqueWordCollection uniqueWord = new UniqueWordCollection();
            CommonLogic commonLogic = new CommonLogic();
            commonLogic.Init(1);
            using (CancellationTokenSource cancelTokenSource = new CancellationTokenSource(timeoutMilliseconds))
            {
                commonLogic.Execute(_filePathList, outputdirName, uniqueWord, cancelTokenSource.Token).Wait();
            }

            uniqueWord.WordDictionary.Clear();
        }

        [Benchmark]
        public void FileProcessingExecuteSingleMultiThreadTests()
        {
            string filePath = _filePathList[0];
            string outputdirName = Path.Combine(Path.GetDirectoryName(filePath), $"output{Guid.NewGuid().ToString("N")}");

            IUniqueWordCollection uniqueWord = new UniqueWordCollection();
            CommonLogic commonLogic = new CommonLogic();
            commonLogic.Init(3);
            using (CancellationTokenSource cancelTokenSource = new CancellationTokenSource(timeoutMilliseconds))
            {
                commonLogic.Execute(_filePathList, outputdirName, uniqueWord, cancelTokenSource.Token).Wait();
            }

            uniqueWord.WordDictionary.Clear();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}