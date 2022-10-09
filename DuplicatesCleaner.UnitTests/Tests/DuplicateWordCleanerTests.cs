using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;

namespace DuplicatesCleaner.FileProcessor.Tests
{
    [TestClass()]
    public class DuplicateWordCleanerTests
    {
        private const int timeoutMilliseconds = 6000;

        /// <summary>
        /// Checks removing duplicate from input file
        /// </summary>
        [TestMethod()]
        public void RemoveDuplicatesTest()
        {
            string filePath = @".\TestFiles\1.txt";
            string outputdirName = Path.Combine(Path.GetDirectoryName(filePath), $"output{DateTime.Now.TimeOfDay.TotalMilliseconds}");

            IUniqueWordCollection uniqueWord = new UniqueWordCollection();

            DuplicateWordCleaner dupicateCleaner = new DuplicateWordCleaner(uniqueWord);
            using (CancellationTokenSource cancelTokenSource = new CancellationTokenSource(timeoutMilliseconds))
            {
                dupicateCleaner.RemoveDuplicates(filePath, outputdirName, cancelTokenSource.Token).Wait();
            }

            Assert.AreEqual(4, uniqueWord.WordDictionary.Count);
            Assert.AreEqual(5, uniqueWord.WordDictionary["gfh"]);
            Assert.AreEqual(6, uniqueWord.WordDictionary["asd"]);
            Assert.AreEqual(6, uniqueWord.WordDictionary["qwerty"]);
            Assert.AreEqual(0, uniqueWord.WordDictionary["gfh2"]);

            string outputExpexted = @"qwerty ,, asd !gfh
 ,,  !
 ,,  !
 ,,  !
 ,,  !
 ,,  !
 ,,  !gfh2￿";

            using (FileStream inputFileStream = File.Open(@".\TestFiles\output\1.txt", FileMode.Open))
            using (BufferedStream inputBufferStream = new BufferedStream(inputFileStream))
            using (StreamReader streamReader = new StreamReader(inputBufferStream))
            {
                var outputFile = streamReader.ReadToEnd();
                Assert.AreEqual(outputExpexted, outputFile);
            }
        }
    }
}