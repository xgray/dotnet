
namespace Thrift.Net
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Reflection;
  using System.Text;
  using System.Xml.Linq;

  using Bench;

  using Thrift.Protocol;
  using Thrift.Transport;

  public sealed class ThriftSerializer<T> where T : new()
  {

    public static string GetXml(T value)
    {
      TMemoryBuffer trans = new TMemoryBuffer();
      TXmlProtocol prot = new TXmlProtocol(trans);

      Proto<T>.Write(prot, value);
      trans.Flush();
      return Encoding.UTF8.GetString(trans.GetBuffer());
    }

    public static T FromXml(string xml)
    {
      TMemoryBuffer trans = new TMemoryBuffer(Encoding.UTF8.GetBytes(xml));
      TProtocol prot = new TXmlProtocol(trans);

      return Proto<T>.Read(prot);
    }

    public static string GetJson(T value)
    {
      TMemoryBuffer trans = new TMemoryBuffer();
      TProtocol prot = new TSimpleJSONProtocol(trans);

      Proto<T>.Write(prot, value);
      byte[] buffer = trans.GetBuffer();
      return Encoding.UTF8.GetString(buffer);
    }

    public static T FromJson(string json)
    {
      TMemoryBuffer trans = new TMemoryBuffer(Encoding.UTF8.GetBytes(json));
      TProtocol prot = new TSimpleJSONProtocol(trans);

      return Proto<T>.Read(prot);
    }

    public static byte[] GetBytes(T value)
    {
      TMemoryBuffer trans = new TMemoryBuffer();
      TProtocol prot = new TCompactProtocol(trans);

      Proto<T>.Write(prot, value);
      return trans.GetBuffer();
    }

    public static T FromBytes(byte[] bytes)
    {
      TMemoryBuffer trans = new TMemoryBuffer(bytes);
      TProtocol prot = new TCompactProtocol(trans);

      return Proto<T>.Read(prot);
    }
  }
}
