
namespace Bench
{
  using System.Collections.Concurrent;
  using System.Threading;

  [CommandModule(ShortName = "cqueue")]
  [BenchmarkDotNet.Attributes.Jobs.SimpleJob]
  public class ConcurrentQueueTest : BenchTest
  {
    [CommandLineParameter]
    public int limit = 1000;

    [CommandLineParameter]
    public int load = 100000;

    [CommandLineParameter]
    public int workers = 4;

    [BenchmarkDotNet.Attributes.Setup]
    public override void Setup()
    {
    }

    public override void RunOnce()
    {

      ConcurrentQueue<int> queue = new ConcurrentQueue<int>();
      int count = 0;

      for (int i = 0; i < workers; i++)
      {
        int id = i;
        ThreadPool.QueueUserWorkItem(delegate
        {
          for (int a = 0; a < load; a++)
          {
            if (Interlocked.Increment(ref count) > limit)
            {
              do
              {
                // Thread.Sleep(0);
                
                Interlocked.MemoryBarrier();
              }
              while (count >= limit);
            }
            queue.Enqueue(a);
            if ((a & 0xffff) == 0)
            {
              WriteLine("{0}:{1:X}:{2}", id, a, count);
            }
          }
          WriteLine("{0}:Done:{1}", id, count);

        });
      }

      ManualResetEvent mre = new ManualResetEvent(false);
      ThreadPool.QueueUserWorkItem(delegate
      {
        for (int a = 0; a < load * workers; a++)
        {
          int x = 0;
          while (!queue.TryDequeue(out x))
          {
            //Thread.Sleep(0);;
          }
          Interlocked.Decrement(ref count);
          if ((a & 0xffff) == 0)
          {
            WriteLine("dequeue:{0:X}:{1}", a, count);
          }
        }
        WriteLine("Done");
        mre.Set();
      });

      mre.WaitOne();
    }

  }
}
