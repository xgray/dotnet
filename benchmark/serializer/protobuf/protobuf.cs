/**
 * Autogenerated by Thrift Compiler (0.10.0)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */

namespace protobuf
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.IO;

  using Bench;
  using Google.Protobuf;

  [CommandModule(ShortName = "protobuf")]
  [BenchmarkDotNet.Attributes.SimpleJob]
  public class ProtoBufBench : SerializeTest
  {
    [CommandLineParameter]
    public string simpleValue = "abc";

    private Simple simpleInput;
    private Simple simpleOutput;

    private Complex complexInput;
    private Complex complexOutput;

    private byte[] buffer;

    [BenchmarkDotNet.Attributes.GlobalSetup]
    public override void Setup()
    {
      simpleInput = new Simple();
      simpleInput.Value = simpleValue;
      simpleInput.ShortValue = 16;
      simpleInput.IntValue = 17;
      simpleInput.LongValue = 18;
      simpleOutput = new Simple();

      complexInput = new Complex();
      complexOutput = new Complex();
      complexInput.SimpleValue = simpleInput;
      complexInput.ListValue.Add(simpleInput);
      complexInput.ListValue.Add(simpleInput);

      // complexInput.MapValue = new Dictionary<string, Simple>
      // {
      //   {"a", simpleInput},
      //   {"b", simpleInput}
      // };

      buffer = new byte[1024];
    }

    [BenchmarkDotNet.Attributes.Benchmark]
    public override void SimpleTest()
    {
      var ostream = new CodedOutputStream(buffer);
      simpleInput.WriteTo(ostream);

      var istream = new CodedInputStream(buffer, 0, (int)ostream.Position);
      simpleOutput.MergeFrom(istream);

      WriteLine("{0}:{1},{2},{3},{4}",
        ostream.Position,
        simpleOutput.Value,
        simpleOutput.ShortValue,
        simpleOutput.IntValue,
        simpleOutput.LongValue);
    }

    [BenchmarkDotNet.Attributes.Benchmark]
    public override void ComplexTest()
    {
      var ostream = new CodedOutputStream(buffer);
      complexInput.WriteTo(ostream);

      var istream = new CodedInputStream(buffer, 0, (int)ostream.Position);
      complexOutput.ListValue.Clear();
      complexOutput.MergeFrom(istream);

      WriteLine("{0}:{1},{2},{3},{4}",
        ostream.Position,
        complexOutput.SimpleValue.Value,
        complexOutput.SimpleValue.ShortValue,
        complexOutput.SimpleValue.IntValue,
        complexOutput.SimpleValue.LongValue);

      for (int i = 0; i < complexOutput.ListValue.Count; i++)
      {
        WriteLine("{0},{1},{2},{3}",
          complexOutput.ListValue[i].Value,
          complexOutput.ListValue[i].ShortValue,
          complexOutput.ListValue[i].IntValue,
          complexOutput.ListValue[i].LongValue);
      }
    }
  }
}
