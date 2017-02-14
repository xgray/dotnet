
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
    /// GuidColumn Type
    /// </summary>
    public class GuidColumn : ProtoColumn<Guid>, IProtoColumn
    {
        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="guidValue">column value</param>
        public override void Write(BinaryWriter writer, Guid guidValue)
        {
            writer.Write(guidValue.ToByteArray());
        }

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        public override Guid Read(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(16);
            return new Guid(bytes);
        }

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="guidValue">column value</param>
        /// <returns>serialized string</returns>
        public override string ToString(Guid guidValue)
        {
            return guidValue.ToString();
        }

        /// <summary>
        /// Deserilize column value from string
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns>column value</returns>
        public override Guid FromString(string str)
        {
            Guid guidValue = new Guid(str);
            return guidValue;
        }
    }
}
