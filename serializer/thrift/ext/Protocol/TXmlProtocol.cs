/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 *
 * Contains some contributions under the Thrift Software License.
 * Please see doc/old-thrift-license.txt in the Thrift distribution for
 * details.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using Bench;
using Thrift.Transport;

namespace Thrift.Protocol
{
  public class TXmlProtocol : TProtocol
  {
    private XmlWriter writer;
    private XmlReader reader;

    private Stack<IXmlContext> stack;

    public TXmlProtocol(TTransport trans) : base(trans)
    {
      this.stack = new Stack<IXmlContext>();
    }

    private IXmlContext Context
    {
      get { return this.stack.Peek(); }
    }

    private void EnsureReader()
    {
      if (this.reader == null)
      {
        Stream stream = new TTransportStream(trans);

        XmlReaderSettings readerSettings = new XmlReaderSettings
        {
          IgnoreProcessingInstructions = true,
          IgnoreWhitespace = true,
          IgnoreComments = true,
        };

        this.reader = XmlReader.Create(stream, readerSettings);
        this.stack.Push(new StructContext(this));
      }
    }

    private void EnsureWriter()
    {
      if (this.writer == null)
      {
        Stream stream = new TTransportStream(trans);
        XmlWriterSettings writerSettings = new XmlWriterSettings
        {
          Indent = true,
          IndentChars = "  ",
          OmitXmlDeclaration = true,
        };

        this.writer = XmlWriter.Create(stream, writerSettings);
        this.stack.Push(new StructContext(this));
      }
    }

    public override void WriteMessageBegin(TMessage message)
    {
      this.EnsureWriter();

      this.Context.WriteBegin(message.Name);
      this.writer.WriteAttributeString("type", CommonUtils.ToString(message.Type));
      this.writer.WriteAttributeString("seq", CommonUtils.ToString(message.SeqID));

      this.stack.Push(new StructContext(this));
    }

    public override void WriteMessageEnd()
    {
      this.stack.Pop();
      this.Context.WriteEnd();
      if (this.stack.Count == 1)
      {
        this.writer.Flush();
      }
    }

    public override void WriteStructBegin(TStruct struc)
    {
      this.EnsureWriter();

      this.Context.WriteBegin(struc.Name);
      this.stack.Push(new StructContext(this));
    }

    public override void WriteStructEnd()
    {
      this.stack.Pop();
      this.Context.WriteEnd();

      if (this.stack.Count == 1)
      {
        this.writer.Flush();
      }
    }

    public override void WriteFieldBegin(TField field)
    {
      this.Context.WriteBegin(field.Name);
      this.writer.WriteAttributeString("id", CommonUtils.ToString(field.ID));
      this.writer.WriteAttributeString("type", CommonUtils.ToString(field.Type));
      this.stack.Push(new FieldContext(this));
    }

    public override void WriteFieldEnd()
    {
      this.stack.Pop();
      this.Context.WriteEnd();
    }

    public override void WriteFieldStop()
    {
    }

    public override void WriteMapBegin(TMap map)
    {
      this.Context.WriteBegin(null);
      this.writer.WriteAttributeString("key", CommonUtils.ToString(map.KeyType));
      this.writer.WriteAttributeString("value", CommonUtils.ToString(map.ValueType));
      this.writer.WriteAttributeString("count", CommonUtils.ToString(map.Count));
      this.stack.Push(new MapContext(this, map.KeyType, map.ValueType));
    }

    public override void WriteMapEnd()
    {
      this.stack.Pop();
      this.Context.WriteEnd();
    }

    public override void WriteListBegin(TList list)
    {
      this.Context.WriteBegin(null);
      this.writer.WriteAttributeString("element", CommonUtils.ToString(list.ElementType));
      this.writer.WriteAttributeString("count", CommonUtils.ToString(list.Count));
      this.stack.Push(new ListContext(this, list.ElementType));
    }

    public override void WriteListEnd()
    {
      this.stack.Pop();
      this.Context.WriteEnd();
    }

    public override void WriteSetBegin(TSet set)
    {
      this.Context.WriteBegin(null);
      this.writer.WriteAttributeString("element", CommonUtils.ToString(set.ElementType));
      this.writer.WriteAttributeString("count", CommonUtils.ToString(set.Count));
      this.stack.Push(new ListContext(this, set.ElementType));
    }

    public override void WriteSetEnd()
    {
      this.stack.Pop();
      this.Context.WriteEnd();
    }

