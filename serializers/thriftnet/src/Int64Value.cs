
namespace Thrift.Net
{
  using System.Linq.Expressions;
  using System.Reflection;
  
  using Bench;

  using Thrift.Protocol;

  public class Int64Value : IProtoValue<long>
  {
    public TType Type
    {
      get { return TType.I64; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.Call(iprot,typeof(TProtocol).GetMethod("ReadI64"));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(long));
      return Expression.Call(oprot, typeof(TProtocol).GetMethod("WriteI64"), value);
    }

    public long Read(TProtocol iprot)
    {
      return iprot.ReadI64();
    }

    public void Write(TProtocol oprot, long value)
    {
      oprot.WriteI64(value);
    }
  }
}
