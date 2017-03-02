namespace Bench
{

    using System;

    public enum ByteOrder
    {
        /// <summary>
        /// TBD
        /// </summary>
        BigEndian,
        /// <summary>
        /// TBD
        /// </summary>
        LittleEndian
    }

    public class ByteBuffer
    {
        private byte[] buffer;
        private int limit;
        private int position;
        private ByteOrder _order;

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="array">TBD</param>
        public ByteBuffer(byte[] array)
            : this(array, 0, array.Length)
        {
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="array">TBD</param>
        /// <param name="offset">TBD</param>
        /// <param name="length">TBD</param>
        public ByteBuffer(byte[] array, int offset, int length)
        {
            buffer = array;
            position = offset;
            limit = offset + length;
        }

        /// <summary>
        /// TBD
        /// </summary>
        public void Clear()
        {
            position = 0;
            limit = buffer.Length;
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="maxBufferSize">TBD</param>
        public void Limit(int maxBufferSize)
        {
            buffer = new byte[maxBufferSize];
            limit = maxBufferSize;
        }

        /// <summary>
        /// TBD
        /// </summary>
        public void Flip()
        {
            limit = position;
            position = 0;
        }

        /// <summary>
        /// TBD
        /// </summary>
        public bool HasRemaining
        {
            get { return position < limit; }
        }

        /// <summary>
        /// TBD
        /// </summary>
        public int Remaining
        {
            get { return limit - position; }
        }

        /// <summary>
        /// TBD
        /// </summary>
        public byte[] ToArray()
        {
            return buffer;
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="src">TBD</param>
        public void Put(byte[] src)
        {
            var len = Math.Min(src.Length, Remaining);
            System.Array.Copy(src, 0, buffer, position, len);
            position += len;
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="array">TBD</param>
        /// <param name="start">TBD</param>
        /// <param name="len">TBD</param>
        /// <returns>TBD</returns>
        public static ByteBuffer Wrap(byte[] array, int start, int len)
        {
            return new ByteBuffer(array, start, len);
        }
        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="array">TBD</param>
        /// <returns>TBD</returns>
        public static ByteBuffer Wrap(byte[] array)
        {
            return new ByteBuffer(array, 0, array.Length);
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="capacity">TBD</param>
        /// <returns>TBD</returns>
        public static ByteBuffer Allocate(int capacity)
        {
            return new ByteBuffer(new byte[capacity]);
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="byteOrder">TBD</param>
        public void Order(ByteOrder byteOrder)
        {
            _order = byteOrder;
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="array">TBD</param>
        /// <param name="from">TBD</param>
        /// <param name="copyLength">TBD</param>
        public void Put(byte[] array, int @from, int copyLength)
        {
            System.Array.Copy(array, @from, buffer, position, copyLength);
            position += copyLength;
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="ar">TBD</param>
        /// <param name="offset">TBD</param>
        /// <param name="length">TBD</param>
        public void Get(byte[] ar, int offset, int length)
        {
            if (length > Remaining)
                throw new IndexOutOfRangeException();
            System.Array.Copy(buffer, position, ar, offset, length);
            position += length;
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="ar">TBD</param>
        public void Get(byte[] ar)
        {
            Get(ar, 0, ar.Length);
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="srsc">TBD</param>
        /// <param name="length">TBD</param>
        public void Put(ByteBuffer src, int length)
        {
            Put(src.buffer, src.position, length);
            src.position += length;
        }
    }
}
