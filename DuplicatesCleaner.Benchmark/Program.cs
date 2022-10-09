using BenchmarkDotNet.Running;
using DuplicatesCleaner.Benchmark.Tests;

namespace DuplicatesCleaner.Benchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BenchmarkTests>();
        }
    }
}