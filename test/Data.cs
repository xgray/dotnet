using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using Thrift.Net;

namespace Tests
{
  [Proto]
  public class Simple
  {
    [ProtoColumn(1)]
    public string Value { get; set; }

    [ProtoColumn(2)]
    public short ShortValue { get; set; }

    [ProtoColumn(3)]
    public int IntValue { get; set; }

    [ProtoColumn(4)]
    public long LongValue { get; set; }


    public static Simple Create(
        string value = "abc",
        short shortValue = 16,
        int intValue = short.MaxValue + 16,
        long longValue = Int16.MaxValue + 16
    )
    {
      return new Simple
      {
        Value = value,
        ShortValue = shortValue,
        IntValue = intValue,
        LongValue = longValue
      };
    }
  }

  [Proto]
  public class Complex
  {
    [ProtoColumn(1)]
    public Simple SimpleValue { get; set; }

    [ProtoColumn(2)]
    public List<Simple> ListValue { get; set; }

    [ProtoColumn(3)]
    public HashSet<Simple> SetValue { get; set; }

    [ProtoColumn(4)]
    public Dictionary<string, Simple> MapValue { get; set; }

    [ProtoColumn(5)]
    public Simple[] ArrayValue { get; set; }

    [ProtoColumn(6)]
    public string[] StringArray { get; set; }

    public static Complex Create(
        string value = "abc",
        short shortValue = 16,
        int intValue = short.MaxValue + 16,
        long longValue = Int16.MaxValue + 16
    )
    {
      Simple simple = Simple.Create(value, shortValue, intValue, longValue);

      return new Complex
      {
        SimpleValue = simple,
        ListValue = new List<Simple>
        {
            simple,
            simple,
            simple
        },
        SetValue = new HashSet<Simple>
        {
            simple,
            simple
        },
        MapValue = new Dictionary<string, Simple>
        {
            {"a", simple},
            {"b", simple}
        },
        ArrayValue = new Simple[]
        {
            simple,
            simple
        },
      };
    }
  }
}
