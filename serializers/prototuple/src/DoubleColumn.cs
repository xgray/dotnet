
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
    /// DoubleColumn Type
    /// </summary>
    public class DoubleColumn : ProtoColumn<double>, IProtoColumn
    {
        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="doubleValue">column value</param>
        public override void Write(BinaryWriter writer, double doubleValue)
        {
            writer.Write(doubleValue);
        }

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        public override double Read(BinaryReader reader)
        {
            return reader.ReadDouble();
        }

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="doubleValue">column value</param>
        /// <returns>serialized string</returns>
        public override string ToString(double doubleValue)
        {
            return doubleValue.ToString();
        }

        /// <summary>
        /// Deserilize column value from string
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns>column value</returns>
        public override double FromString(string str)
        {
            double doubleValue = default(double);
            double.TryParse(str, out doubleValue);
            return doubleValue;
        }
    }
}
