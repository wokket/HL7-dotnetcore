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
    public class ParseMessageBench
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                var baseJob = Job.ShortRun; 
                WithOptions(ConfigOptions.DisableOptimizationsValidator);
                AddJob(baseJob.WithNuGet("HL7-dotnetcore", "2.37.0").WithRuntime(ClrRuntime.Net48));
                AddJob(baseJob.WithNuGet("HL7-dotnetcore", "2.37.0").WithRuntime(CoreRuntime.Core80));
                AddJob(baseJob.WithRuntime(ClrRuntime.Net48).WithCustomBuildConfiguration("LOCAL_CODE")); // custom config to include/exclude nuget reference or target project reference locally
                AddJob(baseJob.WithRuntime(CoreRuntime.Core80).WithCustomBuildConfiguration("LOCAL_CODE")); // custom config to include/exclude nuget reference or target project reference locally
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