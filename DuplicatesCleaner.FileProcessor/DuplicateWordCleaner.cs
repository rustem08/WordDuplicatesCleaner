using DuplicatesRemover.FileProccessing;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DuplicatesCleaner.FileProcessor
{
    /// <summary>
    /// Presents a class for saving words without duplicates from input file to output file
    /// </summary>
    public sealed class DuplicateWordCleaner : IDuplicateWordCleaner
    {
        private readonly IUniqueWordCollection _uniqueWordCollection;

        /// <summary>
        /// Creates an instance of DuplicateWordCleaner
        /// </summary>
        /// <param name="uniqueWordCollection">unique word collection instance</param>
        public DuplicateWordCleaner(IUniqueWordCollection uniqueWordCollection)
        {
            _uniqueWordCollection = uniqueWordCollection;
        }

        /// <inheritdoc/>
        public async Task RemoveDuplicates(string filePath, string outputPath, CancellationToken token)
        {
            try
            {
                Directory.CreateDirectory(outputPath);
                string fileName = Path.GetFileName(filePath);

                // input streams
                using (FileStream inputFileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BufferedStream inputBufferStream = new BufferedStream(inputFileStream))
                using (StreamReader streamReader = new StreamReader(inputBufferStream))

                // output streams
                using (StreamWriter outputFileStream = new StreamWriter(Path.Combine(outputPath, fileName)))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    while (!token.IsCancellationRequested)
                    {
                        // Read character one by one
                        char c = (char)streamReader.Read();

                        if (char.IsLetterOrDigit(c))
                        {
                            // Make word from characters
                            stringBuilder.Append(c);
                        }
                        else
                        {
                            // Handle madden word
                            string currentWord = stringBuilder.ToString();
                            stringBuilder.Clear();

                            bool isUniqueWord = _uniqueWordCollection.TryAdd(currentWord);

                            if (isUniqueWord && !string.IsNullOrEmpty(currentWord))
                            {
                                await outputFileStream.WriteAsync(currentWord).ConfigureAwait(false);
                            }

                            // Write delimiter
                            await outputFileStream.WriteAsync(c).ConfigureAwait(false);

                            // We want check last word, therefor this streamReader.EndOfStream statement 
                            // checks this place
                            if (streamReader.EndOfStream)
                            {
                                break;
                            }
                        }
                    }

                    outputFileStream.Close();
                    inputBufferStream.Close();
                    streamReader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}