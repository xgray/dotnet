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
  public partial class Complex : TBase
  {
    private Simple _simpleValue;
    private List<Simple> _listValue;
    private THashSet<Simple> _setValue;
    private Dictionary<string, Simple> _mapValue;
    private List<THashSet<Dictionary<string, List<Simple>>>> _listSetMap;
    private Dictionary<string, List<Dictionary<string, Simple>>> _mapList;
    private List<List<string>> _listOfList;
    private List<List<List<string>>> _listOfListOfList;

    public Simple SimpleValue
    {
      get
      {
        return _simpleValue;
      }
      set
      {
        __isset.simpleValue = true;
        this._simpleValue = value;
      }
    }

    public List<Simple> ListValue
    {
      get
      {
        return _listValue;
      }
      set
      {
        __isset.listValue = true;
        this._listValue = value;
      }
    }

    public THashSet<Simple> SetValue
    {
      get
      {
        return _setValue;
      }
      set
      {
        __isset.setValue = true;
        this._setValue = value;
      }
    }

    public Dictionary<string, Simple> MapValue
    {
      get
      {
        return _mapValue;
      }
      set
      {
        __isset.mapValue = true;
        this._mapValue = value;
      }
    }

    public List<THashSet<Dictionary<string, List<Simple>>>> ListSetMap
    {
      get
      {
        return _listSetMap;
      }
      set
      {
        __isset.listSetMap = true;
        this._listSetMap = value;
      }
    }

    public Dictionary<string, List<Dictionary<string, Simple>>> MapList
    {
      get
      {
        return _mapList;
      }
      set
      {
        __isset.mapList = true;
        this._mapList = value;
      }
    }

    public List<List<string>> ListOfList
    {
      get
      {
        return _listOfList;
      }
      set
      {
        __isset.listOfList = true;
        this._listOfList = value;
      }
    }

    public List<List<List<string>>> ListOfListOfList
    {
      get
      {
        return _listOfListOfList;
      }
      set
      {
        __isset.listOfListOfList = true;
        this._listOfListOfList = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool simpleValue;
      public bool listValue;
      public bool setValue;
      public bool mapValue;
      public bool listSetMap;
      public bool mapList;
      public bool listOfList;
      public bool listOfListOfList;
    }

    public Complex() {
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
              if (field.Type == TType.Struct) {
                SimpleValue = new Simple();
                SimpleValue.Read(iprot);
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 2:
              if (field.Type == TType.List) {
                {
                  ListValue = new List<Simple>();
                  TList _list12 = iprot.ReadListBegin();
                  for( int _i13 = 0; _i13 < _list12.Count; ++_i13)
                  {
                    Simple _elem14;
                    _elem14 = new Simple();
                    _elem14.Read(iprot);
                    ListValue.Add(_elem14);
                  }
                  iprot.ReadListEnd();
                }
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 3:
              if (field.Type == TType.Set) {
                {
                  SetValue = new THashSet<Simple>();
                  TSet _set15 = iprot.ReadSetBegin();
                  for( int _i16 = 0; _i16 < _set15.Count; ++_i16)
                  {
                    Simple _elem17;
                    _elem17 = new Simple();
                    _elem17.Read(iprot);
                    SetValue.Add(_elem17);
                  }
                  iprot.ReadSetEnd();
                }
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 4:
              if (field.Type == TType.Map) {
                {
                  MapValue = new Dictionary<string, Simple>();
                  TMap _map18 = iprot.ReadMapBegin();
                  for( int _i19 = 0; _i19 < _map18.Count; ++_i19)
                  {
                    string _key20;
                    Simple _val21;
                    _key20 = iprot.ReadString();
                    _val21 = new Simple();
                    _val21.Read(iprot);
                    MapValue[_key20] = _val21;
                  }
                  iprot.ReadMapEnd();
                }
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 5:
              if (field.Type == TType.List) {
                {
                  ListSetMap = new List<THashSet<Dictionary<string, List<Simple>>>>();
                  TList _list22 = iprot.ReadListBegin();
                  for( int _i23 = 0; _i23 < _list22.Count; ++_i23)
                  {
                    THashSet<Dictionary<string, List<Simple>>> _elem24;
                    {
                      _elem24 = new THashSet<Dictionary<string, List<Simple>>>();
                      TSet _set25 = iprot.ReadSetBegin();
                      for( int _i26 = 0; _i26 < _set25.Count; ++_i26)
                      {
                        Dictionary<string, List<Simple>> _elem27;
                        {
                          _elem27 = new Dictionary<string, List<Simple>>();
                          TMap _map28 = iprot.ReadMapBegin();
                          for( int _i29 = 0; _i29 < _map28.Count; ++_i29)
                          {
                            string _key30;
                            List<Simple> _val31;
                            _key30 = iprot.ReadString();
                            {
                              _val31 = new List<Simple>();
                              TList _list32 = iprot.ReadListBegin();
                              for( int _i33 = 0; _i33 < _list32.Count; ++_i33)
                              {
                                Simple _elem34;
                                _elem34 = new Simple();
                                _elem34.Read(iprot);
                                _val31.Add(_elem34);
                              }
                              iprot.ReadListEnd();
                            }
                            _elem27[_key30] = _val31;
                          }
                          iprot.ReadMapEnd();
                        }
                        _elem24.Add(_elem27);
                      }
                      iprot.ReadSetEnd();
                    }
                    ListSetMap.Add(_elem24);
                  }
                  iprot.ReadListEnd();
                }
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 6:
              if (field.Type == TType.Map) {
                {
                  MapList = new Dictionary<string, List<Dictionary<string, Simple>>>();
                  TMap _map35 = iprot.ReadMapBegin();
                  for( int _i36 = 0; _i36 < _map35.Count; ++_i36)
                  {
                    string _key37;
                    List<Dictionary<string, Simple>> _val38;
                    _key37 = iprot.ReadString();
                    {
                      _val38 = new List<Dictionary<string, Simple>>();
                      TList _list39 = iprot.ReadListBegin();
                      for( int _i40 = 0; _i40 < _list39.Count; ++_i40)
                      {
                        Dictionary<string, Simple> _elem41;
                        {
                          _elem41 = new Dictionary<string, Simple>();
                          TMap _map42 = iprot.ReadMapBegin();
                          for( int _i43 = 0; _i43 < _map42.Count; ++_i43)
                          {
                            string _key44;
                            Simple _val45;
                            _key44 = iprot.ReadString();
                            _val45 = new Simple();
                            _val45.Read(iprot);
                            _elem41[_key44] = _val45;
                          }
                          iprot.ReadMapEnd();
                        }
                        _val38.Add(_elem41);
                      }
                      iprot.ReadListEnd();
                    }
                    MapList[_key37] = _val38;
                  }
                  iprot.ReadMapEnd();
                }
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 7:
              if (field.Type == TType.List) {
                {
                  ListOfList = new List<List<string>>();
                  TList _list46 = iprot.ReadListBegin();
                  for( int _i47 = 0; _i47 < _list46.Count; ++_i47)
                  {
                    List<string> _elem48;
                    {
                      _elem48 = new List<string>();
                      TList _list49 = iprot.ReadListBegin();
                      for( int _i50 = 0; _i50 < _list49.Count; ++_i50)
                      {
                        string _elem51;
                        _elem51 = iprot.ReadString();
                        _elem48.Add(_elem51);
                      }
                      iprot.ReadListEnd();
                    }
                    ListOfList.Add(_elem48);
                  }
                  iprot.ReadListEnd();
                }
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 8:
              if (field.Type == TType.List) {
                {
                  ListOfListOfList = new List<List<List<string>>>();
                  TList _list52 = iprot.ReadListBegin();
                  for( int _i53 = 0; _i53 < _list52.Count; ++_i53)
                  {
                    List<List<string>> _elem54;
                    {
                      _elem54 = new List<List<string>>();
                      TList _list55 = iprot.ReadListBegin();
                      for( int _i56 = 0; _i56 < _list55.Count; ++_i56)
                      {
                        List<string> _elem57;
                        {
                          _elem57 = new List<string>();
                          TList _list58 = iprot.ReadListBegin();
                          for( int _i59 = 0; _i59 < _list58.Count; ++_i59)
                          {
                            string _elem60;
                            _elem60 = iprot.ReadString();
                            _elem57.Add(_elem60);
                          }
                          iprot.ReadListEnd();
                        }
                        _elem54.Add(_elem57);
                      }
                      iprot.ReadListEnd();
                    }
                    ListOfListOfList.Add(_elem54);
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
        TStruct struc = new TStruct("Complex");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (SimpleValue != null && __isset.simpleValue) {
          field.Name = "simpleValue";
          field.Type = TType.Struct;
          field.ID = 1;
          oprot.WriteFieldBegin(field);
          SimpleValue.Write(oprot);
          oprot.WriteFieldEnd();
        }
        if (ListValue != null && __isset.listValue) {
          field.Name = "listValue";
          field.Type = TType.List;
          field.ID = 2;
          oprot.WriteFieldBegin(field);
          {
            oprot.WriteListBegin(new TList(TType.Struct, ListValue.Count));
            foreach (Simple _iter61 in ListValue)
            {
              _iter61.Write(oprot);
            }
            oprot.WriteListEnd();
          }
          oprot.WriteFieldEnd();
        }
        if (SetValue != null && __isset.setValue) {
          field.Name = "setValue";
          field.Type = TType.Set;
          field.ID = 3;
          oprot.WriteFieldBegin(field);
          {
            oprot.WriteSetBegin(new TSet(TType.Struct, SetValue.Count));
            foreach (Simple _iter62 in SetValue)
            {
              _iter62.Write(oprot);
            }
            oprot.WriteSetEnd();
          }
          oprot.WriteFieldEnd();
        }
        if (MapValue != null && __isset.mapValue) {
          field.Name = "mapValue";
          field.Type = TType.Map;
          field.ID = 4;
          oprot.WriteFieldBegin(field);
          {
            oprot.WriteMapBegin(new TMap(TType.String, TType.Struct, MapValue.Count));
            foreach (string _iter63 in MapValue.Keys)
            {
              oprot.WriteString(_iter63);
              MapValue[_iter63].Write(oprot);
            }
            oprot.WriteMapEnd();
          }
          oprot.WriteFieldEnd();
        }
        if (ListSetMap != null && __isset.listSetMap) {
          field.Name = "listSetMap";
          field.Type = TType.List;
          field.ID = 5;
          oprot.WriteFieldBegin(field);
          {
            oprot.WriteListBegin(new TList(TType.Set, ListSetMap.Count));
            foreach (THashSet<Dictionary<string, List<Simple>>> _iter64 in ListSetMap)
            {
              {
                oprot.WriteSetBegin(new TSet(TType.Map, _iter64.Count));
                foreach (Dictionary<string, List<Simple>> _iter65 in _iter64)
                {
                  {
                    oprot.WriteMapBegin(new TMap(TType.String, TType.List, _iter65.Count));
                    foreach (string _iter66 in _iter65.Keys)
                    {
                      oprot.WriteString(_iter66);
                      {
                        oprot.WriteListBegin(new TList(TType.Struct, _iter65[_iter66].Count));
                        foreach (Simple _iter67 in _iter65[_iter66])
                        {
                          _iter67.Write(oprot);
                        }
                        oprot.WriteListEnd();
                      }
                    }
                    oprot.WriteMapEnd();
                  }
                }
                oprot.WriteSetEnd();
              }
            }
            oprot.WriteListEnd();
          }
          oprot.WriteFieldEnd();
        }
        if (MapList != null && __isset.mapList) {
          field.Name = "mapList";
          field.Type = TType.Map;
          field.ID = 6;
          oprot.WriteFieldBegin(field);
          {
            oprot.WriteMapBegin(new TMap(TType.String, TType.List, MapList.Count));
            foreach (string _iter68 in MapList.Keys)
            {
              oprot.WriteString(_iter68);
              {
                oprot.WriteListBegin(new TList(TType.Map, MapList[_iter68].Count));
                foreach (Dictionary<string, Simple> _iter69 in MapList[_iter68])
                {
                  {
                    oprot.WriteMapBegin(new TMap(TType.String, TType.Struct, _iter69.Count));
                    foreach (string _iter70 in _iter69.Keys)
                    {
                      oprot.WriteString(_iter70);
                      _iter69[_iter70].Write(oprot);
                    }
                    oprot.WriteMapEnd();
                  }
                }
                oprot.WriteListEnd();
              }
            }
            oprot.WriteMapEnd();
          }
          oprot.WriteFieldEnd();
        }
        if (ListOfList != null && __isset.listOfList) {
          field.Name = "listOfList";
          field.Type = TType.List;
          field.ID = 7;
          oprot.WriteFieldBegin(field);
          {
            oprot.WriteListBegin(new TList(TType.List, ListOfList.Count));
            foreach (List<string> _iter71 in ListOfList)
            {
              {
                oprot.WriteListBegin(new TList(TType.String, _iter71.Count));
                foreach (string _iter72 in _iter71)
                {
                  oprot.WriteString(_iter72);
                }
                oprot.WriteListEnd();
              }
            }
            oprot.WriteListEnd();
          }
          oprot.WriteFieldEnd();
        }
        if (ListOfListOfList != null && __isset.listOfListOfList) {
          field.Name = "listOfListOfList";
          field.Type = TType.List;
          field.ID = 8;
          oprot.WriteFieldBegin(field);
          {
            oprot.WriteListBegin(new TList(TType.List, ListOfListOfList.Count));
            foreach (List<List<string>> _iter73 in ListOfListOfList)
            {
              {
                oprot.WriteListBegin(new TList(TType.List, _iter73.Count));
                foreach (List<string> _iter74 in _iter73)
                {
                  {
                    oprot.WriteListBegin(new TList(TType.String, _iter74.Count));
                    foreach (string _iter75 in _iter74)
                    {
                      oprot.WriteString(_iter75);
                    }
                    oprot.WriteListEnd();
                  }
                }
                oprot.WriteListEnd();
              }
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
      StringBuilder __sb = new StringBuilder("Complex(");
      bool __first = true;
      if (SimpleValue != null && __isset.simpleValue) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("SimpleValue: ");
        __sb.Append(SimpleValue== null ? "<null>" : SimpleValue.ToString());
      }
      if (ListValue != null && __isset.listValue) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("ListValue: ");
        __sb.Append(ListValue);
      }
      if (SetValue != null && __isset.setValue) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("SetValue: ");
        __sb.Append(SetValue);
      }
      if (MapValue != null && __isset.mapValue) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("MapValue: ");
        __sb.Append(MapValue);
      }
      if (ListSetMap != null && __isset.listSetMap) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("ListSetMap: ");
        __sb.Append(ListSetMap);
      }
      if (MapList != null && __isset.mapList) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("MapList: ");
        __sb.Append(MapList);
      }
      if (ListOfList != null && __isset.listOfList) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("ListOfList: ");
        __sb.Append(ListOfList);
      }
      if (ListOfListOfList != null && __isset.listOfListOfList) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("ListOfListOfList: ");
        __sb.Append(ListOfListOfList);
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}
