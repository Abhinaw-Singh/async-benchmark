```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.26100.4652)
13th Gen Intel Core i5-1345U, 1 CPU, 12 logical and 10 physical cores
.NET SDK 9.0.203
  [Host]   : .NET 8.0.18 (8.0.1825.31117), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.18 (8.0.1825.31117), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

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
