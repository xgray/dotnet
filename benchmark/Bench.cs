
namespace Bench
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using BenchmarkDotNet.Attributes;

    using BenchmarkDotNet.Running;

    public class Bench : CommandLine
    {
        [CommandLineSwitchParameter]
        public bool bench = false;

        protected override string[] Prefixes
        {
            get { return new string[] { "--" }; }
        }

        public static void Main(string[] args)
        {
            Bench bench = new Bench { InputArgs = args };

            if (bench.bench)
            {
                bench.RunBench();
            }
            else
            {
                bench.RunCmd();
            }
        }

        private void RunCmd()
        {
            if (this.Args.Length == 0)
            {
                CommandLine.PrintModuleUsage();
                return;
            }

            CommandLine.RunModule(this.Args);
        }

        private void RunBench()
        {
            //var benchmarks =
            //    Assembly.GetEntryAssembly().GetTypes<JobConfigBaseAttribute>();

            //if (this.Args.Length == 0)
            //{
            //    Console.WriteLine("Benchmarks:");
            //    foreach (var benchmark in benchmarks)
            //    {
            //        Console.WriteLine(benchmark.Name);
            //    }
            //}
            //else
            //{
            //    foreach (var benchmark in benchmarks)
            //    {
            //        if (string.Compare(this.Args[0], benchmark.Name, true) == 0)
            //        {
            //            BenchmarkRunner.Run(benchmark);
            //            return;
            //        }
            //    }
            //}
            BenchmarkSwitcher
                .FromAssembly(typeof(Bench).GetTypeInfo().Assembly)
                .Run(this.Args);

        }
    }
}
