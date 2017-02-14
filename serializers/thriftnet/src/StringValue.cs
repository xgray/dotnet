
namespace ProtoThrift
{
  using System.Linq.Expressions;
  using System.Reflection;
  
  using Bench;

  using Thrift.Protocol;

  public class StringValue : IProtoValue<string>
  {
    public TType Type
    {
      get { return TType.String; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.Call(iprot,typeof(TProtocol).GetMethod("ReadString"));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(string));
      return Expression.Call(oprot, typeof(TProtocol).GetMethod("WriteString"), value);
    }

    public string Read(TProtocol iprot)
    {
      return iprot.ReadString();
    }

    public void Write(TProtocol oprot, string value)
    {
      oprot.WriteString(value);
    }
  }
}
