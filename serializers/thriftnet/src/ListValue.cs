
namespace Thrift.Net
{
  using System;
  using System.Collections.Generic;
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Xml;
  using System.Xml.Linq;

  using Bench;
  using Newtonsoft.Json;
  using Thrift.Protocol;

  public interface IThriftListValue : IThriftValue
  {
    TType ValueType { get; }
  }

  public class ListValue<V> : IProtoValue<List<V>>, IThriftListValue
  {
    public ListValue(IProtoValue<V> valueMetadata)
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
      ParameterExpression value = Expression.Variable(typeof(List<V>));
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
          Expression.New(typeof(List<V>).GetConstructor(typeof(int)), count)
        ),
        i,
        Expression.Assign(i, Expression.Constant(0)),
        Expression.Loop(
          Expression.Block(
            Expression.IfThen(
              Expression.GreaterThanOrEqual(i, count),
              Expression.Goto(endLoop)
            ),
            Expression.Call(
              value,
              typeof(List<V>).GetMethod("Add"),
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
      CommonUtils.ThrowIfFalse(value.Type == typeof(List<V>));

      Expression count = Expression.Property(value, "Count");
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
              Expression.MakeIndex(value, typeof(List<V>).GetProperty("Item"), new[] { i })
            ),
            Expression.AddAssign(i, Expression.Constant(1))
          )
        ),
        Expression.Label(endLoop),
        Expression.Call(oprot, typeof(TProtocol).GetMethod("WriteListEnd"))
      );
    }

    public List<V> Read(TProtocol iprot)
    {
      TList tList = iprot.ReadListBegin();
      List<V> value = new List<V>(tList.Count);
      for (int i = 0; i < tList.Count; ++i)
      {
        V element = this.ValueMetadata.Read(iprot);
        value.Add(element);
      }
      iprot.ReadListEnd();
      return value;
    }

    public void Write(TProtocol oprot, List<V> value)
    {
      TList tList = new TList(this.ValueMetadata.Type, value.Count);
      oprot.WriteListBegin(tList);
      for (int i = 0; i < value.Count; i++)
      {
        this.ValueMetadata.Write(oprot, value[i]);
      }
      oprot.WriteListEnd();
    }

    public List<V> Read(XElement xe)
    {
      List<V> list = new List<V>();
      foreach (XElement ce in xe.Elements())
      {
        list.Add(this.ValueMetadata.Read(ce));
      }
      return list;
    }

    public void Write(XElement xe, List<V> value)
    {
      for (int i = 0; i < value.Count; i++)
      {
        XElement ce = new XElement("Item");
        this.ValueMetadata.Write(ce, value[i]);
        xe.Add(ce);
      }
    }

    public List<V> Read(XmlReader reader)
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
      return list;
    }

    public void Write(XmlWriter writer, List<V> value)
    {
      for (int i = 0; i < value.Count; i++)
      {
        writer.WriteStartElement("Item");
        this.ValueMetadata.Write(writer, value[i]);
        writer.WriteEndElement();
      }
    }

    public List<V> Read(JsonReader reader)
    {
      List<V> list = new List<V>();
      while (reader.Read() && reader.TokenType != JsonToken.EndArray)
      {
        list.Add(this.ValueMetadata.Read(reader));
      }
      return list;
    }

    public void Write(JsonWriter writer, List<V> value)
    {
      writer.WriteStartArray();
      for (int i = 0; i < value.Count; i++)
      {
        this.ValueMetadata.Write(writer, value[i]);
      }
      writer.WriteEndArray();
    }
  }
}
