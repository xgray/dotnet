/* Generated SBE (Simple Binary Encoding) message codec */

using System;
using Adaptive.SimpleBinaryEncoding;

namespace Sbe
{
    public class Simple
    {
    public const ushort TemplateId = (ushort)1;
    public const ushort TemplateVersion = (ushort)1;
    public const ushort BlockLength = (ushort)44;
    public const string SematicType = "";

    private readonly Simple _parentMessage;
    private DirectBuffer _buffer;
    private int _offset;
    private int _limit;
    private int _actingBlockLength;
    private int _actingVersion;

    public int Offset { get { return _offset; } }

    public Simple()
    {
        _parentMessage = this;
    }

    public void WrapForEncode(DirectBuffer buffer, int offset)
    {
        _buffer = buffer;
        _offset = offset;
        _actingBlockLength = BlockLength;
        _actingVersion = TemplateVersion;
        Limit = offset + _actingBlockLength;
    }

    public void WrapForDecode(DirectBuffer buffer, int offset,
                              int actingBlockLength, int actingVersion)
    {
        _buffer = buffer;
        _offset = offset;
        _actingBlockLength = actingBlockLength;
        _actingVersion = actingVersion;
        Limit = offset + _actingBlockLength;
    }

    public int Size
    {
        get
        {
            return _limit - _offset;
        }
    }

    public int Limit
    {
        get
        {
            return _limit;
        }
        set
        {
            _buffer.CheckLimit(_limit);
            _limit = value;
        }
    }


    public const int ValueSchemaId = 1;

    public static string ValueMetaAttribute(MetaAttribute metaAttribute)
    {
        switch (metaAttribute)
        {
            case MetaAttribute.Epoch: return "unix";
            case MetaAttribute.TimeUnit: return "nanosecond";
            case MetaAttribute.SemanticType: return "";
        }

        return "";
    }

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

    public const int ShortValueSchemaId = 1;

    public static string ShortValueMetaAttribute(MetaAttribute metaAttribute)
    {
        switch (metaAttribute)
        {
            case MetaAttribute.Epoch: return "unix";
            case MetaAttribute.TimeUnit: return "nanosecond";
            case MetaAttribute.SemanticType: return "";
        }

        return "";
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


    public const int IntValueSchemaId = 1;

    public static string IntValueMetaAttribute(MetaAttribute metaAttribute)
    {
        switch (metaAttribute)
        {
            case MetaAttribute.Epoch: return "unix";
            case MetaAttribute.TimeUnit: return "nanosecond";
            case MetaAttribute.SemanticType: return "";
        }

        return "";
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


    public const int LongValueSchemaId = 1;

    public static string LongValueMetaAttribute(MetaAttribute metaAttribute)
    {
        switch (metaAttribute)
        {
            case MetaAttribute.Epoch: return "unix";
            case MetaAttribute.TimeUnit: return "nanosecond";
            case MetaAttribute.SemanticType: return "";
        }

        return "";
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
