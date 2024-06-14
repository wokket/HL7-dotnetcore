using BenchmarkDotNet.Attributes;
using HL7.Dotnetcore;
using System.IO;

namespace Benchmarks
{
    [SimpleJob]
    [MemoryDiagnoser]
    public class ParseOrmBench
    {
        /*
// Before starting to multi-target
| Method   | Runtime            | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
|--------- |------------------- |---------:|---------:|---------:|------:|--------:|--------:|-------:|----------:|------------:|
| ParseOrm | .NET Framework 4.8 | 92.11 us | 1.173 us | 1.040 us |  0.97 |    0.02 | 44.5557 | 8.6670 | 274.38 KB |        1.00 |
| ParseOrm | .NET 8.0           | 49.60 us | 0.260 us | 0.203 us |  0.52 |    0.01 | 13.6108 | 2.6245 | 208.53 KB |        0.76 |


// After upgrading SegmentRegex
         
         */
        

        private string ORM;

        [GlobalSetup]
        public void Setup()
        {
            ORM = File.ReadAllText("Sample-ORM.txt");
        }
    
    
        [Benchmark]
        public void ParseOrm()
        {
            var message = new Message(ORM);
            var result = message.ParseMessage(false);
        }
    }
}