    public override void WriteBool(bool b)
    {
      this.Context.WriteValue(CommonUtils.ToString(b));
    }

    public override void WriteByte(sbyte b)
    {
      this.Context.WriteValue(CommonUtils.ToString((byte)b));
    }

    public override void WriteI16(short i16)
    {
      this.Context.WriteValue(CommonUtils.ToString(i16));
    }

    public override void WriteI32(int i32)
    {
      this.Context.WriteValue(CommonUtils.ToString(i32));
    }

    public override void WriteI64(long i64)
    {
      this.Context.WriteValue(CommonUtils.ToString(i64));
    }

    public override void WriteDouble(double d)
    {
      this.Context.WriteValue(CommonUtils.ToString(d));
    }

    public override void WriteString(string s)
    {
      this.Context.WriteValue(s);
    }

    public override void WriteBinary(byte[] b)
    {
      this.Context.WriteValue(CommonUtils.ToString(b));
    }

    public override TMessage ReadMessageBegin()
    {
      this.EnsureReader();
      this.Context.ReadBegin();
      this.stack.Push(new StructContext(this));

      TMessageType type = CommonUtils.ToEnum<TMessageType>(this.reader.GetAttribute("type"));
      int seq = CommonUtils.ToInt32(this.reader.GetAttribute("seq"));

      return new TMessage(this.reader.Name, type, seq);
    }

    public override void ReadMessageEnd()
    {
      this.stack.Pop();
      this.Context.ReadEnd();
    }

    public override TStruct ReadStructBegin()
    {
      this.EnsureReader();
      this.Context.ReadBegin();
      this.stack.Push(new StructContext(this));
      return new TStruct(this.reader.Name);
    }

    public override void ReadStructEnd()
    {
      this.stack.Pop();
      this.Context.ReadEnd();
    }

    public override TField ReadFieldBegin()
    {
      this.Context.ReadBegin();
      if (this.reader.NodeType == XmlNodeType.EndElement)
      {
        return new TField("eof", TType.Stop, 0);
      }

      this.stack.Push(new FieldContext(this));

      TType type = CommonUtils.ToEnum<TType>(this.reader.GetAttribute("type"));
      short id = CommonUtils.ToInt16(this.reader.GetAttribute("id"));

      return new TField(this.reader.Name, type, id);
    }

    public override void ReadFieldEnd()
    {
      this.stack.Pop();
      this.Context.ReadEnd();
    }

    public override TMap ReadMapBegin()
    {
      this.Context.ReadBegin();

      TType keyType = CommonUtils.ToEnum<TType>(this.reader.GetAttribute("key"));
      TType valueType = CommonUtils.ToEnum<TType>(this.reader.GetAttribute("value"));
      int count = CommonUtils.ToInt32(this.reader.GetAttribute("count"));

      this.stack.Push(new MapContext(this, keyType, valueType));
      return new TMap(keyType, valueType, count);
    }

    public override void ReadMapEnd()
    {
      this.stack.Pop();
      this.Context.ReadEnd();
    }

    public override TList ReadListBegin()
    {
      this.Context.ReadBegin();

      TType elementType = CommonUtils.ToEnum<TType>(this.reader.GetAttribute("element"));
      int count = CommonUtils.ToInt32(this.reader.GetAttribute("count"));

      this.stack.Push(new ListContext(this, elementType));
      return new TList(elementType, count);
    }

    public override void ReadListEnd()
    {
      this.stack.Pop();
      this.Context.ReadEnd();
    }

    public override TSet ReadSetBegin()
    {
      this.Context.ReadBegin();

      TType elementType = CommonUtils.ToEnum<TType>(this.reader.GetAttribute("element"));
      int count = CommonUtils.ToInt32(this.reader.GetAttribute("count"));

      this.stack.Push(new ListContext(this, elementType));
      return new TSet(elementType, count);
    }

    public override void ReadSetEnd()
    {
      this.stack.Pop();
      this.Context.ReadEnd();
    }

    public override bool ReadBool()
    {
      return CommonUtils.ToBoolean(this.Context.ReadValue());
    }

    public override sbyte ReadByte()
    {
      return (sbyte)CommonUtils.ToByte(this.Context.ReadValue());
    }

    public override short ReadI16()
    {
      return CommonUtils.ToInt16(this.Context.ReadValue());
    }

    public override int ReadI32()
    {
      return CommonUtils.ToInt32(this.Context.ReadValue());
    }

    public override long ReadI64()
    {
      return CommonUtils.ToInt64(this.Context.ReadValue());
    }

