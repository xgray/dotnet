
namespace Thrift.Net
{
  using System;
  using System.Linq.Expressions;
  using System.Reflection;

  using Bench;

  using Thrift.Protocol;

  public class GuidValue : IProtoValue<Guid>
  {
    public TType Type
    {
      get { return TType.String; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.New(
        typeof(Guid).GetConstructor(typeof(byte[])),
        Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadBinary")));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(Guid));

      return Expression.Call(
        oprot,
        typeof(TProtocol).GetMethod("WriteBinary"),
        Expression.Call(value, typeof(Guid).GetMethod("ToByteArray")));
    }

    public Guid Read(TProtocol iprot)
    {
      return new Guid(iprot.ReadBinary());
    }

    public void Write(TProtocol oprot, Guid value)
    {
      oprot.WriteBinary(value.ToByteArray());
    }
  }
}
