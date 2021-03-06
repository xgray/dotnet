﻿
namespace Thrift.Net
{
  using System;
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Xml;
  using System.Xml.Linq;

  using Bench;
  using Newtonsoft.Json;
  using Thrift.Protocol;

  public class DateTimeValue : IProtoValue<DateTime>
  {
    public TType Type
    {
      get { return TType.I64; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.New(
        typeof(DateTime).GetConstructor(typeof(long), typeof(DateTimeKind)),
        Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadI64")),
        Expression.Constant(DateTimeKind.Utc));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(DateTime));

      return Expression.Call(
        oprot,
        typeof(TProtocol).GetMethod("WriteI64"),
        Expression.Property(
          Expression.Call(value, typeof(DateTime).GetMethod("ToUniversalTime")),
          typeof(DateTime).GetProperty("Ticks")
        ));
    }

    public DateTime Read(TProtocol iprot)
    {
      return new DateTime(iprot.ReadI64(), DateTimeKind.Utc);
    }

    public void Write(TProtocol oprot, DateTime value)
    {
      oprot.WriteI64(value.ToUniversalTime().Ticks);
    }

    public DateTime Read(XElement xe)
    {
      return CommonUtils.ToDateTime(xe.Value);
    }
    public void Write(XElement xe, DateTime value)
    {
      xe.Value = CommonUtils.ToString(value);
    }

    public DateTime Read(XmlReader reader)
    {
      return CommonUtils.ToDateTime(reader.ReadElementString());
    }

    public void Write(XmlWriter writer, DateTime value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }

    public DateTime Read(JsonReader reader)
    {
      return CommonUtils.ToDateTime(reader.Value.ToString());      
    }

    public void Write(JsonWriter writer, DateTime value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }
  }
}
