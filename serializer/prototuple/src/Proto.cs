
namespace ProtoInsight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Bench;

    public sealed class Proto<T>
    {
        /// <summary>
        /// Lazy Initializer of the Proto schema
        /// </summary>
        private static Lazy<Proto<T>> instance = new Lazy<Proto<T>>(Initialize);

        private ProtoSchema[] schemas;
        private IProtoColumn[] columns;

        /// <summary>
        /// getter methods used to get column values
        /// </summary>
        private Func<T, object>[] getters;

        /// <summary>
        /// setter methods used to set column values
        /// </summary>
        private Action<T, object>[] setters;

        /// <summary>
        /// Private constructor
        /// </summary>
        private Proto() { }

        ///// <summary>
        ///// Bitmasks addressed by Column.ID. 1 if the column is in use.
        ///// </summary>
        //public static byte[] ColumnMasks
        //{
        //    get { return instance.Value.ColumnMasks; }
        //}

        /// <summary>
        /// ProtoSchema for T
        /// </summary>
        public static ProtoSchema[] Schemas
        {
            get { return instance.Value.schemas; }
        }

        /// <summary>
        /// Schema Columns
        /// </summary>
        public static IProtoColumn[] Columns
        {
            get { return instance.Value.columns; }
        }

        public static void Validate()
        {
            System.Diagnostics.Debug.Assert(instance.Value != null);
        }

        /// <summary>
        /// Gets column by name
        /// </summary>
        /// <param name="name">column name</param>
        /// <returns>proto column</returns>
        public static IProtoColumn GetColumn(string name)
        {
            return Proto<T>.Columns.Single(column => column.Name == name);
        }

        ///// <summary>
        ///// Gets column by column ID
        ///// </summary>
        ///// <param name="id">column ID</param>
        ///// <returns>proto column</returns>
        //public static IProtoColumn GetColumn(int id)
        //{
        //    return Proto<T>.Columns[id - 1];
        //}

        public static bool TryGetColumn(string name, bool ignoreCase, out IProtoColumn column)
        {
            column = Proto<T>.Columns.SingleOrDefault(col => string.Compare(col.Name, name, ignoreCase) == 0);
            return column != null;
        }

        /// <summary>
        /// Get column value
        /// </summary>
        /// <param name="proto">proto object</param>
        /// <param name="column">proto column</param>
        /// <returns>column value</returns>
        public static object GetValue(T proto, IProtoColumn column)
        {
            return instance.Value.getters[column.Index](proto);
        }

        /// <summary>
        /// Set column value
        /// </summary>
        /// <param name="proto">proto object</param>
        /// <param name="column">proto column</param>
        /// <param name="value">column value</param>
        public static void SetValue(T proto, IProtoColumn column, object value)
        {
            instance.Value.setters[column.Index](proto, value);
        }

        /// <summary>
        /// Initialize proto metadata
        /// </summary>
        /// <returns></returns>
        private static Proto<T> Initialize()
        {
            Type type = typeof(T);
            if (type.GetCustomAttribute<ProtoAttribute>() == null)
            {
                // throw exception in static constructor is generally a bad practice. 
                // However, in this case it is likely a design bug and we want fail immediately.
                throw new ArgumentException(string.Format("ProtoAttribute is not defined for type {0}", type.FullName));
            }

            List<Type> types = new List<Type>();

            do
            {
                types.Add(type);
                type = type.GetTypeInfo().BaseType;
            } while (type.GetCustomAttribute<ProtoAttribute>() != null);

            types.Reverse();

            short colIndex = 0;
            Proto<T> protoType = new Proto<T>();
            protoType.schemas = types.Select(t => InitializeSchema(t, ref colIndex)).ToArray();
            protoType.columns = protoType.schemas.SelectMany(s => s.Columns).ToArray();

            type = typeof(T);

            List<Func<T, object>> getters = new List<Func<T, object>>();
            List<Action<T, object>> setters = new List<Action<T, object>>();

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            foreach (IProtoColumn col in protoType.columns)
            {
                PropertyInfo pi = typeof(T).GetProperty(col.Name, bindingFlags);

                if (pi.CanRead)
                {
                    ParameterExpression objectParameter = Expression.Parameter(type);
                    Expression propertyExpr = Expression.Property(objectParameter, pi);

                    if (pi.PropertyType.GetTypeInfo().IsValueType)
                    {
                        Expression convertExpr = Expression.Convert(propertyExpr, typeof(object));
                        Expression<Func<T, object>> getter = Expression.Lambda<Func<T, object>>(convertExpr, objectParameter);
                        getters.Add(getter.Compile());
                    }
                    else
                    {
                        Expression<Func<T, object>> getter = Expression.Lambda<Func<T, object>>(propertyExpr, objectParameter);
                        getters.Add(getter.Compile());
                    }
                }
                else
                {
                    // return default value if the property has no getter method.
                    ParameterExpression objectParameter = Expression.Parameter(type);
                    Expression defaultExpr = Expression.Default(col.ColumnType);
                    Expression convertExpr = Expression.Convert(defaultExpr, typeof(object));
                    Expression<Func<T, object>> getter = Expression.Lambda<Func<T, object>>(convertExpr, objectParameter);
                    getters.Add(getter.Compile());
                }

                if (pi.CanWrite)
                {
                    ParameterExpression objectParameter = Expression.Parameter(type);
                    ParameterExpression valueParameter = Expression.Parameter(typeof(object));
                    Expression fieldExpr = Expression.Property(objectParameter, pi);
                    Expression convertExpr = Expression.Convert(valueParameter, pi.PropertyType);
                    Expression assignExpr = Expression.Assign(fieldExpr, convertExpr);
                    Expression<Action<T, object>> setter = Expression.Lambda<Action<T, object>>(assignExpr, objectParameter, valueParameter);
                    setters.Add(setter.Compile());
                }
                else
                {
                    setters.Add(delegate { });
                }
            }

            protoType.getters = getters.ToArray();
            protoType.setters = setters.ToArray();

            return protoType;
        }

        /// <summary>
        /// Initializes proto schema
        /// <remarks>ArgumentException could be thrown if ProtoAttribute and ProtoColumn 
        /// attributes are not defined correctly on the type. </remarks>
        /// </summary>
        /// <returns>Proto Schema</returns>
        private static ProtoSchema InitializeSchema(Type type, ref short colIndex)
        {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;

            List<IProtoColumn> propColumns = new List<IProtoColumn>();
            foreach (PropertyInfo propInfo in type.GetProperties(bindingFlags))
            {
                ProtoColumnAttribute protoColumnAttribute = propInfo.GetCustomAttribute<ProtoColumnAttribute>();
                if (protoColumnAttribute != null)
                {
                    IProtoColumn protoColumn = Proto<T>.CreateColumn(propInfo.PropertyType, propInfo.Name, protoColumnAttribute.ID, colIndex++, false, protoColumnAttribute.Ignored);
                    propColumns.Add(protoColumn);
                }
            }

            HashSet<string> nameChecks = new HashSet<string>();
            HashSet<short> idChecks = new HashSet<short>();

            int maxID = 0;
            int minID = 1;
            int count = 0;

            foreach (IProtoColumn col in propColumns.OfType<IProtoColumn>())
            {
                if (!string.IsNullOrEmpty(col.Name))
                {
                    if (nameChecks.Contains(col.Name))
                    {
                        throw new ProtoException(ProtoErrorCode.InvalidArgument, "Column name {0} is duplicated", col.Name);
                    }
                    else
                    {
                        nameChecks.Add(col.Name);
                    }
                }

                if (idChecks.Contains(col.ID))
                {
                    throw new ProtoException(ProtoErrorCode.InvalidArgument, "Column ID {0} is duplicated", col.ID);
                }
                else
                {
                    idChecks.Add(col.ID);
                }

                count++;
                minID = Math.Min(col.ID, minID);
                maxID = Math.Max(col.ID, maxID);
            }

            if (minID != 1 || maxID != count)
            {
                throw new ProtoException(ProtoErrorCode.InvalidArgument, "Column IDs are not consecutive starting from 1");
            }

            byte[] columnMasks = new byte[CommonUtils.BitmaskSize(count + 1)];

            if (columnMasks.Length > byte.MaxValue)
            {
                throw new ProtoException(ProtoErrorCode.InvalidArgument, "The length of ColumnMasks is greater than {0}", byte.MaxValue);
            }

            for (int index = 0; index < columnMasks.Length; index++)
            {
                columnMasks[index] = 0;
            }

            foreach (IProtoColumn col in propColumns.OfType<IProtoColumn>().Where(c => !c.Ignored))
            {
                CommonUtils.SetBitmask(columnMasks, col.ID, true);
            }

            return new ProtoSchema
            {
                Name = type.Name,
                ColumnMasks = columnMasks,
                Columns = propColumns.OrderBy(col => col.ID).ToArray(),
            };
        }

        /// <summary>
        /// Create a ProtoColumn for a specific type.
        /// </summary>
        /// <param name="type">Column type</param>
        /// <param name="name">Column name</param>
        /// <param name="id">Column ID</param>
        /// <returns>protocolumn object</returns>
        private static IProtoColumn CreateColumn(Type type, string name, short id = 0, short colIndex = 0, bool nullable = false, bool ignored = false)
        {
            if (type == typeof(string))
            {
                return new StringColumn
                {
                    Name = name,
                    ID = id,
                    Index = colIndex,
                    Ignored = ignored,
                };
            }
            else if (type == typeof(bool))
            {
                return new BooleanColumn
                {
                    Name = name,
                    ID = id,
                    Index = colIndex,
                    IsNullable = nullable,
                    Ignored = ignored,
                };
            }
            else if (type == typeof(byte))
            {
                return new ByteColumn
                {
                    Name = name,
                    ID = id,
                    Index = colIndex,
                    IsNullable = nullable,
                    Ignored = ignored,
                };
            }
            else if (type == typeof(short))
            {
                return new Int16Column
                {
                    Name = name,
                    ID = id,
                    Index = colIndex,
                    IsNullable = nullable,
                    Ignored = ignored,
                };
            }
            else if (type == typeof(ushort))
            {
                return new UInt16Column
                {
                    Name = name,
                    ID = id,
                    Index = colIndex,
                    IsNullable = nullable,
                    Ignored = ignored,
                };
            }
            else if (type == typeof(int))
            {
                return new Int32Column
                {
                    Name = name,
                    ID = id,
                    Index = colIndex,
                    IsNullable = nullable,
                    Ignored = ignored,
                };
            }
            else if (type == typeof(uint))
            {
                return new UInt32Column
                {
                    Name = name,
                    ID = id,
                    Index = colIndex,
                    IsNullable = nullable,
                    Ignored = ignored,
                };
            }
            else if (type == typeof(long))
            {
                return new Int64Column
                {
                    Name = name,
                    ID = id,
                    Index = colIndex,
                    IsNullable = nullable,
                    Ignored = ignored,
                };
            }
            else if (type == typeof(ulong))
            {
                return new UInt64Column
                {
                    Name = name,
                    ID = id,
                    Index = colIndex,
                    IsNullable = nullable,
                    Ignored = ignored,
                };
            }
            else if (type == typeof(double))
            {
                return new DoubleColumn
                {
                    Name = name,
                    ID = id,
                    Index = colIndex,
                    IsNullable = nullable,
                    Ignored = ignored,
                };
            }
            else if (type == typeof(DateTime))
            {
                return new DateTimeColumn
                {
                    Name = name,
                    ID = id,
                    Index = colIndex,
                    IsNullable = nullable,
                    Ignored = ignored,
                };
            }
            else if (type == typeof(Guid))
            {
                return new GuidColumn
                {
                    Name = name,
                    ID = id,
                    Index = colIndex,
                    IsNullable = nullable,
                    Ignored = ignored,
                };
            }
            else if (type == typeof(byte[]))
            {
                return new ImageColumn
                {
                    Name = name,
                    ID = id,
                    Index = colIndex,
                    Ignored = ignored,
                };
            }
            else if (type.IsArray)
            {
                IProtoColumn element = CreateColumn(type.GetElementType(), null);
                return new ArrayColumn
                {
                    Name = name,
                    ID = id,
                    Index = colIndex,
                    Element = element
                };
            }
            else if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(int?).GetGenericTypeDefinition())
            {
                IProtoColumn protoColumn = CreateColumn(type.GetGenericArguments()[0], name, id, colIndex, nullable: true, ignored: ignored);
                if (type.GetGenericTypeDefinition().MakeGenericType(protoColumn.ColumnType) != type)
                {
                    throw new ArgumentException(string.Format("ColumnType of {0} do not match ProtoColumn definition", name));
                }

                return protoColumn;
            }
            else if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(List<int>).GetGenericTypeDefinition())
            {
                IProtoColumn element = CreateColumn(type.GetGenericArguments()[0], null);

                Type listColumnType = typeof(ListColumn<int>).GetGenericTypeDefinition().MakeGenericType(element.ColumnType);

                IProtoColumn protoColumn = (IProtoColumn)System.Activator.CreateInstance(listColumnType);
                TypeUtils.SetProperty(protoColumn, "Name", name);
                TypeUtils.SetProperty(protoColumn, "ID", id);
                TypeUtils.SetProperty(protoColumn, "Index", colIndex);
                TypeUtils.SetProperty(protoColumn, "Ignored", ignored);
                TypeUtils.SetProperty(protoColumn, "Element", element);

                return protoColumn;
            }
            else if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<int>).GetGenericTypeDefinition())
            {
                IProtoColumn element = CreateColumn(type.GetGenericArguments()[0], null);

                Type listColumnType = typeof(SetColumn<int>).GetGenericTypeDefinition().MakeGenericType(element.ColumnType);

                IProtoColumn protoColumn = (IProtoColumn)System.Activator.CreateInstance(listColumnType);
                TypeUtils.SetProperty(protoColumn, "Name", name);
                TypeUtils.SetProperty(protoColumn, "ID", id);
                TypeUtils.SetProperty(protoColumn, "Index", colIndex);
                TypeUtils.SetProperty(protoColumn, "Ignored", ignored);
                TypeUtils.SetProperty(protoColumn, "Element", element);

                return protoColumn;
            }
            else if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<int, int>).GetGenericTypeDefinition())
            {
                IProtoColumn keyElement = CreateColumn(type.GetGenericArguments()[0], null);
                IProtoColumn valueElement = CreateColumn(type.GetGenericArguments()[1], null);

                Type mapColumnType = typeof(MapColumn<int, int>).GetGenericTypeDefinition().MakeGenericType(keyElement.ColumnType, valueElement.ColumnType);

                IProtoColumn protoColumn = (IProtoColumn)System.Activator.CreateInstance(mapColumnType);
                TypeUtils.SetProperty(protoColumn, "Name", name);
                TypeUtils.SetProperty(protoColumn, "ID", id);
                TypeUtils.SetProperty(protoColumn, "Index", colIndex);
                TypeUtils.SetProperty(protoColumn, "Ignored", ignored);
                TypeUtils.SetProperty(protoColumn, "KeyElement", keyElement);
                TypeUtils.SetProperty(protoColumn, "ValueElement", valueElement);

                return protoColumn;
            }
            else if (type.GetCustomAttribute<ProtoAttribute>() != null)
            {
                return ProtoTypeColumn.CreateColumn(name, id, colIndex, type, ignored, nullable);
            }
            else
            {
                throw new ArgumentException(string.Format("failed to create proto column for type {0}", type));
            }
        }
    }
}
