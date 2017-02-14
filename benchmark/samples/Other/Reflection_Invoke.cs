using System;
using System.Reflection;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace BenchmarkDotNet.Samples.Other
{
    [Config(typeof(Config))]
    public class Reflection_Invoke
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                Add(new Job
                {
                    Run = { LaunchCount = 3, TargetCount = 100 }
                });
            }
        }

        [Benchmark]
        public void Rand()
        {
            new Random().Next();
        }

        [Benchmark]
        public void Reflection()
        {
            var randomType = Type.GetType("System.Random");
            var nextMethod = randomType.GetTypeInfo().GetMethod("Next", Type.EmptyTypes);
            var rand = Activator.CreateInstance(randomType);
            nextMethod.Invoke(rand, null);
        }
    }
}