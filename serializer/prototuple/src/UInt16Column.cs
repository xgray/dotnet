
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
    /// UInt16Column Type
    /// </summary>
    public class UInt16Column : ProtoColumn<ushort>, IProtoColumn
    {
        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="ushortValue">column value</param>
        public override void Write(BinaryWriter writer, ushort ushortValue)
        {
            writer.Write(ushortValue);
        }

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        public override ushort Read(BinaryReader reader)
        {
            return reader.ReadUInt16();
        }

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="ushortValue">column value</param>
        /// <returns>serialized string</returns>
        public override string ToString(ushort ushortValue)
        {
            return ushortValue.ToString();
        }

        /// <summary>
        /// Deserilize column value from string
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns>column value</returns>
        public override ushort FromString(string str)
        {
            ushort ushortValue = default(ushort);
            ushort.TryParse(str, out ushortValue);
            return ushortValue;
        }
    }
}
