
namespace ProtoThrift
{
  using System.Linq.Expressions;
  using System.Reflection;
  
  using Bench;

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
  }
}
