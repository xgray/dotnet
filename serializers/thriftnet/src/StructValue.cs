
namespace Thrift.Net
{
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Xml;
  using System.Xml.Linq;

  using Bench;
  using Newtonsoft.Json;
  using Thrift.Protocol;

  public class StructValue<T> : IProtoValue<T> where T : new()
  {
    public TType Type
    {
      get { return TType.Struct; }
    }

    public Expression Read(Expression iprot)
    {
      return Expression.Call(
        typeof(Proto<T>).GetMethod("Read", new[] {typeof(TProtocol)}),
        iprot);
    }
    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(T));

      return Expression.Call(
        typeof(Proto<T>).GetMethod("Write", new[] {typeof(TProtocol), typeof(T)}),
        oprot,
        value);
    }

    public T Read(TProtocol iprot)
    {
      return Proto<T>.Read(iprot);
    }

    public void Write(TProtocol oprot, T value)
    {
      Proto<T>.Write(oprot, value);
    }

    public T Read(XElement xe)
    {
      return Proto<T>.Read(xe);
    }

    public void Write(XElement xe, T value)
    {
      Proto<T>.Write(xe, value);
    }

    public T Read(XmlReader reader)
    {
      return Proto<T>.Read(reader);
    }

    public void Write(XmlWriter writer, T value)
    {
      Proto<T>.Write(writer, value);
    }    

    public T Read(JsonReader reader)
    {
      return Proto<T>.Read(reader);
    }

    public void Write(JsonWriter writer, T value)
    {
      Proto<T>.Write(writer, value);
    }    
  }
}
