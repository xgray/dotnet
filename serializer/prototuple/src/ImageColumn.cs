
namespace ProtoInsight
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// ImageColumn Type for bytes array
    /// </summary>
    public class ImageColumn : ProtoColumn<byte[]>, IProtoColumn
    {
        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="value">column value</param>
        public override void Write(BinaryWriter writer, byte[] value)
        {
            writer.Write(value.Length);
            writer.Write(value, 0, value.Length);
        }

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        public override byte[] Read(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            byte[] bytes = reader.ReadBytes(count);
            return bytes;
        }

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="value">column value</param>
        /// <returns>serialized string</returns>
        public override string ToString(byte[] value)
        {
            return System.Convert.ToBase64String(value);
        }

        /// <summary>
        /// Deserilize column value from string
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns>column value</returns>
        public override byte[] FromString(string str)
        {
            byte[] bytes = System.Convert.FromBase64String(str);
            return bytes;
        }
    }
}
