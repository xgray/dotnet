
namespace ProtoThrift
{
  using System.Linq.Expressions;
  using System.Reflection;

  using Bench;

  using Thrift.Protocol;

  public class ByteValue : IProtoValue<byte>
  {
    public TType Type
    {
      get { return TType.Byte; }
    }
    public Expression Read(Expression iprot)
    {
      return Expression.Convert(
        Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadByte")),
        typeof(byte));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(byte));

      return Expression.Call(
        oprot,
        typeof(TProtocol).GetMethod("WriteByte"),
        Expression.Convert(value, typeof(sbyte)));
    }

    public byte Read(TProtocol iprot)
    {
      return (byte)iprot.ReadByte();
    }

    public void Write(TProtocol oprot, byte value)
    {
      oprot.WriteByte((sbyte)value);
    }
  }
}
