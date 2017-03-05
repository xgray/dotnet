
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
    /// Set represent an list type.
    /// </summary>
    public class SetColumn<T> : ProtoColumn<HashSet<T>>, IProtoColumn
    {
        /// <summary>
        /// Set element column
        /// </summary>
        public IProtoColumn Element { get; set; }

        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="value">column value</param>
        public override void Write(BinaryWriter writer, HashSet<T> set)
        {
            writer.Write(set.Count);

            foreach (T proto in set)
            {
                this.Element.Write(writer, proto);
            }
        }

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        public override HashSet<T> Read(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            HashSet<T> set = new HashSet<T>();

            for (int index = 0; index < length; index++)
            {
                set.Add((T)this.Element.Read(reader));
            }

            return set;
        }

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="value">column value</param>
        /// <returns>serialized string</returns>
        public override string ToString(HashSet<T> set)
        {
            StringBuilder sb = new StringBuilder();

            bool firstUse = true;
            foreach (T proto in set)
            {
                if (!firstUse)
                {
                    sb.Append(ProtoUtils.DefaultSeparateChar);
                }

                firstUse = false;
                string str = this.Element.ToString(proto);
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
        public override HashSet<T> FromString(string str)
        {
            if (str == null)
            {
                return null;
            }

            string[] fields = str.Split(ProtoUtils.DefaultSeparateChar);
            HashSet<T> set = new HashSet<T>();

            for (int index = 0; index < fields.Length; index++)
            {
                StringBuilder sb = new StringBuilder();
                foreach (char ch in CommonUtils.UnescapeChars(fields[index], ProtoUtils.DefaultEscapeChar, ProtoUtils.DefaultSeparateChar))
                {
                    sb.Append(ch);
                }

                set.Add((T)this.Element.FromString(sb.ToString()));
            }

            return set;
        }
    }
}
