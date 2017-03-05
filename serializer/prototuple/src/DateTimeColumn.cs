
namespace ProtoInsight
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
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
    /// DateTimeColumn Type
    /// </summary>
    public class DateTimeColumn : ProtoColumn<DateTime>, IProtoColumn
    {
        /// <summary>
        /// Default DateTime format string
        /// </summary>
        public const string FormatStringDefaultValue = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Initializes a new instance of the DateTimeColumn class
        /// </summary>
        public DateTimeColumn()
        {
            this.FormatString = FormatStringDefaultValue;
        }

        /// <summary>
        /// DateTime format string
        /// </summary>
        public string FormatString
        {
            get;
            set;
        }

        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="dtValue">datetime value</param>
        public override void Write(BinaryWriter writer, DateTime dtValue)
        {
            DateTime utcValue = dtValue.ToUniversalTime();
            writer.Write(utcValue.Ticks);
        }

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        public override DateTime Read(BinaryReader reader)
        {
            long ticks = reader.ReadInt64();
            return new DateTime(ticks, DateTimeKind.Utc);
        }

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="dtValue">datetime value</param>
        /// <returns>serialized string</returns>
        public override string ToString(DateTime dtValue)
        {
            DateTime utcValue = dtValue.ToUniversalTime();
            return utcValue.ToString(FormatString);
        }

        /// <summary>
        /// Deserilize column value from string
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns>column value</returns>
        public override DateTime FromString(string str)
        {
            DateTime dtValue = DateTime.ParseExact(str, FormatString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            return dtValue;
        }
    }
}
