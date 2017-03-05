
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

  public class TimeSpanValue : IProtoValue<TimeSpan>
  {
    public TType Type
    {
      get { return TType.I64; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.New(
        typeof(TimeSpan).GetConstructor(typeof(long)),
        Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadI64")));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(TimeSpan));

      return Expression.Call(
        oprot,
        typeof(TProtocol).GetMethod("WriteI64"),
        Expression.Property(value,typeof(TimeSpan).GetProperty("Ticks")));
    }

    public TimeSpan Read(TProtocol iprot)
    {
      return TimeSpan.FromTicks(iprot.ReadI64());
    }

    public void Write(TProtocol oprot, TimeSpan value)
    {
      oprot.WriteI64(value.Ticks);
    }

    public TimeSpan Read(XElement xe)
    {
      return CommonUtils.ToTimeSpan(xe.Value);
    }
    public void Write(XElement xe, TimeSpan value)
    {
      xe.Value = CommonUtils.ToString(value);
    }

    public TimeSpan Read(XmlReader reader)
    {
      return CommonUtils.ToTimeSpan(reader.ReadElementString());
    }

    public void Write(XmlWriter writer, TimeSpan value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }

    public TimeSpan Read(JsonReader reader)
    {
      return CommonUtils.ToTimeSpan(reader.Value.ToString());      
    }

    public void Write(JsonWriter writer, TimeSpan value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }
  }
}
