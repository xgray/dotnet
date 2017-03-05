
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
    public class MapColumn<K, V> : ProtoColumn<Dictionary<K, V>>, IProtoColumn
    {
        public const char KVSeparatorChar = ':';

        /// <summary>
        /// Key element column
        /// </summary>
        public IProtoColumn KeyElement { get; set; }

        /// <summary>
        /// Value element column
        /// </summary>
        public IProtoColumn ValueElement { get; set; }

        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="value">column value</param>
        public override void Write(BinaryWriter writer, Dictionary<K, V> map)
        {
            writer.Write(map.Count);

            foreach (KeyValuePair<K, V> kv in map)
            {
                this.KeyElement.Write(writer, kv.Key);
                this.ValueElement.Write(writer, kv.Value);
            }
        }

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        public override Dictionary<K, V> Read(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            Dictionary<K, V> map = new Dictionary<K, V>(length);

            for (int index = 0; index < length; index++)
            {
                map.Add((K)this.KeyElement.Read(reader), (V)this.ValueElement.Read(reader));
            }

            return map;
        }

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="value">column value</param>
        /// <returns>serialized string</returns>
        public override string ToString(Dictionary<K, V> map)
        {
            StringBuilder sb = new StringBuilder();

            bool firstUse = true;
            foreach (KeyValuePair<K, V> kv in map)
            {
                if (!firstUse)
                {
                    sb.Append(ProtoUtils.DefaultSeparateChar);
                }

                firstUse = false;
                string key = this.KeyElement.ToString(kv.Key);
                foreach (char ch in CommonUtils.EscapeChars(key, ProtoUtils.DefaultEscapeChar, ProtoUtils.DefaultSeparateChar, KVSeparatorChar))
                {
                    sb.Append(ch);
                }

                sb.Append(KVSeparatorChar);

                string value = this.ValueElement.ToString(kv.Value);
                foreach (char ch in CommonUtils.EscapeChars(value, ProtoUtils.DefaultEscapeChar, ProtoUtils.DefaultSeparateChar, KVSeparatorChar))
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
        public override Dictionary<K, V> FromString(string str)
        {
            if (str == null)
            {
                return null;
            }

            string[] fields = str.Split(ProtoUtils.DefaultSeparateChar);
            Dictionary<K, V> map = new Dictionary<K, V>();

            for (int index = 0; index < fields.Length; index++)
            {
                string[] kv = fields[index].Split(KVSeparatorChar);
                if (kv.Length != 2)
                {
                    // TODO: fix exception
                    throw new InvalidCastException();
                }

                StringBuilder key = new StringBuilder();
                foreach (char ch in CommonUtils.UnescapeChars(kv[0], ProtoUtils.DefaultEscapeChar, ProtoUtils.DefaultSeparateChar, KVSeparatorChar))
                {
                    key.Append(ch);
                }

                StringBuilder value = new StringBuilder();
                foreach (char ch in CommonUtils.UnescapeChars(kv[1], ProtoUtils.DefaultEscapeChar, ProtoUtils.DefaultSeparateChar, KVSeparatorChar))
                {
                    value.Append(ch);
                }

                map.Add((K)this.KeyElement.FromString(key.ToString()), (V)this.ValueElement.FromString(value.ToString()));
            }

            return map;
        }
    }
}
