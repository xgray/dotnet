
namespace ProtoInsight
{
    using System;
    using System.IO;

    /// <summary>
    /// IProtoColumn represents a public property and field of a strong proto type and it is used 
    /// in serializing/deserializing the strong typed proto object into/from bytes or string.
    /// </summary>
    public interface IProtoColumn
    {
        /// <summary>
        /// Column Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Column ID
        /// </summary>
        short ID { get; }

        /// <summary>
        /// Column Type
        /// </summary>
        Type ColumnType { get; }

        /// <summary>
        /// default value for the column
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        /// Whether column type is a nullable generic type
        /// </summary>
        bool IsNullable { get; }

        /// <summary>
        /// Local columns will not be added to columnMask and therefore will not be serialized across the wire.
        /// </summary>
        bool Ignored { get; }

        /// <summary>
        /// Column Index
        /// </summary>
        short Index { get; }

        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="value">column value</param>
        void Write(BinaryWriter writer, object value);

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        object Read(BinaryReader reader);

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="value">column value</param>
        /// <returns>serialized string</returns>
        string ToString(object value);

        /// <summary>
        /// Deserilize column value from string
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns>column value</returns>
        object FromString(string str);
    }
}
