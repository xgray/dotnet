
namespace ProtoInsight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Reflection;
    using System.Text;

    using Bench;

    /// <summary>
    /// ProtoTypeColumn class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProtoTypeColumn : IProtoColumn
    {
        private Action<BinaryWriter, object> write;

        private Func<BinaryReader, object> read;

        private Func<object, string> toString;

        private Func<string, object> fromString;
        
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
            get;
            set;
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

        public short Index
        {
            get;
            set;
        }

        private static void Write<T>(BinaryWriter writer, object value)
        {
            T proto = (T)value;
            byte[] bytes = ProtoUtils.GetBytes<T>(proto.ToEnumerable());
            writer.Write(bytes);
        }

        private static object Read<T>(BinaryReader reader) where T : new()
        {
            T tuple = new T();
            foreach (ProtoSchema schema in Proto<T>.Schemas)
            {
                byte[] cbytes = reader.ReadBytes(sizeof(int));
                int count = BitConverter.ToInt32(cbytes, 0);

                byte[] payload = reader.ReadBytes(count);
                using (MemoryStream ms = new MemoryStream(payload))
                {
                    ms.Deserialize(tuple, schema);
                }
            }

            return tuple;
        }

        private static string ToString<T>(object value)
        {
            T proto = (T)value;
            return proto.GetString(ProtoUtils.DefaultEscapeChar, ProtoUtils.DefaultSeparateChar);
        }

        private static object FromString<T>(string str) where T: new()
        {
            T proto = new T();
            if(! str.TryParse<T>(ProtoUtils.DefaultEscapeChar, ProtoUtils.DefaultSeparateChar, out proto))
            {
                // TODO: better exception
                throw new InvalidCastException();
            }
            return proto;
        }

        public static ProtoTypeColumn CreateColumn(string name, short id, short index, Type columnType, bool ignored, bool isnullable)
        {
            ProtoTypeColumn protoColumn = new ProtoTypeColumn();
            protoColumn.Name = name;
            protoColumn.ID = id;
            protoColumn.Index = index;
            protoColumn.ColumnType = columnType;
            protoColumn.Ignored = ignored;
            protoColumn.IsNullable = isnullable;

            {
                MethodInfo mi = typeof(ProtoTypeColumn).GetMethod("Write", BindingFlags.Static | BindingFlags.NonPublic);
                MethodInfo mt = mi.MakeGenericMethod(columnType);

                Type gt = typeof(Action<int, int>).GetGenericTypeDefinition().MakeGenericType(typeof(BinaryWriter), typeof(object));
                protoColumn.write = (Action<BinaryWriter, object>)mt.CreateDelegate(gt);
            }

            {
                MethodInfo mi = typeof(ProtoTypeColumn).GetMethod("Read", BindingFlags.Static | BindingFlags.NonPublic);
                MethodInfo mt = mi.MakeGenericMethod(columnType);

                Type gt = typeof(Func<int, int>).GetGenericTypeDefinition().MakeGenericType(typeof(BinaryReader), typeof(object));
                protoColumn.read = (Func<BinaryReader, object>)mt.CreateDelegate(gt);
            }

            {
                MethodInfo mi = typeof(ProtoTypeColumn).GetMethod("ToString", BindingFlags.Static | BindingFlags.NonPublic);
                MethodInfo mt = mi.MakeGenericMethod(columnType);

                Type gt = typeof(Func<int, int>).GetGenericTypeDefinition().MakeGenericType(typeof(object), typeof(string));
                protoColumn.toString = (Func<object, string>)mt.CreateDelegate(gt);
            }

            {
                MethodInfo mi = typeof(ProtoTypeColumn).GetMethod("FromString", BindingFlags.Static | BindingFlags.NonPublic);
                MethodInfo mt = mi.MakeGenericMethod(columnType);

                Type gt = typeof(Func<int, int>).GetGenericTypeDefinition().MakeGenericType(typeof(string), typeof(object));
                protoColumn.fromString = (Func<string, object>)mt.CreateDelegate(gt);
            }

            return protoColumn;
        }

        /// <summary>
        /// Serialize column value to binary stream
        /// </summary>
        /// <param name="writer">binary writer</param>
        /// <param name="value">column value</param>
        void IProtoColumn.Write(BinaryWriter writer, object value)
        {
            this.write(writer, value);
        }

        /// <summary>
        /// Deserialize column value from binary stream
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <returns>column value</returns>
        object IProtoColumn.Read(BinaryReader reader)
        {
            return this.read(reader);
        }

        /// <summary>
        /// Serialize column value to string
        /// </summary>
        /// <param name="value">column value</param>
        /// <returns>serialized string</returns>
        string IProtoColumn.ToString(object value)
        {
            return this.toString(value);
        }

        /// <summary>
        /// Deserilize column value from string
        /// </summary>
        /// <param name="str">string value</param>
        /// <returns>column value</returns>
        object IProtoColumn.FromString(string str)
        {
            return this.fromString(str);
        }
    }
}
