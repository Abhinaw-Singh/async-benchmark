using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace AsyncBenchmarks;

[MemoryDiagnoser]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80)]
public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<AsyncVsSyncBenchmarks>();
        Console.WriteLine("Benchmark completed. Check the results above.");
    }
}
