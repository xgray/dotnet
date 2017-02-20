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
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Bench;
using Thrift.Transport;

namespace Thrift.Protocol
{
  public class TXDocProtocol : TProtocol
  {
    private XDocument doc;

    private XElement element;

    private int writeDepth = 0;

    private Stack<IXmlContext> stack;

    public TXDocProtocol(TTransport trans) : base(trans)
    {
      this.stack = new Stack<IXmlContext>();
    }

    private IXmlContext Context
    {
      get { return this.stack.Peek(); }
    }

    private void EnsureReader()
    {
      if (this.doc == null)
      {
        Stream stream = new TTransportStream(trans);

        this.doc = XDocument.Load(stream);
        this.element = doc.Root;
        this.stack.Push(new StructContext(this));
      }
    }

    private void EnsureWriter()
    {
      if (this.doc == null)
      {
        this.doc = XDocument.Parse("<root></root>");
        this.element = doc.Root;
        this.stack.Push(new StructContext(this));
      }
    }

    public override void WriteMessageBegin(TMessage message)
    {
      this.EnsureWriter();

      this.writeDepth++;
      this.Context.WriteBegin(message.Name);
      this.element.Add(new XAttribute("type", CommonUtils.ToString(message.Type)));
      this.element.Add(new XAttribute("seq", CommonUtils.ToString(message.SeqID)));

      this.stack.Push(new StructContext(this));
    }

    public override void WriteMessageEnd()
    {
      this.stack.Pop();
      this.Context.WriteEnd();

      if (this.stack.Count == 1)
      {
        string xml = this.doc.Root.ToString();
        byte[] buf = Encoding.UTF8.GetBytes(xml);
        trans.Write(buf);
        trans.Flush();
      }
    }

    public override void WriteStructBegin(TStruct struc)
    {
      this.EnsureWriter();

      this.writeDepth++;
      this.Context.WriteBegin(struc.Name);
      this.stack.Push(new StructContext(this));
    }

    public override void WriteStructEnd()
    {
      this.stack.Pop();
      this.Context.WriteEnd();

      if (this.stack.Count == 1)
      {
        string xml = this.doc.Root.Elements().First().ToString();
        byte[] buf = Encoding.UTF8.GetBytes(xml);
        trans.Write(buf);
        trans.Flush();
      }
    }

    public override void WriteFieldBegin(TField field)
    {
      this.Context.WriteBegin(field.Name);
      this.element.Add(new XAttribute("id", CommonUtils.ToString(field.ID)));
      this.element.Add(new XAttribute("type", CommonUtils.ToString(field.Type)));
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
      this.element.Add(new XAttribute("key", CommonUtils.ToString(map.KeyType)));
      this.element.Add(new XAttribute("value", CommonUtils.ToString(map.ValueType)));
      this.element.Add(new XAttribute("count", CommonUtils.ToString(map.Count)));
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
      this.element.Add(new XAttribute("element", CommonUtils.ToString(list.ElementType)));
      this.element.Add(new XAttribute("count", CommonUtils.ToString(list.Count)));
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
      this.element.Add(new XAttribute("element", CommonUtils.ToString(set.ElementType)));
      this.element.Add(new XAttribute("count", CommonUtils.ToString(set.Count)));
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

      TMessageType type = CommonUtils.ToEnum<TMessageType>(this.element.Attribute("type").Value);
      int seq = CommonUtils.ToInt32(this.element.Attribute("seq").Value);

      return new TMessage(this.element.Name.LocalName, type, seq);
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
      return new TStruct(this.element.Name.LocalName);
    }

    public override void ReadStructEnd()
    {
      this.stack.Pop();
      this.Context.ReadEnd();
    }

    public override TField ReadFieldBegin()
    {
      this.Context.ReadBegin();
      this.stack.Push(new FieldContext(this));

      TType type = CommonUtils.ToEnum<TType>(this.element.Attribute("type").Value);
      short id = CommonUtils.ToInt16(this.element.Attribute("id").Value);

      return new TField(this.element.Name.LocalName, type, id);
    }

    public override void ReadFieldEnd()
    {
      this.stack.Pop();
      this.Context.ReadEnd();
    }

