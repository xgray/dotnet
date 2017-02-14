
namespace ProtoThrift
{
  using System.Linq.Expressions;
  using System.Reflection;

  using Bench;
  using Thrift.Protocol;

  public class BinaryValue : IProtoValue<byte[]>
  {
    public TType Type
    {
      get { return TType.String; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadBinary"));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(byte[]));
      return Expression.Call(oprot, typeof(TProtocol).GetMethod("WriteBinary"), value);
    }

    public byte[] Read(TProtocol iprot)
    {
      return iprot.ReadBinary();
    }

    public void Write(TProtocol oprot, byte[] value)
    {
      oprot.WriteBinary(value);
    }
  }
}
