// using BenchmarkDotNet.Attributes;
// using HL7.Dotnetcore;
// using System.IO;
//
// namespace Benchmarks
// {
//     [SimpleJob]
//     [MemoryDiagnoser]
//     public class QueryFieldBench
//     {
//         private Message _ormMessage;
//
//         [GlobalSetup]
//         public void Setup()
//         {
//             var ormText = File.ReadAllText("Sample-ORM.txt");
//             _ormMessage = new Message(ormText);
//             _ormMessage.ParseMessage();
//         }
//
//         [Benchmark]
//         public void QueryRequestDateTime()
//         {
//             var value = _ormMessage.GetValue("OBR.6");
//         }
//         
//     }
// }