    public override double ReadDouble()
    {
      return CommonUtils.ToDouble(this.Context.ReadValue());
    }

    public override string ReadString()
    {
      return this.Context.ReadValue();
    }
    public override byte[] ReadBinary()
    {
      return CommonUtils.ToBytes(this.Context.ReadValue());
    }

    public interface IXmlContext
    {
      void WriteBegin(string name);
      void WriteEnd();
      void WriteValue(string value);

      void ReadBegin();

      String ReadValue();

      void ReadEnd();
    }

    public class StructContext : IXmlContext
    {
      private TXmlProtocol prot;

      private int saved = 0;

      public StructContext(TXmlProtocol prot)
      {
        this.prot = prot;
      }

      public void WriteBegin(string name)
      {
        this.prot.writer.WriteStartElement(name);
      }

      public void WriteValue(string value)
      {
        throw new NotImplementedException();
      }

      public void WriteEnd()
      {
        this.prot.writer.WriteEndElement();
      }

      public void ReadBegin()
      {
        this.prot.reader.Read();
        this.saved = this.prot.reader.Depth;
      }

      public String ReadValue()
      {
        throw new NotImplementedException();
      }

      public void ReadEnd()
      {
        while (this.prot.reader.Depth > saved)
        {
          this.prot.reader.Read();
        }
      }
    }

    public class FieldContext : IXmlContext
    {
      private TXmlProtocol prot;

      public FieldContext(TXmlProtocol prot)
      {
        this.prot = prot;
      }

      public void WriteBegin(string name)
      {
      }

      public void WriteValue(string value)
      {
        this.prot.writer.WriteValue(value);
      }

      public void WriteEnd()
      {
      }

      public void ReadBegin()
      {
      }

      public String ReadValue()
      {
        this.prot.reader.Read();
        return this.prot.reader.Value;
      }

      public void ReadEnd()
      {
      }
    }

    public class ListContext : IXmlContext
    {
      private TXmlProtocol prot;
      private TType elementType;
      private int saved = 0;

      public ListContext(TXmlProtocol prot, TType elementType)
      {
        this.prot = prot;
        this.elementType = elementType;
      }

      public void WriteBegin(string name)
      {
        this.prot.writer.WriteStartElement(name ?? "Item");
      }

      public void WriteValue(string value)
      {
        this.prot.writer.WriteElementString("Item", value);
      }

      public void WriteEnd()
      {
        this.prot.writer.WriteEndElement();
      }

      public void ReadBegin()
      {
        this.prot.reader.Read();
        this.saved = this.prot.reader.Depth;
      }

      public String ReadValue()
      {
        try
        {
          this.prot.reader.Read();
          this.prot.reader.Read();
          return this.prot.reader.Value;
        }
        finally
        {
          this.prot.reader.Read();
        }
      }

      public void ReadEnd()
      {
        while (this.prot.reader.Depth > saved)
        {
          this.prot.reader.Read();
        }
      }
    }

    public class MapContext : IXmlContext
    {
      private bool writeKey = true;
      private TXmlProtocol prot;
      private TType keyType;
      private TType valueType;
      private int saved = 0;

      public MapContext(TXmlProtocol prot, TType keyType, TType valueType)
      {
        this.prot = prot;
        this.keyType = keyType;
        this.valueType = valueType;
      }

      public void WriteBegin(string name)
      {
        if (writeKey)
        {
          this.prot.writer.WriteStartElement(name ?? "Key");
        }
        else
        {
          this.prot.writer.WriteStartElement(name ?? "Value");
        }
      }

      public void WriteValue(string value)
      {
        if (writeKey)
        {
          this.prot.writer.WriteElementString("Key", value);
        }
        else
        {
          this.prot.writer.WriteElementString("Value", value);
        }
        writeKey = !writeKey;
      }

      public void WriteEnd()
      {
        this.prot.writer.WriteEndElement();
        writeKey = !writeKey;
      }

      public void ReadBegin()
      {
        this.prot.reader.Read();
        this.saved = this.prot.reader.Depth;
      }

      public String ReadValue()
      {
        try
        {
          this.prot.reader.Read();
          this.prot.reader.Read();
          return this.prot.reader.Value;
        }
        finally
        {
          this.prot.reader.Read();
        }
      }

      public void ReadEnd()
      {
        while (this.prot.reader.Depth > saved)
        {
          this.prot.reader.Read();
        }
      }
    }
  }
}
