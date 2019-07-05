/**
 * Autogenerated by Thrift Compiler (0.10.0)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */

namespace sbe
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.IO;

  using Adaptive.SimpleBinaryEncoding;
  using Sbe;
  using Bench;

  [CommandModule(ShortName = "sbe")]
  [BenchmarkDotNet.Attributes.SimpleJob]
  public class SbeBench : SerializeTest
  {

    [CommandLineParameter]
    public string simpleValue = "abc";

    private DirectBuffer buffer;

    private MessageHeader messageHeader;

    private Simple simpleInput;
    private Simple simpleOutput;

    private Complex complexInput;
    private Complex complexOutput;

    [BenchmarkDotNet.Attributes.GlobalSetup]
    public override void Setup()
    {
      buffer = new DirectBuffer(new byte[1024]);
      simpleInput = new Simple();
      simpleOutput = new Simple();
      complexInput = new Complex();
      complexOutput = new Complex();
      messageHeader = new MessageHeader();
    }


    [BenchmarkDotNet.Attributes.Benchmark]
    public override void SimpleTest()
    {
      const int bufferIndex = 0;

      messageHeader.Wrap(buffer, bufferIndex, 0);
      messageHeader.TemplateId = Simple.TemplateId;
      messageHeader.Version = Simple.TemplateVersion;
      messageHeader.BlockLength = Simple.BlockLength;

      simpleInput.WrapForEncode(buffer, bufferIndex + MessageHeader.Size);

      for (int i = 0; i < simpleValue.Length; i++)
      {
        simpleInput.SetValue(i, (byte)simpleValue[i]);
      }

      simpleInput.ShortValue = 16;
      simpleInput.IntValue = 17;
      simpleInput.LongValue = 18;

      messageHeader.Wrap(buffer, bufferIndex, 0);

      int actingVersion = messageHeader.Version;
      int actingBlockLength = messageHeader.BlockLength;

      simpleOutput.WrapForDecode(
        buffer, 
        bufferIndex + MessageHeader.Size, 
        actingBlockLength, 
        actingVersion);

      WriteLine("{0}:{1},{2},{3},{4},{5},{6}",
        simpleInput.Size + MessageHeader.Size,
        simpleOutput.GetValue(0),
        simpleOutput.GetValue(1),
        simpleOutput.GetValue(2),
        simpleOutput.ShortValue,
        simpleOutput.IntValue,
        simpleOutput.LongValue);
    }

    [BenchmarkDotNet.Attributes.Benchmark]
    public override void ComplexTest()
    {
      const int bufferIndex = 0;

      messageHeader.Wrap(buffer, bufferIndex, 0);
      messageHeader.TemplateId = Simple.TemplateId;
      messageHeader.Version = Simple.TemplateVersion;
      messageHeader.BlockLength = Simple.BlockLength;

      complexInput.WrapForEncode(buffer, bufferIndex + MessageHeader.Size);
      SimpleType simple = complexInput.SimpleValue;

      for (int i = 0; i < simpleValue.Length; i++)
      {
        simple.SetValue(i, (byte)simpleValue[i]);
      }

      simple.ShortValue = 16;
      simple.IntValue = 17;
      simple.LongValue = 18;

      var listValue = complexInput.ListValueCount(2);

      listValue.Next();
      for (int i = 0; i < simpleValue.Length; i++)
      {
        listValue.SetValue(i, (byte)simpleValue[i]);
      }
      listValue.ShortValue = 16;
      listValue.IntValue = 17;
      listValue.LongValue = 18;

      listValue.Next();
      for (int i = 0; i < simpleValue.Length; i++)
      {
        listValue.SetValue(i, (byte)simpleValue[i]);
      }
      listValue.ShortValue = 16;
      listValue.IntValue = 17;
      listValue.LongValue = 18;

      messageHeader.Wrap(buffer, bufferIndex, 0);

      int actingVersion = messageHeader.Version;
      int actingBlockLength = messageHeader.BlockLength;

      complexOutput.WrapForDecode(
        buffer, 
        bufferIndex + MessageHeader.Size, 
        actingBlockLength, 
        actingVersion);

      simple = complexOutput.SimpleValue;

      WriteLine("{0}:{1},{2},{3},{4},{5},{6}",
        complexInput.Size + MessageHeader.Size,
        simple.GetValue(0),
        simple.GetValue(1),
        simple.GetValue(2),
        simple.ShortValue,
        simple.IntValue,
        simple.LongValue);

      listValue = complexOutput.ListValue;
      while (listValue.HasNext) {
        listValue.Next();
        WriteLine("{0},{1},{2},{3},{4},{5}",
          listValue.GetValue(0),
          listValue.GetValue(1),
          listValue.GetValue(2),
          listValue.ShortValue,
          listValue.IntValue,
          listValue.LongValue);        
      }
    }

  }
}
