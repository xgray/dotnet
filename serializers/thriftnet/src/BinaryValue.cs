
namespace Thrift.Net
{
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Xml;
  using System.Xml.Linq;

  using Bench;
  using Newtonsoft.Json;
  using Thrift.Protocol;

  public class BinaryValue : IProtoValue<byte[]>
  {
    public TType Type
    {
      get { return TType.String; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadBinary"));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(byte[]));
      return Expression.Call(oprot, typeof(TProtocol).GetMethod("WriteBinary"), value);
    }

    public byte[] Read(TProtocol iprot)
    {
      return iprot.ReadBinary();
    }

    public void Write(TProtocol oprot, byte[] value)
    {
      oprot.WriteBinary(value);
    }

    public byte[] Read(XElement xe)
    {
      return CommonUtils.ToBytes(xe.Value);
    }

    public void Write(XElement xe, byte[] value)
    {
      xe.Value = CommonUtils.ToString(value);
    }

    public byte[] Read(XmlReader reader)
    {
      return CommonUtils.ToBytes(reader.ReadElementString());
    }

    public void Write(XmlWriter writer, byte[] value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }

    public byte[] Read(JsonReader reader)
    {
      return CommonUtils.ToBytes(reader.Value.ToString());      
    }

    public void Write(JsonWriter writer, byte[] value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }
  }
}
