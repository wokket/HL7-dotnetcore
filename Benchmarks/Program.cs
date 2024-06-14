// to run this targeting multiple frameworks, use 
//   dotnet run -c Release -f net48 --filter "*" --runtimes net48 net8.0
// I'm using net48 just to target the netstandard21 version of the library

using BenchmarkDotNet.Running;
using Benchmarks;

internal class Program
{
    public static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(ParseOrmBench).Assembly).Run(args);
    }
}