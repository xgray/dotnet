
using System;
using System.Threading;
using System.Threading.Tasks;

[Bench.CommandModule]
[BenchmarkDotNet.Attributes.Jobs.SimpleJob]
public class math : Bench.BenchTest
{
    [Bench.CommandLineParameter]
    public int r = 2;

    // public static void Run(String[] args)
    // {
    //     var math = new math { InputArgs = args };
    //     math.RunTest();
    // }

    [BenchmarkDotNet.Attributes.Benchmark]
    public override void RunOnce()
    {
        long n = (long)Math.Pow(10, r);

        long result = 0;
        for (long i = 0; i < n; i++)
        {
            result += i;
            result /= 2;
        }

        WriteLine("{0} -> {1}", n, result);
    }
}
