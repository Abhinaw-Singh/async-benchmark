# Async vs Sync Performance Benchmarks

This project contains benchmarks to support the blog post "Why Async Can Be Slower in Real Projects?".

## Running the Benchmarks

1. **Install .NET 8 SDK** (if not already installed)
2. **Navigate to the project directory:**
   ```powershell
   cd why-async-can-be-slow
   ```

3. **Restore packages:**
   ```powershell
   dotnet restore
   ```

4. **Run the benchmarks:**
   ```powershell
   dotnet run -c Release
   ```

## What the Benchmarks Test

### 1. Database Query Benchmarks
- **GetCustomerSync**: Traditional synchronous EF Core query
- **GetCustomerAsync**: Async EF Core query using `FirstOrDefaultAsync`
- **GetCustomerValueTask**: Same async query but returning `ValueTask<T>`

### 2. CPU-bound Calculation Benchmarks
- **CalculateTotalSync**: Direct synchronous calculation
- **CalculateTotalAsync**: Wrapping sync calculation in `Task.Run` (anti-pattern)
- **CalculateTotalAsyncDirect**: Adding async overhead to sync work

### 3. Simple Method Call Benchmarks
- **ProcessDataSync**: Simple synchronous string processing
- **ProcessDataAsync**: Same logic with unnecessary async
- **ProcessDataValueTask**: Using `ValueTask` for immediate results

## Expected Results

Based on the blog post's claims, you should see:

1. **Sync database queries** performing better for simple operations
2. **Async overhead** being significant for CPU-bound work
3. **ValueTask** showing better performance than Task when results are immediate
4. **Memory allocations** being higher for async operations

## Sample Output

```
| Method                    | Mean         | Error        | StdDev       | Median       | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
|-------------------------- |-------------:|-------------:|-------------:|-------------:|------:|--------:|--------:|-------:|----------:|------------:|
| GetCustomerSync           | 41,511.54 ns | 1,716.858 ns | 5,062.194 ns | 40,485.35 ns | 1.000 |    0.00 | 12.2070 | 1.9531 |   77456 B |       1.000 |
| GetCustomerAsync          | 42,714.36 ns | 1,760.907 ns | 5,192.073 ns | 43,167.24 ns | 1.050 |    0.21 | 12.2070 | 1.9531 |   77608 B |       1.002 |
| GetCustomerValueTask      | 57,879.68 ns | 3,368.943 ns | 9,773.914 ns | 62,016.53 ns | 1.423 |    0.30 | 12.2070 | 1.9531 |   77656 B |       1.003 |
| CalculateTotalSync        | 19,316.87 ns |   385.430 ns | 1,080.788 ns | 19,410.39 ns | 0.480 |    0.06 |       - |      - |      40 B |       0.001 |
| CalculateTotalAsync       | 30,841.26 ns | 1,079.088 ns | 3,164.777 ns | 30,549.47 ns | 0.755 |    0.12 |  0.0610 |      - |     432 B |       0.006 |
| CalculateTotalAsyncDirect | 37,708.00 ns | 1,055.228 ns | 3,061.408 ns | 36,770.64 ns | 0.924 |    0.12 |       - |      - |     206 B |       0.003 |
| ProcessDataSync           |     19.95 ns |     0.806 ns |     2.300 ns |     18.72 ns | 0.000 |    0.00 |  0.0127 |      - |      80 B |       0.001 |
| ProcessDataAsync          |  1,435.27 ns |    27.026 ns |    69.762 ns |  1,406.69 ns | 0.036 |    0.00 |  0.0439 |      - |     279 B |       0.004 |
| ProcessDataValueTask      |     32.70 ns |     0.568 ns |     0.532 ns |     32.63 ns | 0.001 |    0.00 |  0.0127 |      - |      80 B |       0.001 |
```

*(Actual results will vary based on your hardware)*


## Notes

- Run in **Release mode** for accurate performance measurements
- The in-memory database simulates Entity Framework overhead without actual I/O
- Results may vary significantly between different machines and .NET versions
- These benchmarks focus on overhead, not scalability under load


This benchmark is part of a blog post on [Why Async Can Be Slower in Real Projects?](https://bytecrafted.dev/csharp-async-await-scalability-vs-speed).


