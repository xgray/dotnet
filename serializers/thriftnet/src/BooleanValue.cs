
namespace Thrift.Net
{
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Xml;
  using System.Xml.Linq;

  using Bench;
  using Newtonsoft.Json;
  using Thrift.Protocol;

  /// <summary>
  /// BooleanColumn Type
  /// </summary>
  public class BooleanValue : IProtoValue<bool>
  {
    public TType Type
    {
      get { return TType.Bool; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadBool"));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(bool));
      return Expression.Call(oprot, typeof(TProtocol).GetMethod("WriteBool"), value);
    }

    public bool Read(TProtocol iprot)
    {
      return iprot.ReadBool();
    }

    public void Write(TProtocol oprot, bool value)
    {
      oprot.WriteBool(value);
    }

    public bool Read(XElement xe)
    {
      return CommonUtils.ToBoolean(xe.Value);
    }

    public void Write(XElement xe, bool value)
    {
      xe.Value = CommonUtils.ToString(value);
    }

    public bool Read(XmlReader reader)
    {
      return CommonUtils.ToBoolean(reader.ReadElementString());
    }

    public void Write(XmlWriter writer, bool value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }

    public bool Read(JsonReader reader)
    {
      return CommonUtils.ToBoolean(reader.Value.ToString());      
    }

    public void Write(JsonWriter writer, bool value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }
  }
}
