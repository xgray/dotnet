
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
    /// ArrayColumn represent an array type.
    /// </summary>
    public class ArrayColumn : IProtoColumn
    {
        /// <summary>
        /// Column name. Column names usually match the Properties or fields name in a Proto type.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Column Ids have to be unique and consecutive starting from one in a Proto type.
        /// Column Id for a propert or field cannot be changed once assigned. If the property or field 
        /// is deprecated, the same ProtoColumn attribute should be add to the Proto type so 
        /// we already have a complete history of .
        /// </summary>
        public short ID
        {
            get;
            set;
        }

        public short Index
        {
            get;
            set;
        }

        /// <summary>
        /// Type of the column value.
        /// </summary>
        public Type ColumnType
        {
            get { return this.Element.ColumnType.MakeArrayType(); }
        }

        /// <summary>
        /// default value for the column
        /// </summary>
        public object DefaultValue
        {
            get { return null; }
        }

        /// <summary>
        /// Ignored columns will not be added to columnMask and therefore will not be serialized across the wire.
        /// </summary>
        public bool Ignored
        {
            get;
            set;
        }

        /// <summary>
        /// Indicate whether the column is a Nullable generic type
        /// </summary>
        public bool IsNullable
        {
            get;
            set;
        }

        /// <summary>
        /// Array Element column
        /// </summary>
        public IProtoColumn Element
        {
            get;
            set;
        }

        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="value">column value</param>
        public void Write(BinaryWriter writer, object value)
        {
            Array array = (Array)value;
            writer.Write(array.Length);

            for (int index = 0; index < array.Length; index++)
            {
                this.Element.Write(writer, array.GetValue(index));
            }
        }

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        public object Read(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            Array array = Array.CreateInstance(this.Element.ColumnType, length);

            for (int index = 0; index < length; index++)
            {
                array.SetValue(this.Element.Read(reader), index);
            }

            return array;
        }

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="value">column value</param>
        /// <returns>serialized string</returns>
        public string ToString(object value)
        {
            Array array = (Array)value;
            StringBuilder sb = new StringBuilder();

            for (int index = 0; index < array.Length; index++)
            {
                if (index > 0)
                {
                    sb.Append(ProtoUtils.DefaultSeparateChar);
                }

                string str = this.Element.ToString(array.GetValue(index));
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
        public object FromString(string str)
        {
            if (str == null)
            {
                return null;
            }

            string[] fields = str.Split(ProtoUtils.DefaultSeparateChar);
            Array array = Array.CreateInstance(this.Element.ColumnType, fields.Length);

            for (int index = 0; index < fields.Length; index++)
            {
                StringBuilder sb = new StringBuilder();
                foreach (char ch in CommonUtils.UnescapeChars(fields[index], ProtoUtils.DefaultEscapeChar, ProtoUtils.DefaultSeparateChar))
                {
                    sb.Append(ch);
                }

                array.SetValue(this.Element.FromString(sb.ToString()), index);
            }

            return array;
        }
    }
}
