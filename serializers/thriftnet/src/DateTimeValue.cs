
namespace Thrift.Net
{
  using System;
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Xml.Linq;

  using Bench;

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
        typeof(DateTime).GetConstructor(typeof(long)),
        Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadI64")));
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
  }
}
