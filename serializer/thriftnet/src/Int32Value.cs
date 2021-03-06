﻿
namespace Thrift.Net
{
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Xml;
  using System.Xml.Linq;
  
  using Bench;
  using Newtonsoft.Json;
  using Thrift.Protocol;

  public class Int32Value : IProtoValue<int>
  {
    public TType Type
    {
      get { return TType.I32; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.Call(iprot,typeof(TProtocol).GetMethod("ReadI32"));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(int));
      return Expression.Call(oprot, typeof(TProtocol).GetMethod("WriteI32"), value);
    }

    public int Read(TProtocol iprot)
    {
      return iprot.ReadI32();
    }

    public void Write(TProtocol oprot, int value)
    {
      oprot.WriteI32(value);
    }

    public int Read(XElement xe)
    {
      return CommonUtils.ToInt32(xe.Value);
    }
    public void Write(XElement xe, int value)
    {
      xe.Value = CommonUtils.ToString(value);
    }

    public int Read(XmlReader reader)
    {
      return CommonUtils.ToInt32(reader.ReadElementString());
    }

    public void Write(XmlWriter writer, int value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }

    public int Read(JsonReader reader)
    {
      return CommonUtils.ToInt32(reader.Value.ToString());      
    }

    public void Write(JsonWriter writer, int value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }
  }
}
