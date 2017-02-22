
namespace Thrift.Net
{
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Xml.Linq;
  
  using Bench;

  using Thrift.Protocol;

  public class UInt16Value : IProtoValue<ushort>
  {
    public TType Type
    {
      get { return TType.I16; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.Convert(
        Expression.Call(iprot,typeof(TProtocol).GetMethod("ReadI16")),
        typeof(ushort));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(ushort));

      return Expression.Call(
        oprot,
        typeof(TProtocol).GetMethod("WriteI16"),
        Expression.Convert(value, typeof(short)));
    }

    public ushort Read(TProtocol iprot)
    {
      return (ushort)iprot.ReadI16();
    }

    public void Write(TProtocol oprot, ushort value)
    {
      oprot.WriteI16((short)value);
    }
    public ushort Read(XElement xe)
    {
      return CommonUtils.ToUInt16(xe.Value);
    }
    public void Write(XElement xe, ushort value)
    {
      xe.Value = CommonUtils.ToString(value);
    }
  }
}
