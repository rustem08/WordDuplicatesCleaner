using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DuplicatesCleaner.FileProcessor.Tests
{
    [TestClass()]
    public class UniqueWordCollectionTests
    {
        /// <summary>
        /// Checks unique and duplicates word adding
        /// </summary>
        [TestMethod()]
        public void TryAddTest()
        {
            IUniqueWordCollection uniqueWord = new UniqueWordCollection();

            bool result = uniqueWord.TryAdd("1");
            Assert.IsTrue(result);
            result = uniqueWord.TryAdd("1");
            Assert.IsFalse(result);
            result = uniqueWord.TryAdd("2");
            Assert.IsTrue(result);
           
            foreach (var word in uniqueWord.WordDictionary)
            {
                System.Diagnostics.Debug.WriteLine(word);
            }
        }
    }
}