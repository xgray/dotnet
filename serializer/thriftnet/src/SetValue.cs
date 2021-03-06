﻿
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

  public interface IThriftSetValue
  {
    TType ValueType { get; }
  }

  public class SetValue<V> : IProtoValue<HashSet<V>>, IThriftSetValue
  {
    public SetValue(IProtoValue<V> valueMetadata)
    {
      this.ValueMetadata = valueMetadata;
    }

    public IProtoValue<V> ValueMetadata { get; private set; }

    public TType Type
    {
      get { return TType.Set; }
    }

    TType IThriftSetValue.ValueType
    {
      get { return this.ValueMetadata.Type; }
    }

    public Expression Read(Expression iprot)
    {
      ParameterExpression tSet = Expression.Variable(typeof(TSet));
      Expression count = Expression.Property(tSet, typeof(TSet).GetProperty("Count"));
      ParameterExpression value = Expression.Variable(typeof(HashSet<V>));
      ParameterExpression i = Expression.Variable(typeof(int));
      LabelTarget endLoop = Expression.Label();

      return Expression.Block(
        new[] { tSet, value, i },
        Expression.Assign(
          tSet,
          Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadSetBegin"))
        ),
        Expression.Assign(
          value,
          Expression.New(typeof(HashSet<V>).GetConstructor())
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
              typeof(HashSet<V>).GetMethod("Add"),
              this.ValueMetadata.Read(iprot)
            ),
            Expression.AddAssign(i, Expression.Constant(1))
          )
        ),
        Expression.Label(endLoop),
        Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadSetEnd")),
        value
      );
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(HashSet<V>));
      MethodInfo getEnum = typeof(HashSet<V>).GetMethod("GetEnumerator");

      Expression count = Expression.Property(value, "Count");
      ParameterExpression i = Expression.Variable(typeof(HashSet<V>.Enumerator));
      LabelTarget endLoop = Expression.Label();

      return Expression.Block(
        new[] { i },
        Expression.Call(
          oprot,
          typeof(TProtocol).GetMethod("WriteSetBegin"),
          Expression.New(
            typeof(TSet).GetConstructor(typeof(TType), typeof(int)),
            Expression.Constant(this.ValueMetadata.Type),
            count
          )
        ),
        Expression.Assign(i, Expression.Call(value, getEnum)),
        Expression.Loop(
          Expression.Block(
            Expression.IfThen(
              Expression.IsFalse(
                Expression.Call(i, typeof(HashSet<V>.Enumerator).GetMethod("MoveNext"))
              ),
              Expression.Goto(endLoop)
            ),
            this.ValueMetadata.Write(oprot, Expression.Property(i, "Current"))
          )
        ),
        Expression.Label(endLoop),
        Expression.Call(oprot, typeof(TProtocol).GetMethod("WriteSetEnd"))
      );
    }

    public HashSet<V> Read(TProtocol iprot)
    {
      TSet tSet = iprot.ReadSetBegin();
      HashSet<V> value = new HashSet<V>();
      for (int i = 0; i < tSet.Count; ++i)
      {
        V element = this.ValueMetadata.Read(iprot);
        value.Add(element);
      }
      iprot.ReadSetEnd();
      return value;
    }

    public void Write(TProtocol oprot, HashSet<V> value)
    {
      TSet tSet = new TSet(this.ValueMetadata.Type, value.Count);
      oprot.WriteSetBegin(tSet);
      foreach (V item in value)
      {
        this.ValueMetadata.Write(oprot, item);
      }
      oprot.WriteSetEnd();
    }

    public HashSet<V> Read(XElement xe)
    {
      HashSet<V> set = new HashSet<V>();
      foreach (XElement ce in xe.Elements())
      {
        set.Add(this.ValueMetadata.Read(ce));
      }
      return set;
    }

    public void Write(XElement xe, HashSet<V> value)
    {
      IStructValue structValue = this.ValueMetadata as IStructValue;
      string itemTag = structValue?.StructType.Name ?? "Item";

      foreach (V item in value)
      {
        XElement ce = new XElement(itemTag);
        this.ValueMetadata.Write(ce, item);
        xe.Add(ce);
      }
    }

    public HashSet<V> Read(XmlReader reader)
    {
      HashSet<V> set = new HashSet<V>();
      if (!reader.IsEmptyElement)
      {
        int saved = reader.Depth;
        while (reader.Read() && reader.Depth > saved)
        {
          set.Add(this.ValueMetadata.Read(reader));
        }
      }
      return set;
    }

    public void Write(XmlWriter writer, HashSet<V> value)
    {
      IStructValue structValue = this.ValueMetadata as IStructValue;
      string itemTag = structValue?.StructType.Name ?? "Item";

      foreach (V item in value)
      {
        writer.WriteStartElement(itemTag);
        this.ValueMetadata.Write(writer, item);
        writer.WriteEndElement();
      }
    }

    public HashSet<V> Read(JsonReader reader)
    {
      HashSet<V> set = new HashSet<V>();
      while (reader.Read() && reader.TokenType != JsonToken.EndArray)
      {
        set.Add(this.ValueMetadata.Read(reader));
      }
      return set;
    }

    public void Write(JsonWriter writer, HashSet<V> value)
    {
      writer.WriteStartArray();
      foreach (V item in value)
      {
        this.ValueMetadata.Write(writer, item);
      }
      writer.WriteEndArray();
    }
  }
}
