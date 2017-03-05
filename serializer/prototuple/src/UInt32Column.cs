
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
    /// uInt32Column Type
    /// </summary>
    public class UInt32Column : ProtoColumn<uint>, IProtoColumn
    {
        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="uintValue">column value</param>
        public override void Write(BinaryWriter writer, uint uintValue)
        {
            writer.Write(uintValue);
        }

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        public override uint Read(BinaryReader reader)
        {
            return reader.ReadUInt32();
        }

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="uintValue">column value</param>
        /// <returns>serialized string</returns>
        public override string ToString(uint uintValue)
        {
            return uintValue.ToString();
        }

        /// <summary>
        /// Deserilize column value from string
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns>column value</returns>
        public override uint FromString(string str)
        {
            uint uintValue = default(uint);
            uint.TryParse(str, out uintValue);
            return uintValue;
        }
    }
}
