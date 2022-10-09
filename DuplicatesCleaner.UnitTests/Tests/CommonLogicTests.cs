using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;

namespace DuplicatesCleaner.FileProcessor.Tests
{
    [TestClass()]
    public class CommonLogicTests
    {
        private const int timeoutMilliseconds = 6000;

        /// <summary>
        /// Checks common logic removes duplicates
        /// </summary>
        [TestMethod()]
        public void ExecuteTest()
        {
            string filePath = @".\TestFiles\1.txt";
            string outputdirName = Path.Combine(Path.GetDirectoryName(filePath), $"output{DateTime.Now.TimeOfDay.TotalMilliseconds}");

            IUniqueWordCollection uniqueWord = new UniqueWordCollection();
            CommonLogic commonLogic = new CommonLogic();
            commonLogic.Init(5);
            using (CancellationTokenSource cancelTokenSource = new CancellationTokenSource(timeoutMilliseconds))
            {
                commonLogic.Execute(new string[] { filePath, @".\TestFiles\2.txt" }, outputdirName, uniqueWord, cancelTokenSource.Token).GetAwaiter().GetResult();
            }

            string outputExpexted1 = @"qwerty ,, asd !gfh
 ,,  !
 ,,  !
 ,,  !
 ,,  !
 ,,  !
 ,,  !gfh2￿";

            string outputExpexted2 = @"1qwerty ,,  !￿";

            using (FileStream inputFileStream = File.Open(@".\TestFiles\output\1.txt", FileMode.Open))
            using (BufferedStream inputBufferStream = new BufferedStream(inputFileStream))
            using (StreamReader streamReader = new StreamReader(inputBufferStream))
            {
                var outputFile = streamReader.ReadToEnd();
                Assert.AreEqual(outputExpexted1, outputFile);
            }

            using (FileStream inputFileStream = File.Open(@".\TestFiles\output\2.txt", FileMode.Open))
            using (BufferedStream inputBufferStream = new BufferedStream(inputFileStream))
            using (StreamReader streamReader = new StreamReader(inputBufferStream))
            {
                var outputFile = streamReader.ReadToEnd();
                Assert.AreEqual(outputExpexted2, outputFile);
            }
        }
    }
}