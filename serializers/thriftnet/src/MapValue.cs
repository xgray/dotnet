
namespace Thrift.Net
{
  using System.Collections.Generic;
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Xml;
  using System.Xml.Linq;

  using Bench;
  using Thrift.Protocol;

  public interface IThriftMapValue : IThriftValue
  {
    TType KeyType { get; }

    TType ValueType { get; }
  }

  public class MapValue<K, V> : IProtoValue<Dictionary<K, V>>, IThriftMapValue
  {
    public MapValue(IProtoValue<K> keyMetadata, IProtoValue<V> valueMetadata)
    {
      this.KeyMetadata = keyMetadata;
      this.ValueMetadata = valueMetadata;
    }

    public IProtoValue<K> KeyMetadata { get; private set; }

    public IProtoValue<V> ValueMetadata { get; private set; }

    public TType Type
    {
      get { return TType.Map; }
    }

    TType IThriftMapValue.KeyType
    {
      get { return this.KeyMetadata.Type; }
    }

    TType IThriftMapValue.ValueType
    {
      get { return this.ValueMetadata.Type; }
    }

    public Expression Read(Expression iprot)
    {
      ParameterExpression tMap = Expression.Variable(typeof(TMap));
      Expression count = Expression.Property(tMap, typeof(TMap).GetProperty("Count"));
      ParameterExpression value = Expression.Variable(typeof(Dictionary<K, V>));
      ParameterExpression i = Expression.Variable(typeof(int));
      LabelTarget endLoop = Expression.Label();

      return Expression.Block(
        new[] { tMap, value, i },
        Expression.Assign(
          tMap,
          Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadMapBegin"))
        ),
        Expression.Assign(
          value,
          Expression.New(typeof(Dictionary<K, V>).GetConstructor(typeof(int)), count)
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
              typeof(Dictionary<K, V>).GetMethod("Add"),
              this.KeyMetadata.Read(iprot),
              this.ValueMetadata.Read(iprot)
            ),
            Expression.AddAssign(i, Expression.Constant(1))
          )
        ),
        Expression.Label(endLoop),
        Expression.Call(iprot, typeof(TProtocol).GetMethod("ReadMapEnd")),
        value
      );
    }

    public Expression Write(Expression oprot, Expression value)
    {
      CommonUtils.ThrowIfFalse(value.Type == typeof(Dictionary<K, V>));

      MethodInfo getEnum = typeof(Dictionary<K, V>.KeyCollection).GetMethod("GetEnumerator");

      Expression count = Expression.Property(value, "Count");
      ParameterExpression i = Expression.Variable(typeof(Dictionary<K, V>.KeyCollection.Enumerator));
      ParameterExpression k = Expression.Variable(typeof(K));
      LabelTarget endLoop = Expression.Label();

      return Expression.Block(
        new[] { i, k },
        Expression.Call(
          oprot,
          typeof(TProtocol).GetMethod("WriteMapBegin"),
          Expression.New(
            typeof(TMap).GetConstructor(typeof(TType), typeof(TType), typeof(int)),
            Expression.Constant(this.KeyMetadata.Type),
            Expression.Constant(this.ValueMetadata.Type),
            count
          )
        ),
        Expression.Assign(
          i,
          Expression.Call(Expression.Property(value, "Keys"), getEnum)
        ),
        Expression.Loop(
          Expression.Block(
            Expression.IfThen(
              Expression.IsFalse(
                // Expression.Constant(true)
                Expression.Call(i, typeof(Dictionary<K, V>.KeyCollection.Enumerator).GetMethod("MoveNext"))
              ),
              Expression.Goto(endLoop)
            ),
            Expression.Assign(k, Expression.Property(i, "Current")),
            this.KeyMetadata.Write(oprot, k),
            this.ValueMetadata.Write(
              oprot,
              Expression.MakeIndex(value, typeof(Dictionary<K, V>).GetProperty("Item"), new[] { k })
            )
          )
        ),
        Expression.Label(endLoop),
        Expression.Call(oprot, typeof(TProtocol).GetMethod("WriteMapEnd"))
      );
    }

    public Dictionary<K, V> Read(TProtocol iprot)
    {
      TMap tMap = iprot.ReadMapBegin();
      Dictionary<K, V> value = new Dictionary<K, V>();

      for (int i = 0; i < tMap.Count; ++i)
      {
        K k = this.KeyMetadata.Read(iprot);
        V v = this.ValueMetadata.Read(iprot);
        value[k] = v;
      }
      iprot.ReadMapEnd();
      return value;
    }

    public void Write(TProtocol oprot, Dictionary<K, V> value)
    {
      TMap tMap = new TMap(this.KeyMetadata.Type, this.ValueMetadata.Type, value.Count);
      oprot.WriteMapBegin(tMap);
      foreach (K k in value.Keys)
      {
        this.KeyMetadata.Write(oprot, k);
        this.ValueMetadata.Write(oprot, value[k]);
      }
      oprot.WriteMapEnd();
    }

    public Dictionary<K, V> Read(XElement xe)
    {
      Dictionary<K, V> dict = new Dictionary<K, V>();

      bool isKey = true;
      K key = default(K);
      V value = default(V);

      foreach (XElement ce in xe.Elements())
      {
        if (isKey)
        {
          key = this.KeyMetadata.Read(ce);
        }
        else
        {
          value = this.ValueMetadata.Read(ce);
          dict.Add(key, value);
        }
        isKey = !isKey;
      }
      return dict;
    }

    public void Write(XElement xe, Dictionary<K, V> value)
    {
      foreach (K key in value.Keys)
      {
        XElement ke = new XElement("Key");
        this.KeyMetadata.Write(ke, key);
        xe.Add(ke);
        XElement ve = new XElement("Value");
        this.ValueMetadata.Write(ve, value[key]);
        xe.Add(ve);
      }
    }

    public Dictionary<K, V> Read(XmlReader reader)
    {
      Dictionary<K, V> dict = new Dictionary<K, V>();
      if (!reader.IsEmptyElement)
      {
        bool isKey = true;
        K key = default(K);
        V value = default(V);
        int saved = reader.Depth;
        while (reader.Read() && reader.Depth > saved)
        {
          if (isKey)
          {
            key = this.KeyMetadata.Read(reader);
          }
          else
          {
            value = this.ValueMetadata.Read(reader);
            dict.Add(key, value);
          }
          isKey = !isKey;
        }
      }

      return dict;
    }

    public void Write(XmlWriter writer, Dictionary<K, V> value)
    {
      foreach (K key in value.Keys)
      {
        writer.WriteStartElement("Key");
        this.KeyMetadata.Write(writer, key);
        writer.WriteEndElement();
        writer.WriteStartElement("Value");
        this.ValueMetadata.Write(writer, value[key]);
        writer.WriteEndElement();
      }
    }
  }
}
