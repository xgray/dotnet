
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
    /// Int32Column Type
    /// </summary>
    public class Int32Column : ProtoColumn<int>, IProtoColumn
    {
        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="intValue">column value</param>
        public override void Write(BinaryWriter writer, int intValue)
        {
            writer.Write(intValue);
        }

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        public override int Read(BinaryReader reader)
        {
            return reader.ReadInt32();
        }

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="intValue">column value</param>
        /// <returns>serialized string</returns>
        public override string ToString(int intValue)
        {
            return intValue.ToString();
        }

        /// <summary>
        /// Deserilize column value from string
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns>column value</returns>
        public override int FromString(string str)
        {
            int intValue = default(int);
            int.TryParse(str, out intValue);
            return intValue;
        }
    }
}
