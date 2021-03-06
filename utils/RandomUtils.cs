﻿
namespace Bench
{
  using System;
  using System.Reflection;
  using System.Text;

  public static partial class RandomUtils
  {
    /// <summary>
    /// Generates a random bytes array
    /// </summary>
    /// <param name="rand">rand object</param>
    /// <param name="count">number of bytes in the array</param>
    /// <returns>generated byte array</returns>
    public static byte[] NextBytes(this Random rand, int count)
    {
      byte[] bytes = new byte[count];
      rand.NextBytes(bytes);
      return bytes;
    }

    public static string NextString(this Random random, int minLength, int maxLength)
    {
      string charset = "abcdefg";

      StringBuilder sb = new StringBuilder();
      while (sb.Length < minLength)
      {
        sb.Append(charset[random.Next(0, charset.Length)]);
      }

      while (sb.Length < minLength + random.Next() * (maxLength - minLength))
      {
        sb.Append(charset[random.Next(0, charset.Length)]);
      }

      return sb.ToString();
    }

    public static bool NextBool(this Random random)
    {
      return random.NextDouble() > 0.5;
    }

    public static T NextEnum<T>(this Random random)
        where T : struct
    {
      Type type = typeof(T);
      if (type.GetTypeInfo().IsEnum == false)
      {
        throw new InvalidOperationException();
      }

      var array = Enum.GetValues(type);
      var index = random.Next(array.GetLowerBound(0), array.GetUpperBound(0) + 1);
      return (T)array.GetValue(index);
    }

    public static short NextInt16(this Random random)
    {
      return BitConverter.ToInt16(random.NextBytes(2), 0);
    }

    public static ushort NextUInt16(this Random random)
    {
      return BitConverter.ToUInt16(random.NextBytes(2), 0);
    }

    public static int NextInt32(this Random random)
    {
      return random.Next();
    }

    public static uint NextUInt32(this Random random)
    {
      return BitConverter.ToUInt32(random.NextBytes(2), 0);
    }

    public static long NextInt64(this Random random)
    {
      return BitConverter.ToInt64(random.NextBytes(8), 0);
    }

    public static ulong NextUInt64(this Random random)
    {
      return BitConverter.ToUInt64(random.NextBytes(8), 0);
    }

    public static float NextFloat(this Random random)
    {
      return BitConverter.ToSingle(random.NextBytes(4), 0);
    }

    public static DateTime NextDateTime(this Random random, DateTime minValue, DateTime maxValue)
    {
      var ticks = minValue.Ticks + (long)((maxValue.Ticks - minValue.Ticks) * random.NextDouble());
      return new DateTime(ticks);
    }

    public static DateTime NextDateTime(this Random random, DateTime minValue, TimeSpan timeSpan)
    {
      var ticks = minValue.Ticks + (long)((timeSpan.Ticks) * random.NextDouble());
      return new DateTime(ticks);
    }

    public static DateTime NextDateTime(this Random random)
    {
      return NextDateTime(random, DateTime.MinValue, DateTime.MaxValue);
    }
  }
}
