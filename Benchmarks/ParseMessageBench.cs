using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using HL7.Dotnetcore;
using System.IO;

namespace Benchmarks
{
    /// <summary>
    /// Basic benchmark comparing the nuget version to our local code, for both Framework 4.8 and net8.  Calls a copy of one of the tests
    /// that best reflects my usage scenarios (Parse and GetValue)
    /// </summary>
    [Config(typeof(Config))]
    [MemoryDiagnoser]
    [HideColumns("BuildConfiguration", "NuGetReferences", "Runtime")]
    public class ParseMessageBench
    {
/*

| Method                   | Job          | Mean     | Error     | StdDev   | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
|------------------------- |------------- |---------:|----------:|---------:|------:|--------:|--------:|-------:|----------:|------------:|
| ParseMessageAndGetValues | Local Net4.8 | 76.92 us |  2.737 us | 0.150 us |  1.57 |    0.03 | 34.6680 | 5.0049 | 213.52 KB |        1.01 |
| ParseMessageAndGetValues | Local Net8   | 33.03 us |  1.697 us | 0.093 us |  0.67 |    0.01 |  9.7046 | 1.5259 | 149.35 KB |        0.71 |
| ParseMessageAndGetValues | Nuget Net4.8 | 93.29 us |  4.798 us | 0.263 us |  1.90 |    0.03 | 44.4336 | 6.8359 | 273.48 KB |        1.30 |
| ParseMessageAndGetValues | Nuget Net8   | 49.06 us | 15.435 us | 0.846 us |  1.00 |    0.00 | 13.7329 | 2.2583 | 211.17 KB |        1.00 |

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