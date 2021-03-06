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

  public class Int16Value : IProtoValue<short>
  {
    public TType Type
    {
      get { return TType.I16; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.Call(iprot,typeof(TProtocol).GetMethod("ReadI16"));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(short));
      return Expression.Call(oprot, typeof(TProtocol).GetMethod("WriteI16"), value);
    }

    public short Read(TProtocol iprot)
    {
      return iprot.ReadI16();
    }

    public void Write(TProtocol oprot, short value)
    {
      oprot.WriteI16(value);
    }

    public short Read(XElement xe)
    {
      return CommonUtils.ToInt16(xe.Value);
    }
    public void Write(XElement xe, short value)
    {
      xe.Value = CommonUtils.ToString(value);
    }

    public short Read(XmlReader reader)
    {
      return CommonUtils.ToInt16(reader.ReadElementString());
    }

    public void Write(XmlWriter writer, short value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }    

    public short Read(JsonReader reader)
    {
      return CommonUtils.ToInt16(reader.Value.ToString());      
    }

    public void Write(JsonWriter writer, short value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }
  }
}
