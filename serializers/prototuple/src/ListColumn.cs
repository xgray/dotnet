
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

    using Bench;

    /// <summary>
    /// List represent an list type.
    /// </summary>
    public class ListColumn<T> : ProtoColumn<List<T>>, IProtoColumn
    {
        /// <summary>
        /// List element column
        /// </summary>
        public IProtoColumn Element { get; set; }

        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="value">column value</param>
        public override void Write(BinaryWriter writer, List<T> list)
        {
            writer.Write(list.Count);

            for (int index = 0; index < list.Count; index++)
            {
                this.Element.Write(writer, list[index]);
            }
        }

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        public override List<T> Read(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            List<T> list = new List<T>(length);

            for (int index = 0; index < length; index++)
            {
                list.Add((T)this.Element.Read(reader));
            }

            return list;
        }

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="value">column value</param>
        /// <returns>serialized string</returns>
        public override string ToString(List<T> list)
        {
            StringBuilder sb = new StringBuilder();

            for (int index = 0; index < list.Count; index++)
            {
                if (index > 0)
                {
                    sb.Append(ProtoUtils.DefaultSeparateChar);
                }

                string str = this.Element.ToString(list[index]);
                foreach (char ch in CommonUtils.EscapeChars(str, ProtoUtils.DefaultEscapeChar, ProtoUtils.DefaultSeparateChar))
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Deserilize column value from string
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns>column value</returns>
        public override List<T> FromString(string str)
        {
            if (str == null)
            {
                return null;
            }

            string[] fields = str.Split(ProtoUtils.DefaultSeparateChar);
            List<T> list = new List<T>(fields.Length);

            for (int index = 0; index < fields.Length; index++)
            {
                StringBuilder sb = new StringBuilder();
                foreach (char ch in CommonUtils.UnescapeChars(fields[index], ProtoUtils.DefaultEscapeChar, ProtoUtils.DefaultSeparateChar))
                {
                    sb.Append(ch);
                }

                list.Add((T)this.Element.FromString(sb.ToString()));
            }

            return list;
        }
    }
}
