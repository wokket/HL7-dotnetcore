using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using HL7.Dotnetcore;

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
// After Segment Regex
| Method       | Job          | Mean     | Error     | StdDev   | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
|------------- |------------- |---------:|----------:|---------:|------:|--------:|--------:|-------:|----------:|------------:|
| ParseMessage | Local Net4.8 | 53.76 us | 58.786 us | 3.222 us |  1.91 |    0.12 | 23.6816 | 2.0142 | 145.68 KB |        1.27 |
| ParseMessage | Local Net8   | 24.78 us |  7.183 us | 0.394 us |  0.88 |    0.01 |  6.5918 | 0.6104 | 101.41 KB |        0.89 |
| ParseMessage | Nuget Net4.8 | 51.63 us | 21.251 us | 1.165 us |  1.84 |    0.02 | 23.6816 | 2.0142 | 145.69 KB |        1.27 |
| ParseMessage | Nuget Net8   | 28.12 us |  5.295 us | 0.290 us |  1.00 |    0.00 |  7.4768 | 0.7019 | 114.51 KB |        1.00 |
 */

        private class Config : ManualConfig
        {
            public Config()
            {
                var baseJob = Job.ShortRun;
                WithOptions(ConfigOptions.DisableOptimizationsValidator);
                AddJob(baseJob.WithNuGet("HL7-dotnetcore", "2.37.0").WithRuntime(ClrRuntime.Net48).WithId("Nuget Net4.8"));
                
                AddJob(baseJob.WithNuGet("HL7-dotnetcore", "2.37.0").WithRuntime(CoreRuntime.Core80).WithId("Nuget Net8").AsBaseline());
                
                AddJob(baseJob.WithRuntime(ClrRuntime.Net48).WithCustomBuildConfiguration("LOCAL_CODE")
                    .WithId("Local Net4.8")); // custom config to include/exclude nuget reference or target project reference locally
                
                AddJob(baseJob.WithRuntime(CoreRuntime.Core80).WithCustomBuildConfiguration("LOCAL_CODE")
                    .WithId("Local Net8")); // custom config to include/exclude nuget reference or target project reference locally
            }
        }

        [Benchmark]
        public void ParseMessage()
        {
            //_testHarness.BypassValidationGetACK();
            BypassValidationGetACK();
        }

        private void BypassValidationGetACK()
        {
            string sampleMessage = @"MSH|^~\&|SCA|SCA|LIS|LIS|202107300000||ORU^R01||P|2.4|||||||
PID|1|1234|1234||JOHN^DOE||19000101||||||||||||||
OBR|1|1234|1234||||20210708|||||||||||||||20210708||||||||||
OBX|1|TX|SCADOCTOR||^||||||F";
            var msg = new Message(sampleMessage);
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