using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;

namespace AsyncBenchmarks;

/// <summary>
/// Benchmarks comparing async vs sync performance for different scenarios
/// mentioned in the blog post "Why Async Can Be Slower in Real Projects?"
/// </summary>
[MemoryDiagnoser]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80)]
public class AsyncVsSyncBenchmarks
{
    private TestDbContext _context = null!;
    private List<OrderItem> _orderItems = null!;

    [GlobalSetup]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        
        _context = new TestDbContext(options);
        
        // Seed test data
        var customers = Enumerable.Range(1, 1000)
            .Select(i => new Customer { Id = i, Name = $"Customer {i}", Email = $"customer{i}@test.com" })
            .ToList();
        
        _context.Customers.AddRange(customers);
        _context.SaveChanges();

        // Setup order items for calculation benchmarks
        _orderItems = Enumerable.Range(1, 1000)
            .Select(i => new OrderItem { Price = i * 10.50m, Quantity = i % 10 + 1 })
            .ToList();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    #region Database Query Benchmarks

    [Benchmark(Baseline = true)]
    public Customer GetCustomerSync()
    {
        return _context.Customers.FirstOrDefault(c => c.Id == 500)!;
    }

    [Benchmark]
    public async Task<Customer> GetCustomerAsync()
    {
        return (await _context.Customers.FirstOrDefaultAsync(c => c.Id == 500))!;
    }

    [Benchmark]
    public async ValueTask<Customer> GetCustomerValueTask()
    {
        return (await _context.Customers.FirstOrDefaultAsync(c => c.Id == 500))!;
    }

    #endregion

    #region CPU-bound Calculation Benchmarks

    [Benchmark]
    public decimal CalculateTotalSync()
    {
        return _orderItems.Sum(x => x.Price * x.Quantity);
    }

    [Benchmark]
    public async Task<decimal> CalculateTotalAsync()
    {
        return await Task.Run(() => _orderItems.Sum(x => x.Price * x.Quantity));
    }

    [Benchmark]
    public async Task<decimal> CalculateTotalAsyncDirect()
    {
        // This is what happens when you wrap sync code in async unnecessarily
        await Task.Yield();
        return _orderItems.Sum(x => x.Price * x.Quantity);
    }

    #endregion

    #region Simple Method Call Benchmarks

    [Benchmark]
    public string ProcessDataSync()
    {
        var data = GetSomeData();
        return data.ToUpper();
    }

    [Benchmark]
    public async Task<string> ProcessDataAsync()
    {
        var data = await GetSomeDataAsync();
        return data.ToUpper();
    }

    [Benchmark]
    public async ValueTask<string> ProcessDataValueTask()
    {
        var data = await GetSomeDataValueTask();
        return data.ToUpper();
    }

    #endregion

    #region Helper Methods

    private string GetSomeData()
    {
        return "Some test data for processing";
    }

    private async Task<string> GetSomeDataAsync()
    {
        await Task.Yield(); // Simulate minimal async work
        return "Some test data for processing";
    }

    private ValueTask<string> GetSomeDataValueTask()
    {
        // Often the data is already available, so we can return synchronously
        return new ValueTask<string>("Some test data for processing");
    }

    #endregion
}

// Test models
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class OrderItem
{
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    
    public DbSet<Customer> Customers { get; set; } = null!;
}
