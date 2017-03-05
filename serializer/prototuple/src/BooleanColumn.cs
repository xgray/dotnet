
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
    /// BooleanColumn Type
    /// </summary>
    public class BooleanColumn : ProtoColumn<bool>, IProtoColumn
    {
        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="boolValue">column value</param>
        public override void Write(BinaryWriter writer, bool boolValue)
        {
            writer.Write(boolValue);
        }

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        public override bool Read(BinaryReader reader)
        {
            return reader.ReadBoolean();
        }

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="boolValue">column value</param>
        /// <returns>serialized string</returns>
        public override string ToString(bool boolValue)
        {
            return boolValue.ToString();
        }

        /// <summary>
        /// Deserilize column value from string
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns>column value</returns>
        public override bool FromString(string str)
        {
            bool boolValue = default(bool);
            bool.TryParse(str, out boolValue);
            return boolValue;
        }
    }
}
