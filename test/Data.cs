using System;
using System.Collections.Generic;

using Thrift.Net;
using Bench;

namespace Tests
{
  [Proto]
  public class Simple
  {
    [ProtoColumn(10)]
    public string Value { get; set; }

    [ProtoColumn(20)]
    public short ShortValue { get; set; }

    [ProtoColumn(30)]
    public int IntValue { get; set; }

    [ProtoColumn(40)]
    public long LongValue { get; set; }

    [ProtoColumn(50)]
    public DateTime DateTimeValue { get; set; }

    [ProtoColumn(60)]
    public TimeSpan TimeSpanValue { get; set; }

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
        LongValue = longValue,
        DateTimeValue = DateTime.UtcNow,
        TimeSpanValue = TimeSpan.FromSeconds(longValue)
      };
    }

    public static Simple Random(int seed = 10)
    {
      return new Simple
      {
        Value = CommonUtils.Random.NextString(seed, seed*2),
        ShortValue = CommonUtils.Random.NextInt16(),
        IntValue = CommonUtils.Random.Next(),
        LongValue = CommonUtils.Random.NextInt64(),
        DateTimeValue = CommonUtils.Random.NextDateTime(),
        TimeSpanValue = TimeSpan.FromSeconds(CommonUtils.Random.Next())
      };
    }
  }

  [Proto]
  public class Complex
  {
    [ProtoColumn(10)]
    public Simple SimpleValue { get; set; }

    [ProtoColumn(20)]
    public List<Simple> ListValue { get; set; }

    [ProtoColumn(21)]
    public List<List<Simple>> ListListValue { get; set; }

    [ProtoColumn(22)]
    public List<List<int>> ListListIntValue { get; set; }

    [ProtoColumn(30)]
    public HashSet<Simple> SetValue { get; set; }

    [ProtoColumn(40)]
    public Dictionary<string, Simple> MapValue { get; set; }

    [ProtoColumn(50)]
    public Simple[] ArrayValue { get; set; }

    [ProtoColumn(60)]
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
        ListListValue = new List<List<Simple>>
        {
            new List<Simple> { simple, simple },
            new List<Simple> { simple, simple },
        },
        ListListIntValue = new List<List<int>>
        {
            new List<int> { intValue, intValue+1 },
            new List<int> { intValue+2, intValue+3 },
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
