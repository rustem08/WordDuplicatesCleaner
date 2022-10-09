using System;
using System.Collections.Concurrent;
using System.Threading;

namespace DuplicatesCleaner.FileProcessor
{
    /// <summary>
    /// Presents a thread safe class for creating unique case sensitive word collection
    /// Note: "word" and "Word" responds as different unique words
    /// </summary>
    public sealed class UniqueWordCollection : IUniqueWordCollection
    {
        /// <summary>
        /// Unique word collection. Values of collection indicates the numbers of the word duplicates
        /// </summary>
        private readonly ConcurrentDictionary<string, ushort> _wordDict = new ConcurrentDictionary<string, ushort>();

#if DEBUG
        /// <summary>
        /// Log string for debug purpose only
        /// </summary>
        private readonly ConcurrentQueue<string> _removedWordLogger = new ConcurrentQueue<string>();
#endif

        /// <inheritdoc/>
        public bool TryAdd(in string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }

            bool isUnique = 0 == _wordDict.AddOrUpdate(word, 0, (key, oldValue) => (ushort)(oldValue + (ushort)1));
            if (!isUnique)
            {
#if DEBUG
                _removedWordLogger.Enqueue($"[{DateTime.Now.TimeOfDay}] '{word}' removed by thread {Thread.CurrentThread.ManagedThreadId}");
#endif
            }

            return isUnique;
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ushort> WordDictionary => _wordDict;


#if DEBUG
        /// <inheritdoc/>
        public ConcurrentQueue<string> RemovedDuplicateLog => _removedWordLogger;
#endif
    }
}