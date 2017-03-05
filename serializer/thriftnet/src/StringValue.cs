
namespace Thrift.Net
{
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Xml;
  using System.Xml.Linq;
  using Newtonsoft.Json;
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
      return Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadString"));
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
    public string Read(XElement xe)
    {
      return xe.Value;
    }
    public void Write(XElement xe, string value)
    {
      xe.Value = value;
    }

    public string Read(XmlReader reader)
    {
      return reader.ReadElementString();
    }

    public void Write(XmlWriter writer, string value)
    {
      writer.WriteValue(value);
    }

    public string Read(JsonReader reader)
    {
      return reader.Value.ToString();
    }

    public void Write(JsonWriter writer, string value)
    {
      writer.WriteValue(value);
    }
  }
}
