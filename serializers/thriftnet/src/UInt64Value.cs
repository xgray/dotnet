﻿
namespace ProtoThrift
{
  using System.Linq.Expressions;
  using System.Reflection;
  
  using Bench;

  using Thrift.Protocol;

  public class UInt64Value : IProtoValue<ulong>
  {
    public TType Type
    {
      get { return TType.I64; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.Convert(
        Expression.Call(iprot,typeof(TProtocol).GetMethod("ReadI64")),
        typeof(ulong));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(ulong));

      return Expression.Call(
        oprot,
        typeof(TProtocol).GetMethod("WriteI64"),
        Expression.Convert(value, typeof(long)));
    }

    public ulong Read(TProtocol iprot)
    {
      return (ulong)iprot.ReadI64();
    }

    public void Write(TProtocol oprot, ulong value)
    {
      oprot.WriteI64((long)value);
    }
  }
}
