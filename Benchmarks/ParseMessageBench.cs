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
| ParseMessageAndGetValues | Local Net4.8 | 93.15 us | 16.535 us | 0.906 us |  1.86 |    0.01 | 38.5742 | 5.6152 | 237.34 KB |        1.12 |
| ParseMessageAndGetValues | Local Net8   | 40.62 us |  8.698 us | 0.477 us |  0.81 |    0.01 | 11.1084 | 1.7090 | 170.15 KB |        0.81 |
| ParseMessageAndGetValues | Nuget Net4.8 | 94.37 us |  4.109 us | 0.225 us |  1.88 |    0.03 | 44.4336 | 6.8359 | 273.49 KB |        1.30 |
| ParseMessageAndGetValues | Nuget Net8   | 50.08 us | 14.270 us | 0.782 us |  1.00 |    0.00 | 13.7329 | 2.1973 | 211.17 KB |        1.00 |

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