// to run this targeting multiple frameworks, use 
//   dotnet run -c Release -f net48 --filter "*"
// I'm using net48 just to target the netstandard21 version of the library
#define LOCAL_CODE

using BenchmarkDotNet.Running;
using Benchmarks;

internal class Program
{
    public static void Main(string[] args)
    {
        
        //var obj = new ParseMessageBench();
        //obj.ParseMessageAndGetValues();

        BenchmarkSwitcher.FromAssembly(typeof(ParseMessageBench).Assembly).Run(args);
    }
}