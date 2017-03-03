/* Generated SBE (Simple Binary Encoding) message codec */

using System;
using Adaptive.SimpleBinaryEncoding;

namespace Sbe
{
    public class SimpleType
    {
        private DirectBuffer _buffer;
        private int _offset;
        private int _actingVersion;

        public void Wrap(DirectBuffer buffer, int offset, int actingVersion)
        {
            _offset = offset;
            _actingVersion = actingVersion;
            _buffer = buffer;
        }

        public const int Size = 44;

    public const byte ValueNullValue = (byte)0;

    public const byte ValueMinValue = (byte)32;

    public const byte ValueMaxValue = (byte)126;

    public const int ValueLength  = 30;

    public byte GetValue(int index)
    {
        if (index < 0 || index >= 30)
        {
            throw new IndexOutOfRangeException("index out of range: index=" + index);
        }

        return _buffer.CharGet(_offset + 0 + (index * 1));
    }

    public void SetValue(int index, byte value)
    {
        if (index < 0 || index >= 30)
        {
            throw new IndexOutOfRangeException("index out of range: index=" + index);
        }

        _buffer.CharPut(_offset + 0 + (index * 1), value);
    }

    public const string ValueCharacterEncoding = "UTF-8";

    public int GetValue(byte[] dst, int dstOffset)
    {
        const int length = 30;
        if (dstOffset < 0 || dstOffset > (dst.Length - length))
        {
            throw new IndexOutOfRangeException("dstOffset out of range for copy: offset=" + dstOffset);
        }

        _buffer.GetBytes(_offset + 0, dst, dstOffset, length);
        return length;
    }

    public void SetValue(byte[] src, int srcOffset)
    {
        const int length = 30;
        if (srcOffset < 0 || srcOffset > (src.Length - length))
        {
            throw new IndexOutOfRangeException("srcOffset out of range for copy: offset=" + srcOffset);
        }

        _buffer.SetBytes(_offset + 0, src, srcOffset, length);
    }

    public const short ShortValueNullValue = (short)-32768;

    public const short ShortValueMinValue = (short)-32767;

    public const short ShortValueMaxValue = (short)32767;

    public short ShortValue
    {
        get
        {
            return _buffer.Int16GetLittleEndian(_offset + 30);
        }
        set
        {
            _buffer.Int16PutLittleEndian(_offset + 30, value);
        }
    }


    public const long IntValueNullValue = -9223372036854775808L;

    public const long IntValueMinValue = -9223372036854775807L;

    public const long IntValueMaxValue = 9223372036854775807L;

    public long IntValue
    {
        get
        {
            return _buffer.Int64GetLittleEndian(_offset + 32);
        }
        set
        {
            _buffer.Int64PutLittleEndian(_offset + 32, value);
        }
    }


    public const int LongValueNullValue = -2147483648;

    public const int LongValueMinValue = -2147483647;

    public const int LongValueMaxValue = 2147483647;

    public int LongValue
    {
        get
        {
            return _buffer.Int32GetLittleEndian(_offset + 40);
        }
        set
        {
            _buffer.Int32PutLittleEndian(_offset + 40, value);
        }
    }

    }
}
