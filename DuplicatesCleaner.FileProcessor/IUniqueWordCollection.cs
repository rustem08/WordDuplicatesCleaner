using System.Collections.Concurrent;

namespace DuplicatesCleaner.FileProcessor
{
    public interface IUniqueWordCollection
    {
        /// <summary>
        /// Returns unique word collection. Values of collection indicates the numbers of the word duplicates
        /// </summary>
        ConcurrentDictionary<string, ushort> WordDictionary { get; }

        /// <summary>
        /// Tries to add unique word to dictionary. When word is duplicated the dictionary value increased by one.
        /// </summary>
        /// <param name="word">word</param>
        /// <returns>true for unique word</returns>
        bool TryAdd(in string word);

#if DEBUG
        /// <summary>
        /// Returns unique word collection. Values of collection indicates the numbers of the word duplicates
        /// </summary>
        ConcurrentQueue<string> RemovedDuplicateLog { get; }
#endif
    }
}
