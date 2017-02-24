
namespace Thrift.Net
{
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Xml;
  using System.Xml.Linq;
  
  using Bench;
  using Newtonsoft.Json;
  using Thrift.Protocol;

  public class Int64Value : IProtoValue<long>
  {
    public TType Type
    {
      get { return TType.I64; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.Call(iprot,typeof(TProtocol).GetMethod("ReadI64"));
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(long));
      return Expression.Call(oprot, typeof(TProtocol).GetMethod("WriteI64"), value);
    }

    public long Read(TProtocol iprot)
    {
      return iprot.ReadI64();
    }

    public void Write(TProtocol oprot, long value)
    {
      oprot.WriteI64(value);
    }
    public long Read(XElement xe)
    {
      return CommonUtils.ToInt64(xe.Value);
    }
    public void Write(XElement xe, long value)
    {
      xe.Value = CommonUtils.ToString(value);
    }

    public long Read(XmlReader reader)
    {
      return CommonUtils.ToInt64(reader.ReadElementString());
    }

    public void Write(XmlWriter writer, long value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }

    public long Read(JsonReader reader)
    {
      return CommonUtils.ToInt64(reader.Value.ToString());      
    }

    public void Write(JsonWriter writer, long value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }
  }
}
