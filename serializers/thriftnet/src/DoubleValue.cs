
namespace Thrift.Net
{
  using System.Linq.Expressions;
  using System.Reflection;
  
  using Bench;

  using Thrift.Protocol;

  /// <summary>
  /// BooleanColumn Type
  /// </summary>
  public class DoubleValue : IProtoValue<double>
  {
    public TType Type
    {
      get { return TType.Double; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.Call(iprot,typeof(TProtocol).GetMethod("ReadDouble"));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(double));
      return Expression.Call(oprot, typeof(TProtocol).GetMethod("WriteDouble"), value);
    }

    public double Read(TProtocol iprot)
    {
      return iprot.ReadDouble();
    }

    public void Write(TProtocol oprot, double value)
    {
      oprot.WriteDouble(value);
    }
  }
}
