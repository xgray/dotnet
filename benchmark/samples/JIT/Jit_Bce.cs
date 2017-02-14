﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace BenchmarkDotNet.Samples.JIT
{
    [LegacyJitX86Job, LegacyJitX64Job, RyuJitX64Job]
    public class Jit_Bce
    {
        private const int N = 11;
        private int[] x = new int[N];

        [Benchmark(OperationsPerInvoke = N)]
        public int SumConst()
        {
            var y = x;
            int sum = 0;
            for (int i = 0; i < N; i++)
                sum += y[i];
            return sum;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public int SumLength()
        {
            var y = x;
            int sum = 0;
            for (int i = 0; i < y.Length; i++)
                sum += y[i];
            return sum;
        }

        // See also: http://en.wikipedia.org/wiki/Bounds-checking_elimination
        // See also: http://www.codeproject.com/Articles/25801/JIT-Optimizations
    }
}