
namespace ProtoInsight
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// ProtoColumn class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ProtoColumn<T> : IProtoColumn
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

        /// <summary>
        /// Type of the column value.
        /// </summary>
        public Type ColumnType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// default value for the column
        /// </summary>
        public object DefaultValue
        {
            get { return this.IsNullable ? null : (object)default(T); }
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

        public short Index
        {
            get;
            set;
        }

        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="value">column value</param>
        public abstract void Write(BinaryWriter writer, T value);

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        public abstract T Read(BinaryReader reader);

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="value">column value</param>
        /// <returns>serialized string</returns>
        public abstract string ToString(T value);

        /// <summary>
        /// Deserilize column value from string
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns>column value</returns>
        public abstract T FromString(string str);

        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="value">column value</param>
        void IProtoColumn.Write(BinaryWriter writer, object value)
        {
            T tValue = (T)value;
            this.Write(writer, tValue);
        }

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        object IProtoColumn.Read(BinaryReader reader)
        {
            return this.Read(reader);
        }

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="value">column value</param>
        /// <returns>serialized string</returns>
        string IProtoColumn.ToString(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            T tValue = (T)value;
            return this.ToString(tValue);
        }

        /// <summary>
        /// Deserilize column value from string
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns>column value</returns>
        object IProtoColumn.FromString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return this.DefaultValue;
            }

            return this.FromString(str);
        }
    }
}
