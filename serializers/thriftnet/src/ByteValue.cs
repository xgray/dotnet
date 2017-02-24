
namespace Thrift.Net
{
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Xml;
  using System.Xml.Linq;

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
    public byte Read(XElement xe)
    {
      return CommonUtils.ToByte(xe.Value);
    }

    public void Write(XElement xe, byte value)
    {
      xe.Value = CommonUtils.ToString(value);
    }

    public byte Read(XmlReader reader)
    {
      return CommonUtils.ToByte(reader.ReadElementString());
    }

    public void Write(XmlWriter writer, byte value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }
  }
}
