
namespace Thrift.Net
{
  using System;
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Xml;
  using System.Xml.Linq;

  using Bench;

  using Thrift.Protocol;

  public class GuidValue : IProtoValue<Guid>
  {
    public TType Type
    {
      get { return TType.String; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.New(
        typeof(Guid).GetConstructor(typeof(byte[])),
        Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadBinary")));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(Guid));

      return Expression.Call(
        oprot,
        typeof(TProtocol).GetMethod("WriteBinary"),
        Expression.Call(value, typeof(Guid).GetMethod("ToByteArray")));
    }

    public Guid Read(TProtocol iprot)
    {
      return new Guid(iprot.ReadBinary());
    }

    public void Write(TProtocol oprot, Guid value)
    {
      oprot.WriteBinary(value.ToByteArray());
    }

    public Guid Read(XElement xe)
    {
      return Guid.Parse(xe.Value);
    }

    public void Write(XElement xe, Guid value)
    {
      xe.Value = value.ToString("N");
    }

    public Guid Read(XmlReader reader)
    {
      return Guid.Parse(reader.ReadElementString());
    }

    public void Write(XmlWriter writer, Guid value)
    {
      writer.WriteValue(value.ToString("N"));
    }
  }
}
