using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DuplicatesCleaner.FileProcessor;

namespace DuplicatesCleaner
{
    internal class Program
    {
        private const string usage = "DuplicatesCleaner.exe [-c maxThreadCount] [-t timeout_ms] [\"inputPath\"]";

        static void Main(string[] args)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                int maxThreadCount;
                int timeoutMilliseconds;
                string[] filePaths;

                (maxThreadCount, timeoutMilliseconds, filePaths) = ParseArgs(args);

                // For local testing
                //filePaths = new string[] { @"c:\TestData\big.txt", @"c:\TestData\sample-2mb-text-file.txt", @"c:\TestData\plrabn12.txt", @"c:\TestData\alice29.txt", @"c:\TestData\asyoulik.txt", @"c:\TestData\lcet10.txt", @"c:\TestData\5.txt", @"c:\TestData\6.txt", @"c:\TestData\7.txt",
                //@"c:\TestData\bib", @"c:\TestData\book1", @"c:\TestData\book2", @"c:\TestData\geo", @"c:\TestData\news", @"c:\TestData\obj1", @"c:\TestData\obj2", @"c:\TestData\paper1", @"c:\TestData\paper2",
                //@"c:\TestData\paper3", @"c:\TestData\paper4", @"c:\TestData\paper5", @"c:\TestData\paper6", @"c:\TestData\pic", @"c:\TestData\progc", @"c:\TestData\progl", @"c:\TestData\progp", @"c:\TestData\trans",};
                //maxThreadCount = 5;

                string filePath = filePaths[0];
                string outputdirName = Path.Combine(Path.GetDirectoryName(filePath), $"output");


                CommonLogic commonLogic = new CommonLogic();
                commonLogic.Init(maxThreadCount);
                IUniqueWordCollection uniqueWord = new UniqueWordCollection();
                using (CancellationTokenSource cancelTokenSource = new CancellationTokenSource(timeoutMilliseconds))
                {
                    try
                    {
                        commonLogic.Execute(filePaths, outputdirName, uniqueWord, cancelTokenSource.Token).Wait();
                    }
                    catch (AggregateException ex)
                    {
                        Console.WriteLine($"{ex.GetType()}: Operation failed. Reason {ex.Message}");
                        foreach (var exception in ex.Flatten().InnerExceptions)
                        {
                            Console.WriteLine($"{exception.GetType()}: {exception.Message}");
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ex.GetType()}: Operation failed. Reason {ex.Message}");
                    }
                }

                // Show statistics
                Console.WriteLine($"\nReplace time elapsec: {stopwatch.ElapsedMilliseconds}ms");

#if DEBUG
                Console.WriteLine("Start write log to file...");

                // Print log to file
                using (StreamWriter logFileStream = new StreamWriter($@".\removeDuplicate.log", false))
                {
                    while (uniqueWord.RemovedDuplicateLog.TryDequeue(out string logLine))
                    {
                        logFileStream.WriteLineAsync(logLine).Wait();
                    }
                }

                Console.WriteLine("End write log to file");
#endif

                Console.WriteLine("{0,-20} {1,5}\n", "Word", "Duplicates count");

                int counter = 1;
                // note: it Long time operation
                foreach (var word in uniqueWord.WordDictionary)
                {
                    Console.WriteLine("{0,-20} {1,5}", word.Key, word.Value);
                    if (counter % 25 == 0)
                    {
                        Console.WriteLine("Press key for next result");
                        Console.ReadKey();
                    }

                    counter++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Parses app args
        /// </summary>
        /// <param name="args">app args</param>
        /// <returns>max thread count (default 5), operationTimeoutand (default 6 sec) and file path list</returns>
        private static (int maxThreadCount, int operationTimeout, string[] filePaths) ParseArgs(string[] args)
        {
            // set default values
            int maxThreadCount = 5;
            int operationTimeout = 6000;

            string[] filePathList = new string[0];
            string path = string.Empty;
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].StartsWith("-c"))
                    {
                        maxThreadCount = args[i].Length > 2 ? int.Parse(args[i].Substring(2)) : int.Parse(args[i++ + 1]);
                        if (maxThreadCount <= 0)
                        {
                            throw new ArgumentException($"Incorrect {nameof(maxThreadCount)}. Value can be more then the 0");
                        }
                    }
                    else if (args[i].StartsWith("-t"))
                    {
                        operationTimeout = args[i].Length > 2 ? int.Parse(args[i].Substring(2)) : int.Parse(args[i++ + 1]);
                        if (operationTimeout <= 0)
                        {
                            throw new ArgumentException($"Incorrect {nameof(operationTimeout)}. Value can be more then the 0");
                        }
                    }
                    else if (string.IsNullOrEmpty(path))
                    {
                        path = args[i].Replace("\"", "");
                        filePathList = Directory.GetFiles(path);
                    }
                }
            }
            catch (Exception ex)
            {
                // Print usage
                Console.WriteLine(ex.Message);
                Console.WriteLine(usage);
                throw new ArgumentException("Incorrect input", ex);
            }

            return (maxThreadCount, operationTimeout, filePathList);
        }
    }
}