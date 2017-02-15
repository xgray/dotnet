
namespace Thrift.Net
{
  using System.Linq.Expressions;
  using System.Reflection;

  using Bench;

  using Thrift.Protocol;

  public class StructValue<T> : IProtoValue<T> where T : new()
  {
    public TType Type
    {
      get { return TType.Struct; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.Call(
        typeof(Proto<T>).GetMethod("Read", new[] {typeof(TProtocol)}),
        iprot);
    }
    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(T));

      return Expression.Call(
        typeof(Proto<T>).GetMethod("Write", new[] {typeof(TProtocol), typeof(T)}),
        oprot,
        value);
    }

    public T Read(TProtocol iprot)
    {
      return Proto<T>.Read(iprot);
    }

    public void Write(TProtocol oprot, T value)
    {
      Proto<T>.Write(oprot, value);
    }
  }
}
