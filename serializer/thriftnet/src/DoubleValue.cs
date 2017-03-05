
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

    public double Read(XElement xe)
    {
      return CommonUtils.ToDouble(xe.Value);
    }
    public void Write(XElement xe, double value)
    {
      xe.Value = CommonUtils.ToString(value);
    }

    public double Read(XmlReader reader)
    {
      return CommonUtils.ToDouble(reader.ReadElementString());
    }

    public void Write(XmlWriter writer, double value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }
    
    public double Read(JsonReader reader)
    {
      return CommonUtils.ToDouble(reader.Value.ToString());      
    }

    public void Write(JsonWriter writer, double value)
    {
      writer.WriteValue(CommonUtils.ToString(value));
    }
  }
}
