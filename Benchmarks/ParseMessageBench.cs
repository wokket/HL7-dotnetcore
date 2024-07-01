using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

using HL7.Dotnetcore;
using System.IO;

namespace Benchmarks
{
    /// <summary>
    /// Basic benchmark comparing the nuget version to the local code, for both .NET Framework 4.8 and .NET 8.0.  
    /// Calls a copy of one of the tests that best reflects the usage scenarios (Parse and GetValue)
    /// </summary>
    [Config(typeof(Config))]
    [MemoryDiagnoser]
    [HideColumns("BuildConfiguration", "NuGetReferences", "Runtime")]
    public class ParseMessageBench
    {

/*
| Method                   | Job          | Mean     | Error     | StdDev   | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
|------------------------- |------------- |---------:|----------:|---------:|------:|--------:|--------:|-------:|----------:|------------:|
| ParseMessageAndGetValues | Local Net4.8 | 74.06 us |  2.817 us | 0.154 us |  1.51 |    0.01 | 32.8369 | 4.6387 | 202.32 KB |        0.96 |
| ParseMessageAndGetValues | Local Net8   | 39.56 us | 28.321 us | 1.552 us |  0.81 |    0.04 |  9.4604 | 1.4038 | 145.75 KB |        0.69 |
| ParseMessageAndGetValues | Nuget Net4.8 | 96.68 us |  6.802 us | 0.373 us |  1.97 |    0.01 | 44.4336 | 6.7139 | 273.47 KB |        1.30 |
| ParseMessageAndGetValues | Nuget Net8   | 49.12 us |  6.488 us | 0.356 us |  1.00 |    0.00 | 13.7329 | 2.3193 | 211.17 KB |        1.00 |
 */

        private readonly string _sampleMessage = File.ReadAllText("Sample-Orm.txt");

        private class Config : ManualConfig
        {
            public Config()
            {
                var baseJob = Job.ShortRun;

                AddJob(baseJob.WithNuGet("HL7-dotnetcore", "2.37.1").WithRuntime(CoreRuntime.Core80).WithId("Nuget Net8").AsBaseline());
                AddJob(baseJob.WithNuGet("HL7-dotnetcore", "2.37.1").WithRuntime(ClrRuntime.Net48).WithId("Nuget Net4.8"));

                AddJob(baseJob.WithRuntime(ClrRuntime.Net48).WithCustomBuildConfiguration("LOCAL_CODE")
                    .WithId("Local Net4.8")); // custom config to include/exclude nuget reference or target project reference locally

                AddJob(baseJob.WithRuntime(CoreRuntime.Core80).WithCustomBuildConfiguration("LOCAL_CODE")
                    .WithId("Local Net8")); // custom config to include/exclude nuget reference or target project reference locally
            }
        }

        [Benchmark]
        public void ParseMessageAndGetValues()
        {
            var msg = new Message(_sampleMessage);
            msg.ParseMessage(true);

            var ack = msg.GetACK(true);
            string sendingApp = ack.GetValue("MSH.3");
            string sendingFacility = ack.GetValue("MSH.4");
            string receivingApp = ack.GetValue("MSH.5");
            string receivingFacility = ack.GetValue("MSH.6");
            string messageType = ack.GetValue("MSH.9");
        }
    }
}
