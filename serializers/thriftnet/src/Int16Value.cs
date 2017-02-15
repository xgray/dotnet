
namespace Thrift.Net
{
  using System.Linq.Expressions;
  using System.Reflection;
  
  using Bench;

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
  }
}
