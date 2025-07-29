using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.EntityFrameworkCore;

namespace AsyncBenchmarks;

/// <summary>
/// Benchmarks testing async benefits under high concurrency load
/// These show when async actually provides value
/// </summary>
[MemoryDiagnoser]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80)]
public class HighConcurrencyBenchmarks
{
    private TestDbContext _context = null!;
    private HttpClient _httpClient = null!;

    [GlobalSetup]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("ConcurrencyTestDb")
            .Options;
        
        _context = new TestDbContext(options);
        
        // Seed more data for concurrency testing
        var customers = Enumerable.Range(1, 10000)
            .Select(i => new Customer { Id = i, Name = $"Customer {i}", Email = $"customer{i}@test.com" })
            .ToList();
        
        _context.Customers.AddRange(customers);
        _context.SaveChanges();

        _httpClient = new HttpClient();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _context.Dispose();
        _httpClient.Dispose();
    }

    [Params(1, 10, 100)]
    public int ConcurrentRequests { get; set; }

    #region Concurrent Database Operations

    [Benchmark]
    public void ConcurrentDatabaseQueriesSync()
    {
        var tasks = Enumerable.Range(1, ConcurrentRequests)
            .Select(i => Task.Run(() => 
            {
                using var context = CreateNewContext();
                return context.Customers.FirstOrDefault(c => c.Id == i % 1000 + 1);
            }))
            .ToArray();
        
        Task.WaitAll(tasks);
    }

    [Benchmark]
    public async Task ConcurrentDatabaseQueriesAsync()
    {
        var tasks = Enumerable.Range(1, ConcurrentRequests)
            .Select(async i => 
            {
                using var context = CreateNewContext();
                return await context.Customers.FirstOrDefaultAsync(c => c.Id == i % 1000 + 1);
            })
            .ToArray();
        
        await Task.WhenAll(tasks);
    }

    #endregion

    #region Simulated I/O Operations

    [Benchmark]
    public void SimulatedIOSync()
    {
        var tasks = Enumerable.Range(1, ConcurrentRequests)
            .Select(i => Task.Run(() => SimulateSlowOperation(50))) // 50ms delay
            .ToArray();
        
        Task.WaitAll(tasks);
    }

    [Benchmark]
    public async Task SimulatedIOAsync()
    {
        var tasks = Enumerable.Range(1, ConcurrentRequests)
            .Select(i => SimulateSlowOperationAsync(50)) // 50ms delay
            .ToArray();
        
        await Task.WhenAll(tasks);
    }

    #endregion

    #region Helper Methods

    private TestDbContext CreateNewContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("ConcurrencyTestDb")
            .Options;
        return new TestDbContext(options);
    }

    private string SimulateSlowOperation(int delayMs)
    {
        Thread.Sleep(delayMs); // Blocking I/O simulation
        return $"Completed after {delayMs}ms";
    }

    private async Task<string> SimulateSlowOperationAsync(int delayMs)
    {
        await Task.Delay(delayMs); // Non-blocking I/O simulation
        return $"Completed after {delayMs}ms";
    }

    #endregion
}
