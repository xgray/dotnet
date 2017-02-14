/*
http://benchmarksgame.alioth.debian.org/u64q/csharp.html

NOTES:
64-bit Ubuntu quad core
1.0.0-preview2-1-003177
"System.GC.Server": true


Wed, 16 Nov 2016 23:19:01 GMT

MAKE:
cp binarytrees.csharpcore-4.csharpcore Program.cs
cp Include/csharpcore/project.json .
cp Include/csharpcore/project.lock.json .
/usr/bin/dotnet build -c Release
Project tmp (.NETCoreApp,Version=v1.0) will be compiled because expected outputs are missing
Compiling tmp for .NETCoreApp,Version=v1.0

Compilation succeeded.
    0 Warning(s)
    0 Error(s)

Time elapsed 00:00:01.7888234
 

2.64s to complete and log all make actions

COMMAND LINE:
/usr/bin/dotnet ./bin/Release/netcoreapp1.0/tmp.dll 20
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[Bench.CommandModule]
class BinaryTrees : Bench.CommandLine
{
    const int MIN_DEPTH = 4;

    //public static void Main (String [] args)
    //{
    //    var watch = System.Diagnostics.Stopwatch.StartNew();
    //    Run(args);
    //    Console.WriteLine ("elapsed {0} ms", watch.ElapsedMilliseconds);
    //}

    public static void Run(string[] args)
    {
        int n = 0;
        if (args.Length > 0) n = int.Parse(args[0]);

        int maxDepth = n < (MIN_DEPTH + 2) ? MIN_DEPTH + 2 : n;
        int stretchDepth = maxDepth + 1;

        Task<int>[] Tcheck =
        {
                Task.Run(() => TreeNode.bottomUpTree(0, stretchDepth).itemCheck()),
                Task.Run(() => TreeNode.bottomUpTree(0, maxDepth).itemCheck())
        };

        string[] results = new string[(maxDepth - MIN_DEPTH) / 2 + 1];

        var depts = new List<Action>(maxDepth);

        for (int d = maxDepth; d >= MIN_DEPTH; d -= 2)
        {
            var depth = d;
            depts.Add(() =>
            {
                int iterations = 1 << (maxDepth - depth + MIN_DEPTH);

                int check = 0;
                for (int i = 1; i <= iterations; i++)
                {
                    Task<int>[] btm =
                    {
                        Task.Run(() => TreeNode.bottomUpTree(i, depth).itemCheck()),
                        Task.Run(() => TreeNode.bottomUpTree(-i, depth).itemCheck())
                    };

                    Task.WaitAll(btm);

                    check += btm[0].Result + btm[1].Result;
                }

                results[(depth - MIN_DEPTH) / 2] = (iterations * 2) + "\t trees of depth " + depth + "\t check: " + check;
            });
        }

        Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount + 1 }, depts.ToArray());

        Task.WaitAll(Tcheck);

        Console.WriteLine("stretch tree of depth {0}\t check: {1}", stretchDepth, Tcheck[0].Result);
        foreach (var result in results)
        {
            Console.WriteLine(result);
        }

        Console.WriteLine("long lived tree of depth {0}\t check: {1}", maxDepth, Tcheck[1].Result);
    }

    struct TreeNode
    {
        sealed class Next
        {
            public TreeNode left, right;
        }

        private Next next;

        private int item;

        TreeNode(int item)
        {
            this.item = item;
            this.next = null;
        }

        TreeNode(TreeNode left, TreeNode right, int item)
        {
            this.next = new Next();
            this.next.left = left;
            this.next.right = right;
            this.item = item;
        }

        internal static TreeNode bottomUpTree(int item, int depth)
        {
            if (depth > 0)
            {
                int i1, i2, d;

                i2 = 2 * item;
                i1 = i2 - 1;
                d = depth - 1;

                var left = bottomUpTree(i1, d);
                var right = bottomUpTree(i2, d);
                return new TreeNode(left, right, item);
            }
            else
            {
                return new TreeNode(item);
            }
        }

        internal int itemCheck()
        {
            if (next == null)
            {
                return item;
            }
            return item + next.left.itemCheck() - next.right.itemCheck();
        }
    }
}
