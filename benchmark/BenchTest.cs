
namespace Bench
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using System.Reflection;

  using BenchmarkDotNet.Attributes;
  using BenchmarkDotNet.Attributes.Jobs;

  using BenchmarkDotNet.Running;

  public abstract class BenchTest : CommandLine
  {
    [CommandLineParameter]
    public int warms = 0;

    [CommandLineParameter]
    public int runs = 1;

    [CommandLineParameter]
    public int loops = 1;

    [CommandLineParameter]
    public String test = string.Empty;

    [CommandLineParameter]
    public String output = string.Empty;

    public void Run()
    {
      Setup();

      WriteLine("warm {0} times", this.warms);

      for (int i = 0; i < this.warms; i++)
      {
        RunOnce();
      }

      WriteLine("run {0} times, loop {1} times", this.runs, this.loops);

      long total = 0;
      long[] elapsed = new long[this.runs];
      long tms = TimeSpan.TicksPerMillisecond / 1000;

      GC.Collect(2);

      int gc0 = GC.CollectionCount(0);
      int gc1 = GC.CollectionCount(1);
      int gc2 = GC.CollectionCount(2);

      var watch = Stopwatch.StartNew();
      for (int i = 0; i < this.runs; i++)
      {
        watch.Restart();

        for (int j = 0; j < this.loops; j++)
        {
          RunOnce();
        }

        elapsed[i] = watch.Elapsed.Ticks;
        total += elapsed[i];
        WriteLine("elapsed {0} ms", elapsed[i] / tms);
      }

      gc0 = GC.CollectionCount(0) - gc0;
      gc1 = GC.CollectionCount(1) - gc1;
      gc2 = GC.CollectionCount(2) - gc2;

      GC.Collect(2);

      elapsed = elapsed.OrderBy(x => x).ToArray();

      Action<TextWriter> report = delegate (TextWriter writer)
      {
        CommandModuleAttribute ca = this.GetType().GetCustomAttribute<CommandModuleAttribute>();
        string testName = ca?.ShortName ?? this.GetType().Name.ToLower();
        writer.Write("{0}(", testName);
        writer.Write("warms:{0}, ", this.warms);
        writer.Write("runs:{0}, ", this.runs);
        writer.Write("loop:{0}, ", this.loops);
        writer.Write("total:{0:N0}, ", (long)elapsed.Sum() / tms);
        writer.Write("avg:{0:N0}, ", (long)elapsed.Average() / tms);
        writer.Write("max:{0:N0}, ", (long)elapsed.Max() / tms);
        writer.Write("min:{0:N0}, ", (long)elapsed.Min() / tms);
        writer.Write("median:{0:N0}, ", (long)elapsed[elapsed.Length / 2] / tms);
        writer.Write("p95:{0:N0}, ", (long)elapsed[elapsed.Length * 95 / 100] / tms);
        writer.Write("p99:{0:N0}, ", (long)elapsed[elapsed.Length * 99 / 100] / tms);
        writer.Write("p999:{0:N0}, ", (long)elapsed[elapsed.Length * 999 / 1000] / tms);
        writer.Write("gc0:{0}, ", gc0);
        writer.Write("gc1:{0}, ", gc1);
        writer.Write("gc2:{0}", gc2);
        writer.WriteLine(")");
        writer.Dispose();
      };

      report(Console.Out);

      if (!string.IsNullOrEmpty(this.output))
      {
        using (TextWriter writer = new StreamWriter(new FileStream(this.output, FileMode.Append)))
        {
          report(writer);
        }
      }

      Cleanup();
    }

    public abstract void RunOnce();

    public virtual void Setup()
    {
    }
    public virtual void Cleanup()
    {
    }

    [Conditional("DEBUG")]
    public void WriteLine(string msg, params object[] args)
    {
      Console.WriteLine(CommonUtils.Format(msg, args));
    }

  }
}
