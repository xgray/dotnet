
namespace Bench
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;

  public static partial class CommonUtils
  {
    /// <summary>
    /// Hashes a 32-bit signed int using Thomas Wang's method v3.1 (http://www.concentric.net/~Ttwang/tech/inthash.htm).
    /// Runtime is suggested to be 11 cycles. 
    /// </summary>
    /// <param name="input">The integer to hash.</param>
    /// <returns>The hashed result.</returns>
    public static int HashInt32(this int input)
    {
      unchecked
      {
        input = ~input + (input << 15); // x = (x << 15) - x- 1, as (~x) + y is equivalent to y - x - 1 in two's complement representation
        input = input ^ (input >> 12);
        input = input + (input << 2);
        input = input ^ (input >> 4);
        input = input * 2057; // x = (x + (x << 3)) + (x<< 11);
        input = input ^ (input >> 16);
        return (int)input;
      }
    }

    /// <summary>
    /// Hashes a string using Bob Jenkin's "One At A Time" method from Dr. Dobbs (http://burtleburtle.net/bob/hash/doobs.html).
    /// Runtime is suggested to be 9x+9, where x = input.Length. 
    /// </summary>
    /// <param name="input">The string to hash.</param>
    /// <returns>The hashed result.</returns>
    public static int HashInt32(this string input)
    {
      int hash = 0;

      for (int i = 0; i < input.Length; i++)
      {
        hash += input[i];
        hash += (hash << 10);
        hash ^= (hash >> 6);
      }

      hash += (hash << 3);
      hash ^= (hash >> 11);
      hash += (hash << 15);
      return hash;
    }

    public static int HashInt32(this byte[] bytes)
    {
      if (bytes.IsNullOrEmpty())
      {
        return 0;
      }

      return Murmur32(bytes);
    }

    public static long HashInt64(this string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return 0;
      }
      else
      {
        return Murmur64(Encoding.UTF8.GetBytes(input));
      }
    }

    public static long HashInt64(this byte[] bytes)
    {
      if (bytes == null || bytes.Length == 0)
      {
        return 0;
      }

      return Murmur64(bytes);
    }

    /// <summary>
    /// This implemention is copied from http://pastebin.com/aP8aRRHK#
    /// </summary>
    /// <param name="input">input data to be hashed</param>
    /// <param name="seed">seed value</param>
    /// <returns>32 bits hash value</returns>
    public static int Murmur32(this byte[] input, uint seed = 0)
    {
      uint hash = seed;
      uint coefficient1 = 0xcc9e2d51;
      uint coefficient2 = 0x1b873593;
      uint coefficient3 = 5 + 0xe6546b64;

      Func<uint, uint> Finalize = delegate (uint hashValue)
      {
        hashValue ^= hashValue >> 16;
        hashValue *= 0x85ebca6b;
        hashValue ^= hashValue >> 13;
        hashValue *= 0xc2b2ae35;
        hashValue ^= hashValue >> 16;

        return hashValue;
      };

      int length = input.Length;
      int index = 0;

      while (length - index >= 4)
      {
        uint segment = BitConverter.ToUInt32(input, index);
        segment *= coefficient1;
        segment = segment.RotateLeft(15);
        segment *= coefficient2;
        hash ^= segment;
        hash = hash.RotateLeft(13);
        hash = hash * coefficient3;
        index += 4;
      }

      uint tailSegment = 0;
      int rem = length % 4;

      if (rem > 0)
      {
        switch (rem)
        {
          case 3:
            tailSegment ^= (uint)input[index + 2] << 16;
            tailSegment ^= (uint)input[index + 1] << 8;
            tailSegment ^= (uint)input[index];
            break;

          case 2:
            tailSegment ^= (uint)input[index + 1] << 8;
            tailSegment ^= (uint)input[index];
            break;

          case 1:
            tailSegment ^= (uint)input[index];
            break;
        }

        tailSegment *= coefficient1;
        tailSegment = tailSegment.RotateLeft(15);
        tailSegment *= coefficient2;
        hash ^= tailSegment;
      }

      // finalize
      hash ^= (uint)length;
      hash = Finalize(hash);

      return unchecked((int)hash);
    }

    /// <summary>
    /// This implemention is copied from http://pastebin.com/aP8aRRHK#
    /// </summary>
    /// <param name="input">input data to be hashed</param>
    /// <param name="seed">seed value</param>
    /// <returns>32 bits hash value</returns>
    public static long Murmur64(this byte[] key, uint seed = 0)
    {
      uint part1 = seed;
      uint part2 = seed;

      uint coefficient1 = 0x239b961b;
      uint coefficient2 = 0xab0e9789;
      uint coefficient3 = 0x38b34ae5;

      Func<uint, uint> Finalize = delegate (uint hashValue)
      {
        hashValue ^= hashValue >> 16;
        hashValue *= 0x85ebca6b;
        hashValue ^= hashValue >> 13;
        hashValue *= 0xc2b2ae35;
        hashValue ^= hashValue >> 16;

        return hashValue;
      };

      int length = key.Length;
      int index = 0;
      while (length - index >= 8)
      {
        var segment1 = BitConverter.ToUInt32(key, index);
        var segment2 = BitConverter.ToUInt32(key, index + 4);

        segment1 *= coefficient1;
        segment1 = segment1.RotateLeft(15);
        segment1 *= coefficient2;
        part1 ^= segment1;

        part1 = part1.RotateLeft(19);
        part1 += part2;
        part1 *= 5 + 0x561ccd1b;

        segment2 *= coefficient2;
        segment2 = segment2.RotateLeft(16);
        segment2 *= coefficient3;
        part2 ^= segment2;

        part2 = part2.RotateLeft(17);
        part2 += seed;
        part2 *= 5 + 0x0bcaa747;

        index += 8;
      }

      var rem = length % 8;

      if (rem > 0)
      {
        uint tail1 = 0;
        uint tail2 = 0;

        switch (rem)
        {
          case 7:
            tail2 ^= (uint)key[index + 6] << 16;
            tail2 ^= (uint)key[index + 5] << 8;
            tail2 ^= (uint)key[index + 4] << 0;
            tail2 *= coefficient2;
            tail2 = tail2.RotateLeft(16);
            tail2 *= coefficient3;
            part2 ^= tail2;
            tail1 ^= (uint)key[index + 3] << 24;
            tail1 ^= (uint)key[index + 2] << 16;
            tail1 ^= (uint)key[index + 1] << 8;
            tail1 ^= (uint)key[index] << 0;
            break;
          case 6:
            tail2 ^= (uint)key[index + 5] << 8;
            tail2 ^= (uint)key[index + 4] << 0;
            tail2 *= coefficient2;
            tail2 = tail2.RotateLeft(16);
            tail2 *= coefficient3;
            part2 ^= tail2;
            tail1 ^= (uint)key[index + 3] << 24;
            tail1 ^= (uint)key[index + 2] << 16;
            tail1 ^= (uint)key[index + 1] << 8;
            tail1 ^= (uint)key[index] << 0;
            break;
          case 5:
            tail2 ^= (uint)key[index + 4] << 0;
            tail2 *= coefficient2;
            tail2 = tail2.RotateLeft(16);
            tail2 *= coefficient3;
            part2 ^= tail2;
            tail1 ^= (uint)key[index + 3] << 24;
            tail1 ^= (uint)key[index + 2] << 16;
            tail1 ^= (uint)key[index + 1] << 8;
            tail1 ^= (uint)key[index] << 0;
            break;
          case 4:
            tail1 ^= (uint)key[index + 3] << 24;
            tail1 ^= (uint)key[index + 2] << 16;
            tail1 ^= (uint)key[index + 1] << 8;
            tail1 ^= (uint)key[index] << 0;
            break;
          case 3:
            tail1 ^= (uint)key[index + 2] << 16;
            tail1 ^= (uint)key[index + 1] << 8;
            tail1 ^= (uint)key[index] << 0;
            break;
          case 2:
            tail1 ^= (uint)key[index + 1] << 8;
            tail1 ^= (uint)key[index] << 0;
            break;
          case 1:
            tail1 ^= (uint)key[index] << 0;
            break;
        }

        tail1 *= coefficient1;
        tail1 = tail1.RotateLeft(15);
        tail1 *= coefficient2;
        part1 ^= tail1;
      }

      part1 ^= (uint)length;
      part2 ^= (uint)length;

      part1 += part2;
      part2 += part1;

      part1 = Finalize(part1);
      part2 = Finalize(part2);

      part1 += part2;
      part2 += part1;

      long hash = BitConverter.ToInt64(BitConverter.GetBytes(part1).Concat(BitConverter.GetBytes(part2)).ToArray(), 0);
      return hash;
    }

    /// <summary>
    /// Murmur128 is based on Adam Horvath's implemntation, <see cref="http://blog.teamleadnet.com/2012/08/murmurhash3-ultra-fast-hash-algorithm.html"/>.
    /// </summary>
    /// <param name="input">input data to be hashed</param>
    /// <param name="seed">seed value</param>
    /// <returns>128 bits hash value</returns>
    public static byte[] Murmur128(this byte[] input, uint seed = 0)
    {
      ulong C1 = 0x87c37b91114253d5L;
      ulong C2 = 0x4cf5ad432745937fL;

      ulong size = 16L;
      ulong length = 0L;
      ulong h1 = seed;
      ulong h2 = 0L;

      Func<ulong, ulong> mixKey1 = delegate (ulong k1)
      {
        k1 *= C1;
        k1 = k1.RotateLeft(31);
        k1 *= C2;
        return k1;
      };

      Func<ulong, ulong> mixKey2 = delegate (ulong k2)
      {
        k2 *= C2;
        k2 = k2.RotateLeft(33);
        k2 *= C1;
        return k2;
      };


      Action<ulong, ulong> mixBody = delegate (ulong k1, ulong k2)
      {
        h1 ^= mixKey1(k1);

        h1 = h1.RotateLeft(27);
        h1 += h2;
        h1 = h1 * 5 + 0x52dce729;

        h2 ^= mixKey2(k2);

        h2 = h2.RotateLeft(31);
        h2 += h1;
        h2 = h2 * 5 + 0x38495ab5;
      };

      Func<ulong, ulong> mixFinal = delegate (ulong k)
      {
        // avalanche bits

        k ^= k >> 33;
        k *= 0xff51afd7ed558ccdL;
        k ^= k >> 33;
        k *= 0xc4ceb9fe1a85ec53L;
        k ^= k >> 33;
        return k;
      };


      int pos = 0;
      ulong remaining = (ulong)input.Length;

      // read 128 bits, 16 bytes, 2 longs in eacy cycle
      while (remaining >= size)
      {
        ulong k1 = input.GetUInt64(pos);
        pos += 8;

        ulong k2 = input.GetUInt64(pos);
        pos += 8;

        length += size;
        remaining -= size;

        mixBody(k1, k2);
      }

      // if the input MOD 16 != 0
      if (remaining > 0)
      {
        ulong k1 = 0;
        ulong k2 = 0;
        length += remaining;

        // little endian (x86) processing
        switch (remaining)
        {
          case 15:
            k2 ^= (ulong)input[pos + 14] << 48; // fall through
            goto case 14;
          case 14:
            k2 ^= (ulong)input[pos + 13] << 40; // fall through
            goto case 13;
          case 13:
            k2 ^= (ulong)input[pos + 12] << 32; // fall through
            goto case 12;
          case 12:
            k2 ^= (ulong)input[pos + 11] << 24; // fall through
            goto case 11;
          case 11:
            k2 ^= (ulong)input[pos + 10] << 16; // fall through
            goto case 10;
          case 10:
            k2 ^= (ulong)input[pos + 9] << 8; // fall through
            goto case 9;
          case 9:
            k2 ^= (ulong)input[pos + 8]; // fall through
            goto case 8;
          case 8:
            k1 ^= input.GetUInt64(pos);
            break;
          case 7:
            k1 ^= (ulong)input[pos + 6] << 48; // fall through
            goto case 6;
          case 6:
            k1 ^= (ulong)input[pos + 5] << 40; // fall through
            goto case 5;
          case 5:
            k1 ^= (ulong)input[pos + 4] << 32; // fall through
            goto case 4;
          case 4:
            k1 ^= (ulong)input[pos + 3] << 24; // fall through
            goto case 3;
          case 3:
            k1 ^= (ulong)input[pos + 2] << 16; // fall through
            goto case 2;
          case 2:
            k1 ^= (ulong)input[pos + 1] << 8; // fall through
            goto case 1;
          case 1:
            k1 ^= (ulong)input[pos]; // fall through
            break;
          default:
            throw new Exception("Something went wrong with remaining bytes calculation.");
        }

        h1 ^= mixKey1(k1);
        h2 ^= mixKey2(k2);
      }

      h1 ^= length;
      h2 ^= length;

      h1 += h2;
      h2 += h1;

      h1 = mixFinal(h1);
      h2 = mixFinal(h2);

      h1 += h2;
      h2 += h1;

      var hash = new byte[size];

      Array.Copy(BitConverter.GetBytes(h1), 0, hash, 0, 8);
      Array.Copy(BitConverter.GetBytes(h2), 0, hash, 8, 8);

      return hash;
    }
  }
}
