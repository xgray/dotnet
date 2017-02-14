
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
    /// ByteColumn Type
    /// </summary>
    public class ByteColumn : ProtoColumn<byte>, IProtoColumn
    {
        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="byteValue">column value</param>
        public override void Write(BinaryWriter writer, byte byteValue)
        {
            writer.Write(byteValue);
        }

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        public override byte Read(BinaryReader reader)
        {
            return reader.ReadByte();
        }

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="byteValue">column value</param>
        /// <returns>serialized string</returns>
        public override string ToString(byte byteValue)
        {
            return byteValue.ToString();
        }

        /// <summary>
        /// Deserilize column value from string
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns>column value</returns>
        public override byte FromString(string str)
        {
            byte byteValue = default(byte);
            byte.TryParse(str, out byteValue);
            return byteValue;
        }
    }
}
