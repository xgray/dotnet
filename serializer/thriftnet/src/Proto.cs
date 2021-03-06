﻿
namespace Thrift.Net
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Text;
  using System.Xml;
  using System.Xml.Linq;

  using Bench;

  using Newtonsoft.Json;
  using Thrift.Protocol;
  using Thrift.Transport;

  public sealed class Proto<T> where T : new()
  {
    private static readonly Type protType = typeof(TProtocol);

    private static readonly Type protUtilType = typeof(TProtocolUtil);

    private static Lazy<Proto<T>> instance = new Lazy<Proto<T>>(Initialize);

    private IProtoColumn<T>[] columns;

    private IDictionary<string, IProtoColumn<T>> metadata;

    private Action<TProtocol, T> reader;

    private Action<TProtocol, T> writer;

    /// <summary>
    /// Private constructor
    /// </summary>
    private Proto(IProtoColumn<T>[] columns)
    {
      this.columns = columns;
      this.metadata = columns.Where(c => c != null).ToDictionary(c => c.Name);
    }

    public static void Validate()
    {
      System.Diagnostics.Debug.Assert(instance.Value != null);
    }

    public static string ToXml(T proto)
    {
      XmlWriterSettings writerSettings = new XmlWriterSettings
      {
        Indent = true,
        IndentChars = "  ",
        OmitXmlDeclaration = true,
      };

      using (MemoryStream stream = new MemoryStream())
      {
        using (XmlWriter writer = XmlWriter.Create(stream, writerSettings))
        {
          writer.WriteStartElement(typeof(T).Name);
          Proto<T>.Write(writer, proto);
          writer.WriteEndElement();
          writer.Flush();
        }
        return Encoding.UTF8.GetString(stream.ToArray());
      }
    }

    public static T FromXml(string xml)
    {
      XmlReaderSettings readerSettings = new XmlReaderSettings
      {
        IgnoreProcessingInstructions = true,
        IgnoreWhitespace = true,
        IgnoreComments = true,
      };

      MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
      XmlReader reader = XmlReader.Create(stream, readerSettings);
      reader.Read();

      return Proto<T>.Read(reader);
    }

    public static XElement ToXDoc(T proto)
    {
      XElement xe = new XElement(typeof(T).Name);
      Proto<T>.Write(xe, proto);
      return xe;
    }

    public static T FromXDoc(XElement xe)
    {
      // XElement xe = XElement.Load(new StringReader(xml));
      return Proto<T>.Read(xe);
    }

    public static string ToJson(T proto)
    {
      using (StringWriter stringWriter = new StringWriter())
      {
        JsonWriter writer = new JsonTextWriter(stringWriter);
        writer.Formatting = Newtonsoft.Json.Formatting.Indented;
        Proto<T>.Write(writer, proto);
        return stringWriter.ToString();
      }
    }

    public static T FromJson(string json)
    {
      using (StringReader stringReader = new StringReader(json))
      {
        JsonReader reader = new JsonTextReader(stringReader);
        reader.DateParseHandling = DateParseHandling.None;
        reader.Read();
        return Proto<T>.Read(reader);
      }
    }
    public static T Read(XElement xe)
    {
      T proto = new T();
      IDictionary<string, IProtoColumn<T>> metadata = instance.Value.metadata;
      foreach (XElement ce in xe.Elements())
      {
        IProtoColumn<T> column;
        if (metadata.TryGetValue(ce.Name.LocalName, out column))
        {
          column.Read(ce, proto);
        }
      }
      return proto;
    }

    public static void Write(XElement xe, T proto)
    {
      IProtoColumn<T>[] columns = instance.Value.columns;
      for (int index = 0; index < columns.Length; index++)
      {
        IProtoColumn<T> column = columns[index];
        if (column != null)
        {
          column.Write(xe, proto);
        }
      }
    }

    public static T Read(XmlReader reader)
    {
      T proto = new T();
      IDictionary<string, IProtoColumn<T>> metadata = instance.Value.metadata;

      if (reader.IsEmptyElement)
      {
        return proto;
      }

      int saved = reader.Depth;
      reader.Read();
      while (reader.Depth > saved)
      {
        IProtoColumn<T> column;
        if (metadata.TryGetValue(reader.Name, out column))
        {
          column.Read(reader, proto);
          reader.Read();
        }
        else
        {
          reader.Skip();
        }
      }

      return proto;
    }

    public static void Write(XmlWriter writer, T proto)
    {
      IProtoColumn<T>[] columns = instance.Value.columns;
      for (int index = 0; index < columns.Length; index++)
      {
        IProtoColumn<T> column = columns[index];
        if (column != null)
        {
          column.Write(writer, proto);
        }
      }
    }

    public static T Read(JsonReader reader)
    {
      T proto = new T();
      IDictionary<string, IProtoColumn<T>> metadata = instance.Value.metadata;

      while (reader.Read() && reader.TokenType != JsonToken.EndObject)
      {
        IProtoColumn<T> column;
        if (metadata.TryGetValue(reader.Value.ToString(), out column))
        {
          column.Read(reader, proto);
        }
        else
        {
          reader.Skip();
        }
      }

      return proto;
    }

    public static void Write(JsonWriter writer, T proto)
    {
      IProtoColumn<T>[] columns = instance.Value.columns;

      writer.WriteStartObject();
      for (int index = 0; index < columns.Length; index++)
      {
        IProtoColumn<T> column = columns[index];
        if (column != null)
        {
          column.Write(writer, proto);
        }
      }
      writer.WriteEndObject();
    }

    public static T Read(TProtocol iprot)
    {
      T value = new T();
      return Proto<T>.Read(iprot, value);
    }
    public static T Read(TProtocol iprot, T value)
    {
      instance.Value.reader(iprot, value);
      return value;
    }

    public static T Read2(TProtocol iprot, T value)
    {
      IProtoColumn<T>[] columns = instance.Value.columns;
      iprot.IncrementRecursionDepth();
      try
      {
        iprot.ReadStructBegin();
        while (true)
        {
          TField field = iprot.ReadFieldBegin();
          if (field.Type == TType.Stop)
          {
            break;
          }

          if (field.ID < columns.Length)
          {
            IProtoColumn<T> column = columns[field.ID];
            column.Read(iprot, value);
          }
          else
          {
            TProtocolUtil.Skip(iprot, field.Type);
          }

          iprot.ReadFieldEnd();
        }
        iprot.ReadStructEnd();
        return value;
      }
      finally
      {
        iprot.DecrementRecursionDepth();
      }
    }

    public static void Write(TProtocol oprot, T value)
    {
      instance.Value.writer(oprot, value);
    }

    public static void Write2(TProtocol oprot, T value)
    {
      IProtoColumn<T>[] columns = instance.Value.columns;
      oprot.IncrementRecursionDepth();
      try
      {
        TStruct struc = new TStruct(typeof(T).Name);
        oprot.WriteStructBegin(struc);
        for (int index = 0; index < columns.Length; index++)
        {
          IProtoColumn<T> column = columns[index];
          if (column != null)
          {
            column.Write(oprot, value);
          }
        }
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }
      finally
      {
        oprot.DecrementRecursionDepth();
      }
    }

    /// <summary>
    /// Initialize proto metadata
    /// </summary>
    /// <returns></returns>
    private static Proto<T> Initialize()
    {
      Type type = typeof(T);
      if (!type.HasCustomAttribute<ProtoAttribute>())
      {
        throw new ProtoException(ProtoErrorCode.InvalidArgument, "ProtoAttribute is not defined for type {0}", type.FullName);
      }

      BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;

      List<IProtoColumn<T>> propColumns = new List<IProtoColumn<T>>();
      foreach (PropertyInfo propInfo in type.GetProperties(bindingFlags))
      {
        if (propInfo.HasCustomAttribute<ProtoColumnAttribute>())
        {
          IProtoColumn<T> protoColumn = Proto<T>.CreateProtoColumn(propInfo);
          propColumns.Add(protoColumn);
        }
      }

      HashSet<string> nameChecks = new HashSet<string>();
      HashSet<short> idChecks = new HashSet<short>();

      int maxID = 0;
      int minID = 1;
      int count = 0;

      foreach (IProtoColumn<T> col in propColumns)
      {
        if (!string.IsNullOrEmpty(col.Name))
        {
          if (nameChecks.Contains(col.Name))
          {
            throw new ProtoException(ProtoErrorCode.InvalidArgument, "Duplicated column name {0}", col.Name);
          }
          else
          {
            nameChecks.Add(col.Name);
          }
        }

        if (idChecks.Contains(col.ID))
        {
          throw new ProtoException(ProtoErrorCode.InvalidArgument, "Duplicated column ID {0}", col.ID);
        }
        else
        {
          idChecks.Add(col.ID);
        }

        count++;
        minID = Math.Min(col.ID, minID);
        maxID = Math.Max(col.ID, maxID);
      }

      // if (minID != 1 || maxID != count)
      // {
      //   throw new ProtoException(ProtoErrorCode.InvalidArgument, "Column IDs are not consecutive starting from 1");
      // }

      if (minID <=  0 && maxID > short.MaxValue)
      {
        throw new ProtoException(ProtoErrorCode.InvalidArgument, "Column IDs must be unique and between 1 to 65535");
      }

      IProtoColumn<T>[] columns = new IProtoColumn<T>[maxID + 1];
      foreach (IProtoColumn<T> column in propColumns)
      {
        columns[column.ID] = column;
      }

      Proto<T> proto = new Proto<T>(columns);

      ParameterExpression iprot = Expression.Parameter(protType);
      ParameterExpression ivalue = Expression.Parameter(typeof(T));
      Expression readExpr = Read(iprot, ivalue, columns);
      proto.reader = Expression.Lambda<Action<TProtocol, T>>(readExpr, iprot, ivalue).Compile();

      ParameterExpression oprot = Expression.Parameter(protType);
      ParameterExpression ovalue = Expression.Parameter(typeof(T));
      Expression writeExpr = Write(oprot, ovalue, columns);
      proto.writer = Expression.Lambda<Action<TProtocol, T>>(writeExpr, oprot, ovalue).Compile();

      return proto;
    }

    public static Expression Read(ParameterExpression iprot, ParameterExpression value, IProtoColumn<T>[] columns)
    {
      ParameterExpression field = Expression.Variable(typeof(TField), "field");
      LabelTarget exitTarget = Expression.Label();
      LabelTarget fieldEnd = Expression.Label();

      return Expression.Block(
        Expression.Call(iprot, protType.GetMethod("IncrementRecursionDepth")),
        Expression.TryFinally(
          Expression.Block(
            new ParameterExpression[] { field },
            Expression.Call(iprot, protType.GetMethod("ReadStructBegin")),
            Expression.Loop(
              Expression.Block(
                Expression.Assign(
                  field,
                  Expression.Call(iprot, protType.GetMethod("ReadFieldBegin"))
                ),
                Expression.IfThen(
                  Expression.Equal(
                    Expression.Property(field, typeof(TField).GetProperty("Type")),
                    Expression.Constant(TType.Stop)
                  ),
                  Expression.Goto(exitTarget)
                ),
                Expression.Switch(
                  Expression.Property(field, typeof(TField).GetProperty("ID")),
                  Expression.Call(
                    protUtilType.GetMethod("Skip", new[] { protType, typeof(TType) }),
                    iprot,
                    Expression.Property(field, typeof(TField).GetProperty("Type"))
                  ),
                  null,
                  columns
                    .Where(column => column != null)
                    .Select(column => Expression.SwitchCase(
                      Expression.Block(
                        Expression.Assign(
                          Expression.Property(value, column.PropertyInfo),
                          column.Value.Read(iprot)
                        ),
                        Expression.Break(fieldEnd)
                      ),
                      Expression.Constant(column.ID)
                    )
                  )
                ),
                Expression.Label(fieldEnd),
                Expression.Call(iprot, protType.GetMethod("ReadFieldEnd"))
              )
            ),
            Expression.Label(exitTarget),
            Expression.Call(iprot, protType.GetMethod("ReadStructEnd"))
          ),
          Expression.Call(iprot, protType.GetMethod("DecrementRecursionDepth"))
        ),
        value
      );
    }

    private static Expression Write(ParameterExpression oprot, ParameterExpression value, IProtoColumn<T>[] columns)
    {
      ParameterExpression field = Expression.Variable(typeof(TField), "field");
      LabelTarget exitTarget = Expression.Label();
      LabelTarget fieldEnd = Expression.Label();

      return Expression.Block(
        Expression.Call(oprot, protType.GetMethod("IncrementRecursionDepth")),
        Expression.TryFinally(
          Expression.Block(
            new ParameterExpression[] { field },
            Expression.Call(
              oprot,
              protType.GetMethod("WriteStructBegin"),
              Expression.New(
                typeof(TStruct).GetConstructor(typeof(string)),
                Expression.Constant(typeof(T).Name)
              )
            ),
            Expression.Assign(field, Expression.New(typeof(TField))),
            Expression.Block(
              columns
                .Where(column => column != null)
                .Select(column =>
                {
                  Type propertyType = column.PropertyInfo.PropertyType;
                  Type comparerType = typeof(EqualityComparer<int>)
                    .GetGenericTypeDefinition()
                    .MakeGenericType(propertyType);

                  PropertyInfo defaultComparerProperty = comparerType.GetProperty(
                    "Default",
                    BindingFlags.Static | BindingFlags.Public);

                  object comparer = defaultComparerProperty.GetValue(null);

                  ParameterExpression propertyValue = Expression.Variable(propertyType);
                  return Expression.Block(
                    new[] { propertyValue },
                    // V fieldValue = value.prop;
                    Expression.Assign(propertyValue, Expression.Property(value, column.PropertyInfo)),

                    Expression.IfThen(
                      // if (!EqualityComparer<V>.Default.Equals(value, this.Default))
                      Expression.IsFalse(
                        Expression.Call(
                          Expression.Constant(comparer),
                          comparerType.GetMethod("Equals", new[] { propertyType, propertyType }),
                          propertyValue,
                          Expression.Default(propertyType)
                        )
                      ),
                      Expression.Block(

                        Expression.Assign(
                          Expression.Property(field, "Name"),
                          Expression.Constant(column.Name)
                        ),
                        Expression.Assign(
                          Expression.Property(field, "Type"),
                          Expression.Constant(column.Type)
                        ),
                        Expression.Assign(
                          Expression.Property(field, "ID"),
                          Expression.Constant(column.ID)
                        ),
                        Expression.Call(oprot, protType.GetMethod("WriteFieldBegin"), field),
                        column.Value.Write(oprot, propertyValue),
                        Expression.Call(oprot, protType.GetMethod("WriteFieldEnd"))
                      )
                    )
                  );
                }
              )
            ),
            Expression.Call(oprot, protType.GetMethod("WriteFieldStop")),
            Expression.Call(oprot, protType.GetMethod("WriteStructEnd"))
          ),
          Expression.Call(oprot, protType.GetMethod("DecrementRecursionDepth"))
        )
      );
    }

    private static IProtoColumn<T> CreateProtoColumn(PropertyInfo propInfo)
    {
      Type type = typeof(T);
      Type propertyType = propInfo.PropertyType;


      LambdaExpression getter = null;
      LambdaExpression setter = null;

      Expression defaultExpr = Expression.Default(propertyType);

      Type getterType = typeof(Func<T, int>)
            .GetGenericTypeDefinition()
            .MakeGenericType(typeof(T), propertyType);

      Type setterType = typeof(Action<T, int>)
            .GetGenericTypeDefinition()
            .MakeGenericType(typeof(T), propertyType);

      if (propInfo.CanRead)
      {
        ParameterExpression objectParameter = Expression.Parameter(type);
        Expression propertyExpr = Expression.Property(objectParameter, propInfo);
        getter = Expression.Lambda(getterType, propertyExpr, objectParameter);
      }
      else
      {
        // return default value if the property has no getter method.
        ParameterExpression objectParameter = Expression.Parameter(type);
        getter = Expression.Lambda(getterType, defaultExpr, objectParameter);
      }

      if (propInfo.CanWrite)
      {
        ParameterExpression objectParameter = Expression.Parameter(type);
        ParameterExpression valueParameter = Expression.Parameter(propertyType);
        Expression propertyExpr = Expression.Property(objectParameter, propInfo);
        Expression assignExpr = Expression.Assign(propertyExpr, valueParameter);
        setter = Expression.Lambda(setterType, assignExpr, objectParameter, valueParameter);
      }
      else
      {
        ParameterExpression objectParameter = Expression.Parameter(type);
        ParameterExpression valueParameter = Expression.Parameter(propertyType);
        Expression emptyExpr = Expression.Empty();
        setter = Expression.Lambda(setterType, emptyExpr, objectParameter, valueParameter);
      }

      ProtoColumnAttribute protoColumnAttribute = propInfo.GetCustomAttribute<ProtoColumnAttribute>();
      IProtoValue valueMetadata = Proto<T>.CreateProtoValue(propertyType);

      Type genericDef = typeof(ProtoColumn<T, int>).GetGenericTypeDefinition();
      Type target = genericDef.MakeGenericType(typeof(T), propertyType);

      ConstructorInfo constructorInfo = target.GetConstructors()[0];
      return (IProtoColumn<T>)constructorInfo.Invoke(
          new object[]
          {
            propInfo,
            propInfo.Name,
            protoColumnAttribute.ID,
            valueMetadata,
            Expression.Lambda(defaultExpr).Compile().DynamicInvoke(),
            getter.Compile(),
            setter.Compile()
          }
      );
    }

    private static IProtoValue CreateProtoValue(Type type)
    {
      if (type == typeof(string))
      {
        return new StringValue();
      }
      else if (type == typeof(bool))
      {
        return new BooleanValue();
      }
      else if (type == typeof(byte))
      {
        return new ByteValue();
      }
      else if (type == typeof(short))
      {
        return new Int16Value();
      }
      else if (type == typeof(ushort))
      {
        return new UInt16Value();
      }
      else if (type == typeof(int))
      {
        return new Int32Value();
      }
      else if (type == typeof(uint))
      {
        return new UInt32Value();
      }
      else if (type == typeof(long))
      {
        return new Int64Value();
      }
      else if (type == typeof(ulong))
      {
        return new UInt64Value();
      }
      else if (type == typeof(double))
      {
        return new DoubleValue();
      }
      else if (type == typeof(DateTime))
      {
        return new DateTimeValue();
      }
      else if (type == typeof(TimeSpan))
      {
        return new TimeSpanValue();
      }
      else if (type == typeof(Guid))
      {
        return new GuidValue();
      }
      else if (type == typeof(byte[]))
      {
        return new BinaryValue();
      }
      else if (type.IsArray)
      {
        Type elementType = type.GetElementType();
        IProtoValue elementMetadata = Proto<T>.CreateProtoValue(elementType);
        Type genericDef = typeof(ArrayValue<int>).GetGenericTypeDefinition();
        Type target = genericDef.MakeGenericType(elementType);
        return (IProtoValue)Activator.CreateInstance(target, elementMetadata);
      }
      else if (type.GetTypeInfo().IsGenericType
        && type.GetGenericTypeDefinition() == typeof(List<int>).GetGenericTypeDefinition())
      {
        Type elementType = type.GetGenericArguments()[0];
        IProtoValue elementMetadata = Proto<T>.CreateProtoValue(elementType);
        Type genericDef = typeof(ListValue<int>).GetGenericTypeDefinition();
        Type target = genericDef.MakeGenericType(elementType);
        return (IProtoValue)Activator.CreateInstance(target, elementMetadata);
      }
      else if (type.GetTypeInfo().IsGenericType
        && type.GetGenericTypeDefinition() == typeof(HashSet<int>).GetGenericTypeDefinition())
      {
        Type elementType = type.GetGenericArguments()[0];
        IProtoValue elementMetadata = Proto<T>.CreateProtoValue(elementType);
        Type genericDef = typeof(SetValue<int>).GetGenericTypeDefinition();
        Type target = genericDef.MakeGenericType(elementType);
        return (IProtoValue)Activator.CreateInstance(target, elementMetadata);
      }
      else if (type.GetTypeInfo().IsGenericType
        && type.GetGenericTypeDefinition() == typeof(Dictionary<int, int>).GetGenericTypeDefinition())
      {
        Type keyType = type.GetGenericArguments()[0];
        IProtoValue keyMetadata = Proto<T>.CreateProtoValue(keyType);
        Type elementType = type.GetGenericArguments()[1];
        IProtoValue elementMetadata = Proto<T>.CreateProtoValue(elementType);
        Type genericDef = typeof(MapValue<int, int>).GetGenericTypeDefinition();
        Type target = genericDef.MakeGenericType(keyType, elementType);
        return (IProtoValue)Activator.CreateInstance(target, keyMetadata, elementMetadata);
      }
      else if (type.HasCustomAttribute<ProtoAttribute>())
      {
        Type genericDef = typeof(StructValue<T>).GetGenericTypeDefinition();
        Type target = genericDef.MakeGenericType(type);
        return (IProtoValue)Activator.CreateInstance(target);
      }
      else
      {
        throw new ProtoException(ProtoErrorCode.InvalidArgument, "failed to create proto column for type {0}", type);
      }
    }
  }
}
