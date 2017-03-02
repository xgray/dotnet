
namespace Bench
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    public static partial class StreamUtils
    {
        /// <summary>
        /// Reads n bytes from stream in async mode
        /// </summary>
        /// <param name="stream">stream to be read</param>
        /// <param name="count">number of bytes to be read</param>
        /// <returns>bytes array</returns>
        public static Task<byte[]> ReadAsync(this Stream stream, int count)
        {
            return stream.ReadAsync(count, CancellationToken.None);
        }

        public static byte[] Read(this Stream stream, int count)
        {
            byte[] bytes = new byte[count];

            int current = 0;
            while (current < count)
            {
                int reads = stream.Read(bytes, current, count - current);
                if (reads == 0)
                {
                    throw new EndOfStreamException();
                }

                current += reads;
            }

            return bytes;
        }

        /// <summary>
        /// Reads n bytes from stream in async mode
        /// </summary>
        /// <param name="stream">stream to be read</param>
        /// <param name="count">number of bytes to be read</param>
        /// <returns>bytes array</returns>
        public static async Task<byte[]> ReadAsync(this Stream stream, int count, CancellationToken ct)
        {
            byte[] bytes = new byte[count];

            int current = 0;
            while (current < count)
            {
                int reads = await stream.ReadAsync(bytes, current, count - current, ct);
                if (reads == 0)
                {
                    throw new EndOfStreamException();
                }

                current += reads;
            }

            return bytes;
        }

        /// <summary>
        /// Reads n bytes from stream in async mode
        /// </summary>
        /// <param name="stream">stream to be read</param>
        /// <param name="count">number of bytes to be read</param>
        /// <returns>bytes array</returns>
        public static async Task<byte[]> ReadAsync(this Stream stream, int count, Task cancelTask)
        {
            Task<byte[]> readTask = stream.ReadAsync(count);
            if (cancelTask == await Task.WhenAny(readTask, cancelTask))
            {
                cancelTask.Wait();
            }

            return readTask.Result;
        }

        /// <summary>
        /// Write bytes into stream
        /// </summary>
        /// <param name="stream">stream to be written</param>
        /// <param name="bytes">byte array to be written</param>
        public static void Write(this Stream stream, byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
