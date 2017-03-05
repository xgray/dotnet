
namespace Thrift.Net
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;

    using Bench;

    /// <summary>
    /// ProtoColumn is used for two-folds. 
    /// 1. Decorate public properties and fields of a Proto type so they are can serialized/deserialized correctly. If 
    ///      the property or field is deprecated, the same ProtoColumn attribute should be used to decorate the Proto type directly.
    /// 2. The derived ProtoColumn classes should implement serailzation and deserializatoin method for each type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ProtoColumnAttribute : Attribute
    {
        public ProtoColumnAttribute()
        {
        }

        public ProtoColumnAttribute(short id)
        {
            this.ID = id;
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
        /// Ignored columns will not be added to columnMask and therefore will not be serialized across the wire.
        /// </summary>
        public bool Ignored
        {
            get;
            set;
        }
    }
}
