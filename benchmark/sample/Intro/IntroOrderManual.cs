﻿using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace BenchmarkDotNet.Samples.Intro
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    [DryJob]
    public class IntroOrderManual
    {
        [Params(1, 2, 3)]
        public int X { get; set; }

        [Benchmark]
        public void Slow() => Thread.Sleep(X * 100);

        [Benchmark]
        public void Fast() => Thread.Sleep(X * 50);
    }
}