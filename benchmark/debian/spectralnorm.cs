/**

http://benchmarksgame.alioth.debian.org/u64q/csharp.html

NOTES:
64-bit Ubuntu quad core
1.0.0-preview2-1-003177
"System.GC.Server": true

MONO-MAKE:
mv binarytrees.csharpllvm-4.csharpllvm binarytrees.csharpllvm-4.cs
/usr/local/bin/mcs  -optimize+ -platform:x86 -out:binarytrees.csharpllvm-4.csharpllvm_run binarytrees.csharpllvm-4.cs
rm binarytrees.csharpllvm-4.cs
3.83s to complete and log all make actions

COMMAND LINE:
/usr/local/bin/mono --llvm --gc=sgen binarytrees.csharpllvm-4.csharpllvm_run 20

CLR-MAKE:
cp spectralnorm.csharpcore-3.csharpcore Program.cs
cp Include/csharpcore/project.json .
cp Include/csharpcore/project.lock.json .
/usr/bin/dotnet build -c Release
Project tmp (.NETCoreApp,Version=v1.0) will be compiled because expected outputs are missing
Compiling tmp for .NETCoreApp,Version=v1.0

Compilation succeeded.
    0 Warning(s)
    0 Error(s)

Time elapsed 00:00:01.6891497
 

2.56s to complete and log all make actions

COMMAND LINE:
/usr/bin/dotnet ./bin/Release/netcoreapp1.0/tmp.dll 5500

PROGRAM OUTPUT:
1.274224153

*/

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

[Bench.CommandModule]
class spectralnorm : Bench.CommandLine
{
    //public static void Main (String [] args)
    //{
    //    var watch = System.Diagnostics.Stopwatch.StartNew();
    //    Run(args);
    //    Console.WriteLine ("elapsed {0} ms", watch.ElapsedMilliseconds);
    //}

    public static void Run (String [] args)
    {
        int n = 100;
        if (args.Length > 0) n = Int32.Parse (args [0]);

        Console.WriteLine ("{0:f9}", spectralnormGame (n));
    }

    private static double spectralnormGame (int n)
    {
        double [] u = new double [n];
        double [] v = new double [n];
        double [] tmp = new double [n];

        // create unit vector
        for (int i = 0; i < n; i++)
            u [i] = 1.0;

        int nthread = Environment.ProcessorCount;
        int chunk = n / nthread;
        var barrier = new Barrier (nthread);
        Approximate [] ap = new Approximate [nthread];

        for (int i = 0; i < nthread; i++) {
            int r1 = i * chunk;
            int r2 = (i < (nthread - 1)) ? r1 + chunk : n;
            ap [i] = new Approximate (u, v, tmp, r1, r2, barrier);
        }

        double vBv = 0, vv = 0;
        for (int i = 0; i < nthread; i++) {
            ap [i].t.Wait ();
            vBv += ap [i].m_vBv;
            vv += ap [i].m_vv;
        }

        return Math.Sqrt (vBv / vv);
    }

}

public class Approximate
{
    private Barrier barrier;
    public Task t;

    private double [] _u;
    private double [] _v;
    private double [] _tmp;

    private int range_begin, range_end;
    public double m_vBv, m_vv;

    public Approximate (double [] u, double [] v, double [] tmp, int rbegin, int rend, Barrier b)
    {
        m_vBv = 0;
        m_vv = 0;
        _u = u;
        _v = v;
        _tmp = tmp;
        range_begin = rbegin;
        range_end = rend;
        barrier = b;
        t = Task.Run (() => run ());
    }

    private void run ()
    {
        // 20 steps of the power method
        for (int i = 0; i < 10; i++) {
            MultiplyAtAv (_u, _tmp, _v);
            MultiplyAtAv (_v, _tmp, _u);
        }

        for (int i = range_begin; i < range_end; i++) {
            m_vBv += _u [i] * _v [i];
            m_vv += _v [i] * _v [i];
        }
    }

    /* return element i,j of infinite matrix A */
    private double eval_A (int i, int j)
    {
        return 1.0 / ((i + j) * (i + j + 1) / 2 + i + 1);
    }

    /* multiply vector v by matrix A, each thread evaluate its range only */
    private void MultiplyAv (double [] v, double [] Av)
    {
        for (int i = range_begin; i < range_end; i++) {
            double sum = 0;
            for (int j = 0; j < v.Length; j++)
                sum += eval_A (i, j) * v [j];

            Av [i] = sum;
        }
    }

    /* multiply vector v by matrix A transposed */
    private void MultiplyAtv (double [] v, double [] Atv)
    {
        for (int i = range_begin; i < range_end; i++) {
            double sum = 0;
            for (int j = 0; j < v.Length; j++)
                sum += eval_A (j, i) * v [j];

            Atv [i] = sum;
        }
    }

    /* multiply vector v by matrix A and then by matrix A transposed */
    private void MultiplyAtAv (double [] v, double [] tmp, double [] AtAv)
    {

        MultiplyAv (v, tmp);
        // all thread must syn at completion
        barrier.SignalAndWait ();
        MultiplyAtv (tmp, AtAv);
        // all thread must syn at completion
        barrier.SignalAndWait ();
    }

}

