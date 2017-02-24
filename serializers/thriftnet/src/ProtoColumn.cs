
namespace Thrift.Net
{
  using System;
  using System.Collections.Generic;
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Xml;
  using System.Xml.Linq;

  using Newtonsoft.Json;
  using Thrift.Protocol;

  public interface IThriftValue
  {
    TType Type { get; }
  }

  public interface IProtoValue : IThriftValue
  {
    Expression Read(Expression iprot);

    Expression Write(Expression oprot, Expression value);
  }

  public interface IProtoValue<V> : IProtoValue
  {
    V Read(TProtocol iprot);
    void Write(TProtocol oprot, V value);

    V Read(XElement xe);
    void Write(XElement xe, V value);

    V Read(XmlReader reader);
    void Write(XmlWriter writer, V value);

    V Read(JsonReader reader);
    void Write(JsonWriter writer, V value);
  }
  public interface IProtoColumn<T>
  {
    PropertyInfo PropertyInfo { get; }

    string Name { get; }

    short ID { get; }

    TType Type { get; }

    // Expression DefaultExpression { get; }

    IProtoValue Value { get; }

    void Read(TProtocol iprot, T proto);
    void Write(TProtocol oprot, T proto);

    void Read(XElement iprot, T proto);
    void Write(XElement oprot, T proto);

    void Read(XmlReader reader, T proto);
    void Write(XmlWriter writer, T proto);

    void Read(JsonReader reader, T proto);
    void Write(JsonWriter writer, T proto);
  }

  /// <summary>
  /// ProtoColumn class
  /// </summary>
  /// <typeparam name="T">proto type</typeparam>
  /// <typeparam name="V">column type</typeparam>
  public class ProtoColumn<T, V> : IProtoColumn<T>
  {
    public ProtoColumn(
      PropertyInfo propertyInfo,
      string name,
      short id,
      IProtoValue<V> valueMetadata,
      // Expression defaultExpression,
      V defaultValue,
      Func<T, V> getter,
      Action<T, V> setter)
    {
      this.PropertyInfo = propertyInfo;
      this.Name = name;
      this.ID = id;
      this.ValueMetadata = valueMetadata;
      // this.DefaultExpression = defaultExpression;
      // this.Default = Expression.Lambda<Func<V>>(defaultExpression).Compile().Invoke();
      this.Default = defaultValue;
      this.Getter = getter;
      this.Setter = setter;
    }

    /// <summary>
    /// Column name. Column names usually match the Properties or fields name in a Proto type.
    /// </summary>
    public string Name { get; private set; }

    public short ID { get; private set; }

    public TType Type
    {
      get { return this.ValueMetadata.Type; }
    }

    public IProtoValue Value
    {
      get { return this.ValueMetadata; }
    }

    public PropertyInfo PropertyInfo { get; private set; }

    /// <summary>
    /// Type of the column value.
    /// </summary>
    public IProtoValue<V> ValueMetadata { get; private set; }

    // public Expression DefaultExpression { get; private set; }

    public V Default { get; private set; }

    public Func<T, V> Getter { get; private set; }

    public Action<T, V> Setter { get; private set; }

    public bool IsDefault(V value)
    {
      return EqualityComparer<V>.Default.Equals(value, this.Default);
    }

    /// <summary>
    /// Read column value from a thrift protocol
    /// </summary>
    /// <param name="proto">binary writer</param>
    /// <param name="iprot">column value</param>
    public void Read(TProtocol iprot, T proto)
    {
      V value = this.ValueMetadata.Read(iprot);
      this.Setter(proto, value);
    }

    /// <summary>
    /// Write column value to a thrift protocol
    /// </summary>
    /// <param name="oprot">binary writer</param>
    /// <param name="proto">column value</param>
    public void Write(TProtocol oprot, T proto)
    {
      V value = this.Getter(proto);
      if (!this.IsDefault(value))
      {
        TField field = new TField();
        field.ID = this.ID;
        field.Type = this.Type;

        oprot.WriteFieldBegin(field);
        this.ValueMetadata.Write(oprot, value);
        oprot.WriteFieldEnd();
      }
    }

    public void Read(XElement xe, T proto)
    {
      V value = this.ValueMetadata.Read(xe);
      this.Setter(proto, value);
    }

    public void Write(XElement xe, T proto)
    {
      V value = this.Getter(proto);
      if (!this.IsDefault(value))
      {
        XElement fe = new XElement(this.Name);
        this.ValueMetadata.Write(fe, value);
        xe.Add(fe);
      }
    }

    public void Read(XmlReader reader, T proto)
    {
      V value = this.ValueMetadata.Read(reader);
      this.Setter(proto, value);
    }

    public void Write(XmlWriter writer, T proto)
    {
      V value = this.Getter(proto);
      if (!this.IsDefault(value))
      {
        writer.WriteStartElement(this.Name);
        this.ValueMetadata.Write(writer, value);
        writer.WriteEndElement();
      }
    }

    public void Read(JsonReader reader, T proto)
    {
      reader.Read();
      V value = this.ValueMetadata.Read(reader);
      this.Setter(proto, value);
    }

    public void Write(JsonWriter writer, T proto)
    {
      V value = this.Getter(proto);
      if (!this.IsDefault(value))
      {
        writer.WritePropertyName(this.Name);
        this.ValueMetadata.Write(writer, value);
      }
    }
  }
}
