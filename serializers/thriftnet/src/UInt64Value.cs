
namespace Thrift.Net
{
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Xml;
  using System.Xml.Linq;
  
  using Bench;

  using Thrift.Protocol;

  public class UInt64Value : IProtoValue<ulong>
  {
    public TType Type
    {
      get { return TType.I64; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.Convert(
        Expression.Call(iprot,typeof(TProtocol).GetMethod("ReadI64")),
        typeof(ulong));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(ulong));

      return Expression.Call(
        oprot,
        typeof(TProtocol).GetMethod("WriteI64"),
        Expression.Convert(value, typeof(long)));
    }

    public ulong Read(TProtocol iprot)
    {
      return (ulong)iprot.ReadI64();
    }

    public void Write(TProtocol oprot, ulong value)
    {
      oprot.WriteI64((long)value);
    }

    public ulong Read(XElement xe)
    {
      return CommonUtils.ToUInt64(xe.Value);
    }
    public void Write(XElement xe, ulong value)
    {
      xe.Value = CommonUtils.ToString(value);
    }

    public ulong Read(XmlReader reader)
    {
      return CommonUtils.ToUInt64(reader.ReadElementString());
    }

    public void Write(XmlWriter writer, ulong value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }    
  }
}
