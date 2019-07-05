using System.Threading;
using BenchmarkDotNet.Attributes;

namespace BenchmarkDotNet.Samples.Intro
{
    [ShortRunJob]
    [KeepBenchmarkFiles()]
    public class IntroJobsFull
    {
        [Benchmark(Baseline = true)]
        public void Sleep100() => Thread.Sleep(100);

        [Benchmark]
        public void Sleep50() => Thread.Sleep(50);
    }
}