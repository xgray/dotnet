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
  public partial class ComplexList : TBase
  {
    private List<Complex> _struct_list_field;

    public List<Complex> Struct_list_field
    {
      get
      {
        return _struct_list_field;
      }
      set
      {
        __isset.struct_list_field = true;
        this._struct_list_field = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool struct_list_field;
    }

    public ComplexList() {
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
              if (field.Type == TType.List) {
                {
                  Struct_list_field = new List<Complex>();
                  TList _list76 = iprot.ReadListBegin();
                  for( int _i77 = 0; _i77 < _list76.Count; ++_i77)
                  {
                    Complex _elem78;
                    _elem78 = new Complex();
                    _elem78.Read(iprot);
                    Struct_list_field.Add(_elem78);
                  }
                  iprot.ReadListEnd();
                }
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
        TStruct struc = new TStruct("ComplexList");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (Struct_list_field != null && __isset.struct_list_field) {
          field.Name = "struct_list_field";
          field.Type = TType.List;
          field.ID = 1;
          oprot.WriteFieldBegin(field);
          {
            oprot.WriteListBegin(new TList(TType.Struct, Struct_list_field.Count));
            foreach (Complex _iter79 in Struct_list_field)
            {
              _iter79.Write(oprot);
            }
            oprot.WriteListEnd();
          }
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
      StringBuilder __sb = new StringBuilder("ComplexList(");
      bool __first = true;
      if (Struct_list_field != null && __isset.struct_list_field) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Struct_list_field: ");
        __sb.Append(Struct_list_field);
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}