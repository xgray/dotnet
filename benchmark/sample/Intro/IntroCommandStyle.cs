using System.Threading;
using BenchmarkDotNet.Attributes;

namespace BenchmarkDotNet.Samples.Intro
{
    [DryJob, StdDevColumn]
    public class IntroCommandStyle
    {
        [Benchmark]
        public void Benchmark()
        {
            Thread.Sleep(10);
        }
    }
}