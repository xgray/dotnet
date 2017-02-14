/**
 * Autogenerated by Thrift Compiler (0.10.0)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace thrift
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class Simple : TBase
  {
    private string _value;
    private short _shortValue;
    private int _intValue;
    private long _longValue;

    public string Value
    {
      get
      {
        return _value;
      }
      set
      {
        __isset.@value = true;
        this._value = value;
      }
    }

    public short ShortValue
    {
      get
      {
        return _shortValue;
      }
      set
      {
        __isset.shortValue = true;
        this._shortValue = value;
      }
    }

    public int IntValue
    {
      get
      {
        return _intValue;
      }
      set
      {
        __isset.intValue = true;
        this._intValue = value;
      }
    }

    public long LongValue
    {
      get
      {
        return _longValue;
      }
      set
      {
        __isset.longValue = true;
        this._longValue = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool @value;
      public bool shortValue;
      public bool intValue;
      public bool longValue;
    }

    public Simple() {
    }

    public void Read (TProtocol iprot)
    {
      iprot.IncrementRecursionDepth();
      try
      {
        TField field;
        iprot.ReadStructBegin();
        while (true)
        {
          field = iprot.ReadFieldBegin();
          if (field.Type == TType.Stop) { 
            break;
          }
          switch (field.ID)
          {
            case 1:
              if (field.Type == TType.String) {
                Value = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 2:
              if (field.Type == TType.I16) {
                ShortValue = iprot.ReadI16();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 3:
              if (field.Type == TType.I32) {
                IntValue = iprot.ReadI32();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 4:
              if (field.Type == TType.I64) {
                LongValue = iprot.ReadI64();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            default: 
              TProtocolUtil.Skip(iprot, field.Type);
              break;
          }
          iprot.ReadFieldEnd();
        }
        iprot.ReadStructEnd();
      }
      finally
      {
        iprot.DecrementRecursionDepth();
      }
    }

    public void Write(TProtocol oprot) {
      oprot.IncrementRecursionDepth();
      try
      {
        TStruct struc = new TStruct("Simple");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (Value != null && __isset.@value) {
          field.Name = "value";
          field.Type = TType.String;
          field.ID = 1;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Value);
          oprot.WriteFieldEnd();
        }
        if (__isset.shortValue) {
          field.Name = "shortValue";
          field.Type = TType.I16;
          field.ID = 2;
          oprot.WriteFieldBegin(field);
          oprot.WriteI16(ShortValue);
          oprot.WriteFieldEnd();
        }
        if (__isset.intValue) {
          field.Name = "intValue";
          field.Type = TType.I32;
          field.ID = 3;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32(IntValue);
          oprot.WriteFieldEnd();
        }
        if (__isset.longValue) {
          field.Name = "longValue";
          field.Type = TType.I64;
          field.ID = 4;
          oprot.WriteFieldBegin(field);
          oprot.WriteI64(LongValue);
          oprot.WriteFieldEnd();
        }
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }
      finally
      {
        oprot.DecrementRecursionDepth();
      }
    }

    public override string ToString() {
      StringBuilder __sb = new StringBuilder("Simple(");
      bool __first = true;
      if (Value != null && __isset.@value) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Value: ");
        __sb.Append(Value);
      }
      if (__isset.shortValue) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("ShortValue: ");
        __sb.Append(ShortValue);
      }
      if (__isset.intValue) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("IntValue: ");
        __sb.Append(IntValue);
      }
      if (__isset.longValue) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("LongValue: ");
        __sb.Append(LongValue);
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}
