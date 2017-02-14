/*

http://benchmarksgame.alioth.debian.org/u64q/csharp.html

NOTES:
64-bit Ubuntu quad core
1.0.0-preview2-1-003177
"System.GC.Server": true


Thu, 17 Nov 2016 00:09:36 GMT

MAKE:
cp mandelbrot.csharpcore-5.csharpcore Program.cs
cp Include/csharpcore/project.json .
cp Include/csharpcore/project.lock.json .
/usr/bin/dotnet build -c Release
Project tmp (.NETCoreApp,Version=v1.0) will be compiled because expected outputs are missing
Compiling tmp for .NETCoreApp,Version=v1.0

Compilation succeeded.
    0 Warning(s)
    0 Error(s)

Time elapsed 00:00:01.7882213
 

2.62s to complete and log all make actions

COMMAND LINE:
/usr/bin/dotnet ./bin/Release/netcoreapp1.0/tmp.dll 16000
*/

using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Runtime.CompilerServices;

[Bench.CommandModule]
public class MandelBrot : Bench.CommandLine
{
    private static int n = 200;
    private static byte[][] data;
    private static int lineCount = -1;

    private static double[] Crb;
    private static double[] Cib;

     [MethodImpl(MethodImplOptions.AggressiveInlining)]
     static int getByte(int x, int y){
      int res=0;
      for(int i=0;i<8;i+=2){
         double Zr1=Crb[x+i];
         double Zi1=Cib[y];

         double Zr2=Crb[x+i+1];
         double Zi2=Cib[y];

         int b=0;
         int j=49;do{
            double nZr1=Zr1*Zr1-Zi1*Zi1+Crb[x+i];
            double nZi1=Zr1*Zi1+Zr1*Zi1+Cib[y];
            Zr1=nZr1;Zi1=nZi1;

            double nZr2=Zr2*Zr2-Zi2*Zi2+Crb[x+i+1];
            double nZi2=Zr2*Zi2+Zr2*Zi2+Cib[y];
            Zr2=nZr2;Zi2=nZi2;

            if(Zr1*Zr1+Zi1*Zi1>4){b|=2;if(b==3)break;}
            if(Zr2*Zr2+Zi2*Zi2>4){b|=1;if(b==3)break;}
         }while(--j>0);
         res=(res<<2)+b;
      }
      return res^-1;
    }

    //public static void Main (String [] args)
    //{
    //    var watch = System.Diagnostics.Stopwatch.StartNew();
    //    Run(args);
    //    Console.WriteLine ("elapsed {0} ms", watch.ElapsedMilliseconds);
    //}

    public static void Run (String[] args)
    {
        if (args.Length > 0) n = Int32.Parse(args[0]);
       
        int lineLen = (n-1)/8 + 1;
        data = new byte[n][];
           
        Crb=new double[n+7];
        Cib =new double[n+7];
        
        double invN=2.0/n; for(int i=0;i<n;i++){ Cib[i]=i*invN-1.0; Crb[i]=i*invN-1.5; }
        
        var threads = new Thread[Environment.ProcessorCount]; 
        for (int i = 0; i < threads.Length; i++)
        {   
            threads[i] = new Thread(() => {
                                               int y;
                                               while ((y = Interlocked.Increment(ref lineCount)) < n)
                                               {
                                                   var buffer = new byte[lineLen];                    
                                                   for (int x = 0; x < lineLen; x++)
                                                   {
                                                       buffer[x] = (byte) getByte(x*8, y);
                                                   }
                                                   data[y] = buffer; 
                                               }
            });
            threads[i].Start();
        }

        foreach (var t in threads) t.Join();
        
        // Console.Out.WriteLine("P4\n{0} {0}", n);
        var s = new MemoryStream();
        for (int y = 0; y < n; y++) s.Write(data[y], 0, lineLen);        
    }  
}
