
namespace ProtoThrift
{
  using System.Linq.Expressions;
  using System.Reflection;
  
  using Bench;

  using Thrift.Protocol;

  public class UInt32Value : IProtoValue<uint>
  {
    public TType Type
    {
      get { return TType.I32; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.Convert(
        Expression.Call(iprot,typeof(TProtocol).GetMethod("ReadI32")),
        typeof(uint));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(uint));

      return Expression.Call(
        oprot,
        typeof(TProtocol).GetMethod("WriteI32"),
        Expression.Convert(value, typeof(int)));
    }

    public uint Read(TProtocol iprot)
    {
      return (uint)iprot.ReadI32();
    }

    public void Write(TProtocol oprot, uint value)
    {
      oprot.WriteI32((int)value);
    }
  }
}