    public override TMap ReadMapBegin()
    {
      this.Context.ReadBegin();

      TType keyType = CommonUtils.ToEnum<TType>(this.element.Attribute("key").Value);
      TType valueType = CommonUtils.ToEnum<TType>(this.element.Attribute("value").Value);
      int count = CommonUtils.ToInt32(this.element.Attribute("count").Value);

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

      TType elementType = CommonUtils.ToEnum<TType>(this.element.Attribute("element").Value);
      int count = CommonUtils.ToInt32(this.element.Attribute("count").Value);

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

      TType elementType = CommonUtils.ToEnum<TType>(this.element.Attribute("element").Value);
      int count = CommonUtils.ToInt32(this.element.Attribute("count").Value);

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
      private TXDocProtocol prot;

      private int index = 0;

      private XElement saved = null;
      private XElement[] elements = null;

      public StructContext(TXDocProtocol prot)
      {
        this.prot = prot;
        this.saved = prot.element;
        this.elements = prot.element.Elements().ToArray();
      }

      public void WriteBegin(string name)
      {
        XElement e = new XElement(name);
        this.prot.element.Add(e);
        this.prot.element = e;
      }

      public void WriteValue(string value)
      {
        throw new NotImplementedException();
      }

      public void WriteEnd()
      {
        this.prot.element = saved;
      }

      public void ReadBegin()
      {
        this.prot.element = this.elements[index++];
      }

      public String ReadValue()
      {
        throw new NotImplementedException();
      }

      public void ReadEnd()
      {
        this.prot.element = this.saved;
      }
    }

    public class FieldContext : IXmlContext
    {
      private TXDocProtocol prot;
      private XElement saved = null;
      private XElement[] elements = null;

      public FieldContext(TXDocProtocol prot)
      {
        this.prot = prot;
        this.saved = prot.element;
        this.elements = prot.element.Elements().ToArray();
      }

      public void WriteBegin(string name)
      {
      }

      public void WriteValue(string value)
      {
        this.prot.element.Value = value;
      }

      public void WriteEnd()
      {
      }

      public void ReadBegin()
      {
      }

      public String ReadValue()
      {
        return prot.element.Value;
      }

      public void ReadEnd()
      {
      }
    }

    public class ListContext : IXmlContext
    {
      private TXDocProtocol prot;
      private TType elementType;
      private int index = 0;
      private XElement saved = null;
      private XElement[] elements = null;

      public ListContext(TXDocProtocol prot, TType elementType)
      {
        this.prot = prot;
        this.elementType = elementType;
        this.saved = prot.element;
        this.elements = prot.element.Elements().ToArray();
      }

      public void WriteBegin(string name)
      {
        XElement e = new XElement(name ?? "Item");
        this.prot.element.Add(e);
        this.prot.element = e;
      }

      public void WriteValue(string value)
      {
        this.prot.element.Add(new XElement("Item", value));
      }

      public void WriteEnd()
      {
        this.prot.element = saved;
      }

      public void ReadBegin()
      {
        this.prot.element = this.elements[index++];
      }

      public String ReadValue()
      {
        return this.elements[index++].Value;
      }

      public void ReadEnd()
      {
        this.prot.element = saved;
      }
    }

    public class MapContext : IXmlContext
    {
      private bool writeKey = true;
      private TXDocProtocol prot;
      private TType keyType;
      private TType valueType;
      private int index = 0;
      private XElement saved = null;
      private XElement[] elements = null;

      public MapContext(TXDocProtocol prot, TType keyType, TType valueType)
      {
        this.prot = prot;
        this.keyType = keyType;
        this.valueType = valueType;
        this.saved = prot.element;
        this.elements = prot.element.Elements().ToArray();
      }

      public void WriteBegin(string name)
      {
        if (writeKey)
        {
          XElement e = new XElement(name ?? "Key");
          this.prot.element.Add(e);
          this.prot.element = e;
        }
        else
        {
          XElement e = new XElement(name ?? "Value");
          this.prot.element.Add(e);
          this.prot.element = e;
        }
      }

      public void WriteValue(string value)
      {
        if (writeKey)
        {
          this.prot.element.Add(new XElement("Key", value));
        }
        else
        {
          this.prot.element.Add(new XElement("Value", value));
        }
        writeKey = !writeKey;
      }

      public void WriteEnd()
      {
        this.prot.element = this.saved;
        writeKey = !writeKey;
      }

      public void ReadBegin()
      {
        this.prot.element = this.elements[index++];
      }

      public String ReadValue()
      {
        return this.elements[index++].Value;
      }

      public void ReadEnd()
      {
        this.prot.element = saved;
      }
    }
  }
}
