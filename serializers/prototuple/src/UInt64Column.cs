
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
    /// UInt64Column Type
    /// </summary>
    public class UInt64Column : ProtoColumn<ulong>, IProtoColumn
    {
        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="ulongValue">column value</param>
        public override void Write(BinaryWriter writer, ulong ulongValue)
        {
            writer.Write(ulongValue);
        }

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        public override ulong Read(BinaryReader reader)
        {
            return reader.ReadUInt64();
        }

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="ulongValue">column value</param>
        /// <returns>serialized string</returns>
        public override string ToString(ulong ulongValue)
        {
            return ulongValue.ToString();
        }

        /// <summary>
        /// Deserilize column value from string
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns>column value</returns>
        public override ulong FromString(string str)
        {
            ulong ulongValue = default(ulong);
            ulong.TryParse(str, out ulongValue);
            return ulongValue;
        }
    }
}
