
namespace Thrift.Net
{
  using System.Collections.Generic;
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Xml;
  using System.Xml.Linq;

  using Bench;
  using Newtonsoft.Json;
  using Thrift.Protocol;

  public class ArrayValue<V> : IProtoValue<V[]>, IThriftListValue
  {
    public ArrayValue(IProtoValue<V> valueMetadata)
    {
      this.ValueMetadata = valueMetadata;
    }

    public IProtoValue<V> ValueMetadata { get; private set; }

    public TType Type
    {
      get { return TType.List; }
    }

    TType IThriftListValue.ValueType
    {
      get { return this.ValueMetadata.Type; }
    }

    public Expression Read(Expression iprot)
    {
      ParameterExpression tList = Expression.Variable(typeof(TList));
      Expression count = Expression.Property(tList, typeof(TList).GetProperty("Count"));
      ParameterExpression value = Expression.Variable(typeof(V[]));
      ParameterExpression i = Expression.Variable(typeof(int));
      LabelTarget endLoop = Expression.Label();

      return Expression.Block(
        new[] { tList, value, i },
        Expression.Assign(
          tList,
          Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadListBegin"))
        ),
        Expression.Assign(
          value,
          Expression.New(typeof(V[]).GetConstructor(typeof(int)), count)
        ),
        Expression.Assign(i, Expression.Constant(0)),
        Expression.Loop(
          Expression.Block(
            Expression.IfThen(
              Expression.GreaterThanOrEqual(i, count),
              Expression.Goto(endLoop)
            ),
            Expression.Assign(
              Expression.ArrayAccess(value, i),
              this.ValueMetadata.Read(iprot)
            ),
            Expression.AddAssign(i, Expression.Constant(1))
          )
        ),
        Expression.Label(endLoop),
        Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadListEnd")),
        value
      );
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(V[]));

      Expression count = Expression.Property(value, "Length");
      ParameterExpression i = Expression.Variable(typeof(int));
      LabelTarget endLoop = Expression.Label();

      return Expression.Block(
        new[] { i },
        Expression.Call(
          oprot,
          typeof(TProtocol).GetMethod("WriteListBegin"),
          Expression.New(
            typeof(TList).GetConstructor(typeof(TType), typeof(int)),
            Expression.Constant(this.ValueMetadata.Type),
            count
          )
        ),
        Expression.Assign(i, Expression.Constant(0)),
        Expression.Loop(
          Expression.Block(
            Expression.IfThen(
              Expression.GreaterThanOrEqual(i, count),
              Expression.Goto(endLoop)
            ),
            this.ValueMetadata.Write(
              oprot,
              Expression.ArrayIndex(value, i)
            ),
            Expression.AddAssign(i, Expression.Constant(1))
          )
        ),
        Expression.Label(endLoop),
        Expression.Call(oprot, typeof(TProtocol).GetMethod("WriteListEnd"))
      );
    }

    public V[] Read(TProtocol iprot)
    {
      TList tList = iprot.ReadListBegin();
      V[] value = new V[tList.Count];
      for (int i = 0; i < tList.Count; ++i)
      {
        V element = this.ValueMetadata.Read(iprot);
        value[i] = element;
      }
      iprot.ReadListEnd();
      return value;
    }

    public void Write(TProtocol oprot, V[] value)
    {
      TList tList = new TList(this.ValueMetadata.Type, value.Length);
      oprot.WriteListBegin(tList);
      for (int i = 0; i < value.Length; i++)
      {
        this.ValueMetadata.Write(oprot, value[i]);
      }
      oprot.WriteListEnd();
    }

    public V[] Read(XElement xe)
    {
      List<V> list = new List<V>();
      foreach (XElement ce in xe.Elements())
      {
        list.Add(this.ValueMetadata.Read(ce));
      }
      return list.ToArray();
    }

    public void Write(XElement xe, V[] value)
    {
      for (int i = 0; i < value.Length; i++)
      {
        XElement ce = new XElement("Item");
        this.ValueMetadata.Write(ce, value[i]);
        xe.Add(ce);
      }
    }

    public V[] Read(XmlReader reader)
    {
      List<V> list = new List<V>();
      if (!reader.IsEmptyElement)
      {
        int saved = reader.Depth;
        while (reader.Read() && reader.Depth > saved)
        {
          list.Add(this.ValueMetadata.Read(reader));
        }
      }
      return list.ToArray();
    }

    public void Write(XmlWriter writer, V[] value)
    {
      for (int i = 0; i < value.Length; i++)
      {
        writer.WriteStartElement("Item");
        this.ValueMetadata.Write(writer, value[i]);
        writer.WriteEndElement();
      }
    }

    public V[] Read(JsonReader reader)
    {
      List<V> list = new List<V>();
      while (reader.Read() && reader.TokenType != JsonToken.EndArray)
      {
        list.Add(this.ValueMetadata.Read(reader));
      }
      return list.ToArray();
    }

    public void Write(JsonWriter writer, V[] value)
    {
      writer.WriteStartArray();
      for (int i = 0; i < value.Length; i++)
      {
        this.ValueMetadata.Write(writer, value[i]);
      }
      writer.WriteEndArray();
    }
  }
}
