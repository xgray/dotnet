/**
 * Autogenerated by Thrift Compiler (0.10.0)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */

namespace prototuple
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.IO;

  using Bench;
  using ProtoInsight;

  [CommandModule(ShortName="prototuple")]
  [BenchmarkDotNet.Attributes.SimpleJob]
  public class ProtoTupleBench : SerializeTest
  {
    [CommandLineParameter]
    public string simpleValue = "abc";

    private Simple simpleInput;
    private Simple simpleOutput;

    private Complex complexInput;
    private Complex complexOutput;

    private MemoryStream stream;

    private ProtoSchema[] simpleSchema;
    private ProtoSchema[] complexSchema;

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
      complexInput.ListValue = new List<Simple>();
      complexInput.ListValue.Add(simpleInput);
      complexInput.ListValue.Add(simpleInput);

      // complexInput.ListValue = simpleInput.ToEnumerable().ToList();
      // complexInput.MapValue = new Dictionary<string, Simple>
      // {
      //   {"a", simpleInput},
      //   {"b", simpleInput}
      // };

      simpleSchema = Proto<Simple>.Schemas;
      complexSchema = Proto<Complex>.Schemas;
      stream = new MemoryStream(1024);
    }

    [BenchmarkDotNet.Attributes.Benchmark]
    public override void SimpleTest()
    {
      stream.Position = 0;
      Serialize(simpleSchema, simpleInput);
      
      stream.Position = 0;
      Deseralize(simpleSchema, simpleOutput);

      WriteLine("{0}:{1},{2},{3},{4}",
        stream.Length,
        simpleOutput.Value,
        simpleOutput.ShortValue,
        simpleOutput.IntValue,
        simpleOutput.LongValue);
    }

    [BenchmarkDotNet.Attributes.Benchmark]
    public override void ComplexTest()
    {
      stream.Position = 0;
      Serialize(complexSchema, complexInput);
      
      stream.Position = 0;
      Deseralize(complexSchema, complexOutput);

      WriteLine("{0}:{1},{2},{3},{4}",
        stream.Length,
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
    private void Serialize<T>(ProtoSchema[] schemas, T proto)
    {
      foreach (ProtoSchema schema in schemas)
      {
        int startPos = (int)stream.Position;
        stream.Write(BitConverter.GetBytes(startPos), 0, sizeof(int));

        stream.Serialize(proto, schema);

        int endPos = (int)stream.Position;
        stream.Position = startPos;
        stream.Write(BitConverter.GetBytes(endPos - startPos - sizeof(int)), 0, sizeof(int));
        stream.Position = endPos;
      }
    }

    private byte[] buffer = new byte[1024];

    private void Deseralize<T>(ProtoSchema[] schemas, T proto)
    {
      foreach (ProtoSchema schema in schemas)
      {
        stream.Read(buffer, 0, sizeof(int));        
        stream.Deserialize(proto, schema);
      }
    }
  }


  [Proto]
  public class Simple
  {
    [ProtoColumn(1)]
    public string Value { get; set; }
    [ProtoColumn(2)]
    public short ShortValue { get; set; }

    [ProtoColumn(3)]
    public int IntValue { get; set; }

    [ProtoColumn(4)]
    public long LongValue { get; set; }
  }

  [Proto]
  public class Complex
  {
    [ProtoColumn(1)]
    public Simple SimpleValue { get; set; }
    [ProtoColumn(2)]
    public List<Simple> ListValue { get; set; }
    [ProtoColumn(3)]
    public Dictionary<string, Simple> MapValue { get; set; }
  }
}
