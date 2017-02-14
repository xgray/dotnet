/*
NOTES:
64-bit Ubuntu quad core
1.0.0-preview2-1-003177
"System.GC.Server": true


Thu, 17 Nov 2016 00:29:50 GMT

MAKE:
cp regexdna.csharpcore-8.csharpcore Program.cs
cp Include/csharpcore/project.json .
cp Include/csharpcore/project.lock.json .
/usr/bin/dotnet build -c Release
Project tmp (.NETCoreApp,Version=v1.0) will be compiled because expected outputs are missing
Compiling tmp for .NETCoreApp,Version=v1.0

Compilation succeeded.
    0 Warning(s)
    0 Error(s)

Time elapsed 00:00:01.7859466
 

2.62s to complete and log all make actions

COMMAND LINE:
/usr/bin/dotnet ./bin/Release/netcoreapp1.0/tmp.dll 0 < regexdna-input5000000.txt
*/

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[Bench.CommandModule]
class regexdna : Bench.CommandLine
{
    static string readStdIn(out int seqLength, out int inputLength)
    {
        StringBuilder sb = new StringBuilder();
        int commentLength = 0;
        String line;
        
        while ((line = Console.ReadLine()) != null)
        {
            if (line[0] == '>')
                commentLength += line.Length + 1;
            else
            {
                sb.Append(line);
                commentLength += 1;
            }
        }
        seqLength = sb.Length;
        inputLength = seqLength + commentLength; 
        return sb.ToString();
    }

    //public static void Main (String [] args)
    //{
    //    var watch = System.Diagnostics.Stopwatch.StartNew();
    //    Run();
    //    Console.WriteLine ("elapsed {0} ms", watch.ElapsedMilliseconds);
    //}

    static void Run()
    {

        string[] variants = {
           "agggtaaa|tttaccct"
          ,"[cgt]gggtaaa|tttaccc[acg]"
          ,"a[act]ggtaaa|tttacc[agt]t"
          ,"ag[act]gtaaa|tttac[agt]ct"
          ,"agg[act]taaa|ttta[agt]cct"
          ,"aggg[acg]aaa|ttt[cgt]ccct"
          ,"agggt[cgt]aa|tt[acg]accct"
          ,"agggta[cgt]a|t[acg]taccct"
          ,"agggtaa[cgt]|[acg]ttaccct"
        };
         
        int seqLength, initialLength;
        var sequence = readStdIn(out seqLength, out initialLength);
        var newSequenceLength = Task.Run(() =>
            {
                var table = new int['Z'];
                table['D'] = "(a|g|t)".Length - 1;
                table['H'] = "(a|c|t)".Length - 1;
                table['K'] = "(g|t)".Length - 1;
                table['M'] = "(a|c)".Length - 1;
                table['N'] = "(a|c|g|t)".Length - 1;
                table['R'] = "(a|g)".Length - 1;
                table['S'] = "(c|g)".Length - 1;
                table['V'] = "(a|c|g)".Length - 1;
                table['W'] = "(a|t)".Length - 1;
                table['Y'] = "(c|t)".Length - 1;
                table['B'] = "(c|g|t)".Length - 1;

                var r = new Regex("[WYKMSRBDVHN]", RegexOptions.Compiled);

                int length = sequence.Length;

                for (Match m = r.Match(sequence); m.Success; m = m.NextMatch())
                {
                    length += table[m.Value[0]];
                }
                
                return length;
            });

        var output = new string[variants.Length];
        Parallel.For(0, variants.Length, i =>
        {
            Regex r = new Regex(variants[i], RegexOptions.Compiled);            
            output[i] = r.ToString() + " " + r.Matches(sequence).Count;
        });

        foreach (var s in output)
            Console.WriteLine(s);
        
        Console.WriteLine("\n{0}\n{1}\n{2}", initialLength, seqLength, newSequenceLength.Result);        
    }
}
