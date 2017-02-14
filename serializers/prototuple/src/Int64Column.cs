
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
    /// Int64Column Type
    /// </summary>
    public class Int64Column : ProtoColumn<long>, IProtoColumn
    {
        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="longValue">column value</param>
        public override void Write(BinaryWriter writer, long longValue)
        {
            writer.Write(longValue);
        }

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        public override long Read(BinaryReader reader)
        {
            return reader.ReadInt64();
        }

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="longValue">column value</param>
        /// <returns>serialized string</returns>
        public override string ToString(long longValue)
        {
            return longValue.ToString();
        }

        /// <summary>
        /// Deserilize column value from string
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns>column value</returns>
        public override long FromString(string str)
        {
            long longValue = default(long);
            long.TryParse(str, out longValue);
            return longValue;
        }
    }
}
