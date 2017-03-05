
namespace ProtoInsight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Bench;

    /// <summary>
    /// ProtoSchema Type
    /// </summary>
    public class ProtoSchema
    {
        /// <summary>
        /// Schema Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Bitmasks addressed by Column.ID. 1 if the column is in use.
        /// </summary>
        public byte[] ColumnMasks { get; set; }

        /// <summary>
        /// Schema Columns
        /// </summary>
        public IProtoColumn[] Columns { get; set; }

        ///// <summary>
        ///// Gets column by name
        ///// </summary>
        ///// <param name="name">column name</param>
        ///// <returns>proto column</returns>
        //public IProtoColumn this[string name]
        //{
        //    get { return this.Columns.Single(column => column.Name == name); }
        //}

        ///// <summary>
        ///// Gets column by column ID
        ///// </summary>
        ///// <param name="id">column ID</param>
        ///// <returns>proto column</returns>
        //public IProtoColumn this[int id]
        //{
        //    get { return this.Columns[id - 1]; }
        //}

        //public bool TryGetColumn(string name, bool ignoreCase, out IProtoColumn column)
        //{
        //    column = this.Columns.SingleOrDefault(col => string.Compare(col.Name, name, ignoreCase) == 0);
        //    return column != null;
        //}
    }
}
