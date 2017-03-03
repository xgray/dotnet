
namespace Bench
{
    using System;

    public static partial class BitUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static ulong RotateLeft(this ulong original, int bits)
        {
            return (original << bits) | (original >> (64 - bits));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static ulong RotateRight(this ulong original, int bits)
        {
            return (original >> bits) | (original << (64 - bits));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static uint RotateLeft(this uint original, int bits)
        {
            return (original << bits) | (original >> (32 - bits));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static uint RotateRight(this uint original, int bits)
        {
            return (original >> bits) | (original << (32 - bits));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bb"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static ulong GetUInt64(this byte[] bb, int pos)
        {
            return BitConverter.ToUInt64(bb, pos);
        }
    }
}
