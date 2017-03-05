
namespace ProtoInsight
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    using Bench;

    public static class ProtoUtils
    {
        /// <summary>
        /// server port map in test mode
        /// </summary>
        private static ConcurrentDictionary<string, string> serverPortMapper = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// start port in local run
        /// </summary>
        private static int testPort = 12000;

        /// <summary>
        /// Tryparse delegate
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="str">string to be parsed</param>
        /// <param name="result">result value</param>
        /// <returns>true if parse is success</returns>
        public delegate bool TryParseDelegate<T>(string str, out T result);

        /// <summary>
        /// The environment variable name for the domain of the machine
        /// </summary>
        public const string UserDomainEnvVariableName = "USERDOMAIN";

        /// <summary>
        /// The configuration name for the Partition assigned to the proto app in the deployment
        /// </summary>
        public const string PartitionConfigureName = "Partition";

        /// <summary>
        /// The configuration name for the primary partitions assigned to the proto app in the deployment
        /// </summary>
        public const string PrimaryPartitionsConfigureName = "PrimaryPartitions";

        /// <summary>
        /// The configuration name for the backup partitions assigned to the proto app in the deployment
        /// </summary>
        public const string BackupPartitionsConfigureName = "BackupPartitions";

        /// <summary>
        /// Backup servers config name
        /// </summary>
        public const string BackupServersConfigName = "BackupServers";

        /// <summary>
        /// Default buffer size
        /// </summary>
        public const int DefaultBufferSize = ushort.MaxValue + 1;

        /// <summary>
        /// Default seperator char
        /// </summary>
        public const char DefaultSeparateChar = ',';

        /// <summary>
        /// Default Escape char
        /// </summary>
        public const char DefaultEscapeChar = '\\';

        /// <summary>
        /// Default comment char
        /// </summary>
        public const char DefaultCommentChar = '#';

        /// <summary>
        /// Run ProtoApps in local test mode
        /// </summary>
        public static bool LocalRun { get; set; }

        /// <summary>
        /// current user domain
        /// </summary>
        public static string CurrentDomain
        {
            get { return Environment.GetEnvironmentVariable(ProtoUtils.UserDomainEnvVariableName); }
        }

        /// <summary>
        /// Gets current running process
        /// </summary>
        public static string CurrentProcess
        {
            get { return Process.GetCurrentProcess().ProcessName; }
        }

        /// <summary>
        /// Gets current machine name
        /// </summary>
        public static string CurrentMachineName
        {
            get { return Environment.MachineName; }
        }

        /// <summary>
        /// Gets environment current directory
        /// </summary>
        public static string CurrentDirectory
        {
            get { return System.IO.Directory.GetCurrentDirectory(); }
        }

        // /// <summary>
        // /// Changes context working directory and return a new context
        // /// </summary>
        // /// <param name="context">current context</param>
        // /// <param name="dir">directory to go</param>
        // /// <returns>new context</returns>
        // public static IProtoContext ChangeDirectory(this IProtoContext context, string dir)
        // {
        //     return new ProtoContext(context, dir);
        // }

        // /// <summary>
        // /// Log a trace
        // /// </summary>
        // /// <param name="context">component context</param>
        // /// <param name="component">component name</param>
        // /// <param name="traceLevel">trace level</param>
        // /// <param name="category">trace category</param>
        // /// <param name="msg">message format</param>
        // /// <param name="args">message arguments</param>
        // public static void Log(this IProtoContext context, IProtoComponent component, TraceLevel traceLevel, string category, string msg, params object[] args)
        // {
        //     context.Log(component.Name, traceLevel, category, msg, args);
        // }

        /// <summary>
        /// Get column value
        /// </summary>
        /// <param name="column">proto column</param>
        /// <returns>column value</returns>
        public static object GetValue<T>(this T tuple, IProtoColumn column)
        {
            return Proto<T>.GetValue(tuple, column);
        }

        /// <summary>
        /// Set column value
        /// </summary>
        /// <param name="column">proto column</param>
        /// <param name="value">column value</param>
        public static void SetValue<T>(this T tuple, IProtoColumn column, object value)
        {
            Proto<T>.SetValue(tuple, column, value);
        }

        /// <summary>
        /// Writes tuples to a csv file
        /// </summary>
        /// <typeparam name="T">tuple type</typeparam>
        /// <param name="tuples">tuple instances</param>
        /// <param name="writer">text writer to csv file</param>
        /// <param name="escapeChar">escape char</param>
        /// <param name="seperateChar">seperate char</param>
        public static void WriteToCsv<T>(this TextWriter writer, IEnumerable<T> tuples, char escapeChar, char seperateChar, bool outputHeader = false)
        {
            if (outputHeader)
            {
                bool firstColumn = true;
                foreach (IProtoColumn column in Proto<T>.Columns)
                {
                    if (column.Ignored)
                    {
                        continue;
                    }

                    if (firstColumn)
                    {
                        firstColumn = false;
                    }
                    else
                    {
                        writer.Write(seperateChar);
                    }

                    writer.Write(column.Name);
                }

                writer.WriteLine();
            }

            foreach (T tuple in tuples)
            {
                string line = tuple.GetString(escapeChar, seperateChar);
                writer.WriteLine(line);
            }
        }

        /// <summary>
        /// Reads tuples from a csv file
        /// </summary>
        /// <typeparam name="T">tuple type</typeparam>
        /// <param name="tuples">tuple instances</param>
        /// <param name="writer">text writer to csv file</param>
        /// <param name="escapeChar">escape char</param>
        /// <param name="seperateChar">seperate char</param>
        public static IEnumerable<T> ReadCsv<T>(this TextReader reader, char escapeChar, char seperateChar, bool withHeader = false) where T : new()
        {
            string line;

            if (withHeader)
            {
                if ((line = reader.ReadLine()) == null)
                {
                    yield break;
                }
            }

            while ((line = reader.ReadLine()) != null)
            {
                T tuple;
                if (line.TryParse(escapeChar, seperateChar, out tuple))
                {
                    yield return tuple;
                }
                else
                {
                }
            }
        }

        /// <summary>
        /// Serialize a tuple to string
        /// </summary>
        /// <param name="tuple">tuple object</param>
        /// <param name="escapeChar">escape char</param>
        /// <param name="seperateChar">seperate char</param>
        /// <returns>serialized string of the tuple</returns>
        public static string GetString<T>(this T proto, char escapeChar, char seperateChar)
        {
            using (StringWriter writer = new StringWriter())
            {
                bool firstColumn = true;
                foreach (IProtoColumn column in Proto<T>.Columns)
                {
                    if (column.Ignored)
                    {
                        continue;
                    }

                    if (firstColumn)
                    {
                        firstColumn = false;
                    }
                    else
                    {
                        writer.Write(seperateChar);
                    }

                    object value = proto.GetValue(column);
                    if (value != null)
                    {
                        string str = column.ToString(value);
                        string escaped = CommonUtils.Escape(str, escapeChar, seperateChar);
                        writer.Write(escaped);
                    }
                }

                writer.Flush();
                return writer.ToString();
            }
        }

        /// <summary>
        /// Try to parse tuple from string
        /// </summary>
        /// <param name="str">string value</param>
        /// <param name="escapeChar">escape char</param>
        /// <param name="seperateChar">seperate char</param>
        /// <param name="tuple">parsed tuple</param>
        /// <returns>true if str was converted successfully; otherwise, false.</returns>
        public static bool TryParse<T>(this string str, char escapeChar, char seperateChar, out T proto) where T : new()
        {
            Proto<T>.Validate();

            if (string.IsNullOrEmpty(str))
            {
                proto = default(T);
                return false;
            }

            proto = new T();

            string[] fields = str.Split(seperateChar);

            for (int cIndex = 0, fIndex = 0; cIndex < Proto<T>.Columns.Length && fIndex < fields.Length; cIndex++)
            {
                if (Proto<T>.Columns[cIndex].Ignored)
                {
                    continue;
                }

                string field = CommonUtils.Unescape(fields[fIndex++], escapeChar, seperateChar);
                if (!string.IsNullOrEmpty(field))
                {
                    proto.SetValue(Proto<T>.Columns[cIndex], Proto<T>.Columns[cIndex].FromString(field));
                }
                else
                {
                    proto.SetValue(Proto<T>.Columns[cIndex], Proto<T>.Columns[cIndex].DefaultValue);
                }
            }

            return true;
        }

        // /// <summary>
        // /// Creates a data table for tuples
        // /// </summary>
        // /// <typeparam name="T">tuple type</typeparam>
        // /// <param name="tuples">tuple objects</param>
        // /// <returns>data table for the tuples</returns>
        // public static DataTable CreateDataTable<T>(this IEnumerable<T> protoes) where T : new()
        // {
        //     DataTable table = new DataTable();

        //     Proto<T>.Validate();
        //     foreach (IProtoColumn protoColumn in Proto<T>.Columns)
        //     {
        //         table.Columns.Add(protoColumn.Name, protoColumn.ColumnType);
        //     }

        //     foreach (T proto in protoes)
        //     {
        //         table.Rows.Add(Proto<T>.Columns.Select(col => proto.GetValue(col)).ToArray());
        //     }

        //     return table;
        // }

        /// <summary>
        /// Display tuple objects in grid
        /// </summary>
        /// <typeparam name="T">tuple type</typeparam>
        /// <param name="dataSource"></param>
        /// <param name="msgLoop"></param>
        // public static void ShowGrid<T>(this IEnumerable<T> protoes, bool msgLoop) where T : new()
        // {
        //     DataGridView gridView = ProtoGrid<T>.CreateDataGrid();
        //     gridView.DataSource = protoes.CreateDataTable();
        //     ProtoGrid<T>.ShowGrid(gridView, msgLoop);
        // }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tuples"></param>
        /// <returns></returns>
        public static byte[] GetBytes<T>(params T[] protoes)
        {
            return ProtoUtils.GetBytes<T>((IEnumerable<T>)protoes);
        }

        /// <summary>
        /// serialize an array of tuples to bytes with version marks
        /// </summary>
        /// <typeparam name="T">tuple type</typeparam>
        /// <param name="tuples">tuple batch</param>
        /// <returns>serialized byte array</returns>
        public static byte[] GetBytes<T>(IEnumerable<T> protoes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                foreach (T proto in protoes)
                {
                    foreach (ProtoSchema schema in Proto<T>.Schemas)
                    {
                        int startPos = (int)ms.Position;
                        ms.Write(BitConverter.GetBytes(startPos), 0, sizeof(int));

                        ms.Serialize(proto, schema);

                        int endPos = (int)ms.Position;
                        ms.Position = startPos;
                        ms.Write(BitConverter.GetBytes(endPos - startPos - sizeof(int)), 0, sizeof(int));
                        ms.Position = endPos;
                    }
                }

                return ms.ToArray();
            }
        }

        public static bool TryRead<T>(this Stream stream, out T proto) where T : new()
        {
            try
            {
                proto = stream.Read<T>();
                return true;
            }
            catch (EndOfStreamException)
            {
                proto = default(T);
                return false;
            }
        }

        public static T Read<T>(this Stream stream) where T : new()
        {
            T proto = new T();
            foreach (ProtoSchema schema in Proto<T>.Schemas)
            {
                byte[] cbytes = stream.Read(sizeof(int));
                int count = BitConverter.ToInt32(cbytes, 0);

                byte[] payload = stream.Read(count);
                using (MemoryStream ms = new MemoryStream(payload))
                {
                    ms.Deserialize(proto, schema);
                }
            }

            return proto;
        }

        /// <summary>
        /// Reads a tuple from stream asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<T> ReadAsync<T>(this Stream stream, CancellationToken ct) where T : new()
        {
            T proto = new T();
            foreach (ProtoSchema schema in Proto<T>.Schemas)
            {
                byte[] cbytes = await stream.ReadAsync(sizeof(int), ct);
                int count = BitConverter.ToInt32(cbytes, 0);

                byte[] payload = await stream.ReadAsync(count, ct);
                using (MemoryStream ms = new MemoryStream(payload))
                {
                    ms.Deserialize(proto, schema);
                }
            }

            return proto;
        }

        /// <summary>
        /// Reads a tuple from stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="cancelTask"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<T> ReadAsync<T>(this Stream stream, Task cancelTask) where T : new()
        {
            Task<T> readTask = stream.ReadAsync<T>(CancellationToken.None);
            if (cancelTask == await Task.WhenAny(readTask, cancelTask))
            {
                cancelTask.Wait();
            }

            return readTask.Result;
        }

        public static void Write<T>(this Stream stream, T proto)
        {
            byte[] payload = ProtoUtils.GetBytes(proto);
            stream.Write(payload, 0, payload.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static Task WriteAsync<T>(this Stream stream, T proto)
        {
            return WriteAsync(stream, proto, CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static async Task WriteAsync<T>(this Stream stream, T proto, CancellationToken token)
        {
            byte[] payload = ProtoUtils.GetBytes(proto);
            await stream.WriteAsync(payload, 0, payload.Length, token);
        }

        /// <summary>
        /// Deseralize a byte array to an array of tuples with version marks
        /// </summary>
        /// <typeparam name="T">tuple type</typeparam>
        /// <param name="bytes">byte array</param>
        /// <returns>Deserialized tuple array</returns>
        public static IEnumerable<T> GetTuples<T>(byte[] bytes) where T : new()
        {
            if (bytes == null)
            {
                return Array<T>.Empty;
            }

            return GetTuples<T>(bytes, bytes.Length);
        }

        /// <summary>
        /// Deseralize a byte array to an array of tuples with version marks
        /// </summary>
        /// <typeparam name="T">tuple type</typeparam>
        /// <param name="bytes">byte array</param>
        /// <param name="length">bytes count</param>
        /// <returns>Deserialized tuple array</returns>
        public static IEnumerable<T> GetTuples<T>(byte[] bytes, int length) where T : new()
        {
            if (bytes == null || length == 0)
            {
                yield break;
            }

            int position = 0;

            while (position + sizeof(int) < length)
            {
                T tuple = new T();
                foreach (ProtoSchema schema in Proto<T>.Schemas)
                {
                    int count = BitConverter.ToInt32(bytes, position);
                    position += sizeof(int);

                    if (position + count <= length)
                    {
                        using (MemoryStream ms = new MemoryStream(bytes, position, count))
                        {
                            ms.Deserialize(tuple, schema);
                        }
                    }

                    position += count;
                }

                yield return tuple;

            }
        }

        /// <summary>
        /// Serializes key value pairs into byte array
        /// </summary>
        /// <param name="array">array of key value pairs</param>
        /// <returns>byte array</returns>
        public static byte[] GetBytes(KeyValuePair<string, object>[] array)
        {
            if (array == null || array.Length == 0)
            {
                return null;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    Int32Column intCol = new Int32Column();
                    StringColumn strCol = new StringColumn();

                    intCol.Write(writer, array.Length);
                    foreach (KeyValuePair<string, object> pair in array)
                    {
                        strCol.Write(writer, pair.Key);
                        strCol.Write(writer, pair.Value != null ? pair.Value.ToString() : string.Empty);
                    }
                }

                ms.Flush();
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Get key value pairs from serialized byte array
        /// </summary>
        /// <param name="bytes">byte array</param>
        /// <returns>key value pairs</returns>
        public static KeyValuePair<string, object>[] GetKeyValuePairs(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    Int32Column intCol = new Int32Column();
                    StringColumn strCol = new StringColumn();

                    int length = intCol.Read(reader);
                    KeyValuePair<string, object>[] result = new KeyValuePair<string, object>[length];

                    for (int index = 0; index < length; index++)
                    {
                        string key = strCol.Read(reader);
                        string value = strCol.Read(reader);
                        result[index] = new KeyValuePair<string, object>(key, value);
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <returns></returns>
        public static byte[] GetBytes(IPEndPoint ipEndPoint)
        {
            if (ipEndPoint == null)
            {
                return null;
            }

            using (MemoryStream outputStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(outputStream, Encoding.UTF8, true))
                {
                    byte[] ipBytes = ipEndPoint.Address.GetAddressBytes();
                    writer.Write(ipEndPoint.Port);
                    writer.Write(ipBytes.Length);
                    writer.Write(ipBytes, 0, ipBytes.Length);
                }

                outputStream.Flush();
                return outputStream.ToArray();
            }
        }


        /// <summary>
        /// Parse IPEndPiont from bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static IPEndPoint GetIPEndPoint(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            using (MemoryStream inputStream = new MemoryStream(bytes))
            {
                using (BinaryReader reader = new BinaryReader(inputStream, Encoding.UTF8, true))
                {
                    int port = reader.ReadInt32();
                    int len = reader.ReadInt32();
                    byte[] ipBytes = reader.ReadBytes(len);

                    IPAddress ipAddress = new IPAddress(ipBytes);
                    IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
                    return ipEndPoint;
                }
            }
        }

        // public static string GetFullName(this IProtoComponent component)
        // {
        //     if (component.Context != null && component.Context.Name != null)
        //     {
        //         return component.Context.Name + "$" + component.Name;
        //     }
        //     else
        //     {
        //         return component.Name;
        //     }
        // }

        // /// <summary>
        // /// 
        // /// </summary>
        // /// <typeparam name="T"></typeparam>
        // /// <param name="partitionSchema"></param>
        // /// <param name="tuple"></param>
        // /// <returns></returns>
        // public static int GetPartition<T>(this PartitionSchema partitionSchema, T proto)
        // {
        //     if (partitionSchema == null || partitionSchema.NumberOfPartitions == 0)
        //     {
        //         return 0;
        //     }

        //     IPartitionProto partitionProto = proto as IPartitionProto;
        //     if (partitionProto != null)
        //     {
        //         return Math.Abs(partitionProto.PartitionNumber) % partitionSchema.NumberOfPartitions;
        //     }

        //     if (partitionSchema.Columns == null || partitionSchema.Columns.Length == 0)
        //     {
        //         int hashCode = proto.GetHashCode();
        //         return Math.Abs(hashCode) % partitionSchema.NumberOfPartitions;
        //     }
        //     else
        //     {
        //         int hashCode = 0;
        //         foreach (IProtoColumn column in Proto<T>.Columns)
        //         {
        //             if (partitionSchema.Columns.Contains(column.Name))
        //             {
        //                 object value = proto.GetValue(column);
        //                 if (value != null)
        //                 {
        //                     hashCode ^= Math.Abs(value.GetHashCode());
        //                 }
        //             }
        //         }

        //         return Math.Abs(hashCode) % partitionSchema.NumberOfPartitions;
        //     }
        // }

        // /// <summary>
        // /// Gets partitions assigned to a component
        // /// </summary>
        // /// <param name="dep">proto deployment</param>
        // /// <param name="component">proto component</param>
        // /// <param name="includeBackups">include backup partitions</param>
        // /// <returns>partitions assigned to the component</returns>
        // public static IEnumerable<int> GetPartitions(this ProtoDeployment dep, IProtoComponent component, bool includeBackups = false)
        // {
        //     string partitionConfig = dep.GetConfiguration(ProtoUtils.PartitionConfigureName);

        //     string primaryPartitionsConfig = dep.GetConfiguration(component.Name, ProtoUtils.PrimaryPartitionsConfigureName);
        //     if (string.IsNullOrEmpty(primaryPartitionsConfig))
        //     {
        //         primaryPartitionsConfig = dep.GetConfiguration(ProtoUtils.PrimaryPartitionsConfigureName);
        //     }

        //     string backupPartitionsConfig = null;
        //     if (includeBackups)
        //     {
        //         backupPartitionsConfig = dep.GetConfiguration(component.Name, ProtoUtils.BackupPartitionsConfigureName);
        //         if (string.IsNullOrEmpty(backupPartitionsConfig))
        //         {
        //             primaryPartitionsConfig = dep.GetConfiguration(ProtoUtils.BackupPartitionsConfigureName);
        //         }
        //     }

        //     return new Partitions(partitionConfig, primaryPartitionsConfig, backupPartitionsConfig);
        // }

        /// <summary>
        /// Returns current machine IPAddress
        /// </summary>
        /// <returns>current IPAddress</returns>
        // public static IPAddress GetServerIPAddress()
        // {
        //     return GetServerIPAddress(Dns.GetHostName());
        // }

        // /// <summary>
        // /// Returns current machine IPAddress
        // /// </summary>
        // /// <returns>current IPAddress</returns>
        // public static IPAddress GetServerIPAddress(string serverName)
        // {
        //     IPAddress ipAddress = Dns.GetHostAddresses(serverName).First(ipa => ipa.AddressFamily == AddressFamily.InterNetwork);
        //     return ipAddress;
        // }
            
        public static void GetServerPort(ref string serverName, ref int port)
        {
            if (ProtoUtils.LocalRun)
            {
                string key = string.Format("{0}:{1}", serverName, port);
                string mapped = serverPortMapper.GetOrAdd(key, k => string.Format("{0}:{1}", System.Environment.MachineName, Interlocked.Increment(ref testPort)));

                string[] fields = mapped.Split(':');
                serverName = fields[0];
                port = int.Parse(fields[1]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName">remote server name</param>
        /// <param name="serverName">server port numbert</param>
        /// <returns>IPEndPoint to the server and port</returns>
        // public static IPEndPoint DnsResolve(string serverName, int port)
        // {
        //     if (ProtoUtils.LocalRun)
        //     {
        //         string key = string.Format("{0}:{1}", serverName, port);
        //         string mapped = serverPortMapper.GetOrAdd(key, k => string.Format("{0}:{1}", System.Environment.MachineName, Interlocked.Increment(ref testPort)));

        //         string[] fields = mapped.Split(':');
        //         serverName = fields[0];
        //         port = int.Parse(fields[1]);
        //     }

        //     IPAddress ipAddress = Dns.GetHostAddresses(serverName).First(ipa => ipa.AddressFamily == AddressFamily.InterNetwork);
        //     return new IPEndPoint(ipAddress, port);
        // }

        #region Serialize and Deserialize

        /// <summary>
        /// Serialize a tuple into stream.
        /// 
        /// The following data are writen into the stream
        /// 1. bitmask bytes length 
        /// 2. bitmarks bytes
        /// 3. serialized value for each column in columns masks.
        /// 
        /// </summary>
        /// <typeparam name="T">tuple type</typeparam>
        /// <param name="tuple">tuple object</param>
        /// <param name="stream">the stread to write the tuple into</param>
        private static void Serialize<T>(this Stream stream, T tuple)
        {
           foreach (ProtoSchema schema in Proto<T>.Schemas)
           {
               stream.Serialize(tuple, schema);
           }
        }

        /// <summary>
        /// Serialize a tuple into stream.
        /// 
        /// The following data are writen into the stream
        /// 1. bitmask bytes length 
        /// 2. bitmarks bytes
        /// 3. serialized value for each column in columns masks.
        /// 
        /// </summary>
        /// <typeparam name="T">tuple type</typeparam>
        /// <param name="tuple">tuple object</param>
        /// <param name="stream">the stread to write the tuple into</param>
        /// <param name="columnMasks">column masks for serialized columns</param>
        public static void Serialize<T>(this Stream stream, T proto, ProtoSchema schema)
        {
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                int bitPos = (int)stream.Position;
                byte[] bitmarks = new byte[schema.ColumnMasks.Length];
                writer.Write((byte)bitmarks.Length);
                writer.Write(bitmarks, 0, bitmarks.Length);

                for (int index = 0; index < schema.Columns.Length; index++)
                {
                    IProtoColumn column = schema.Columns[index];
                    if (column.ID >= schema.ColumnMasks.Length * 8)
                    {
                        break;
                    }

                    if (!schema.ColumnMasks.GetBitmask(column.ID))
                    {
                        bitmarks.SetBitmask(column.ID, false);
                        continue;
                    }

                    object value = proto.GetValue(column);
                    if (CommonUtils.Equals(value, column.DefaultValue))
                    {
                        bitmarks.SetBitmask(column.ID, false);
                    }
                    else
                    {
                        bitmarks.SetBitmask(column.ID, true);
                        column.Write(writer, value);
                    }
                }

                int endPos = (int)stream.Position;
                stream.Position = bitPos;

                writer.Write((byte)bitmarks.Length);
                writer.Write(bitmarks, 0, bitmarks.Length);

                stream.Position = endPos;
            }
        }

        /// <summary>
        /// Tries deserialize a tuple from stream
        /// </summary>
        /// <typeparam name="T">tuple type</typeparam>
        /// <param name="tuple">tuple object</param>
        /// <param name="stream">the stream to read the tuple</param>
        /// <returns>true when success</returns>
        public static void Deserialize<T>(this Stream stream, T proto, ProtoSchema schema)
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, true))
                {
                    int bitLen = reader.ReadByte();
                    byte[] bitmasks = reader.ReadBytes(bitLen);

                    foreach (IProtoColumn column in schema.Columns)
                    {
                        if (bitmasks.GetBitmask(column.ID))
                        {
                            object value = column.Read(reader);
                            proto.SetValue(column, value);
                        }
                        else
                        {
                            proto.SetValue(column, column.DefaultValue);
                        }
                    }
                }
            }
            catch (EndOfStreamException)
            {
                throw new ProtoException(ProtoErrorCode.DataCorrupted);
            }
        }

        #endregion

        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="name">topology host name</param>
        // /// <param name="machine">machine name</param>
        // /// <param name="location">topology host location</param>
        // /// <returns>topology host</returns>
        // public static TopologyHost CreateTopologyHost(string name, string machine, string location)
        // {
        //     TopologyHost topologyHost = new TopologyHost
        //     {
        //         Name = name,
        //     };

        //     topologyHost.Deployment = new ProtoDeployment
        //     {
        //         Machine = machine,
        //         Location = location
        //     };

        //     return topologyHost;
        // }

        /// <summary>
        /// Creates a standalone proto component
        /// </summary>
        /// <typeparam name="T">component type</typeparam>
        /// <param name="name">component name</param>
        /// <param name="location">working directory</param>
        /// <returns>new component instance</returns>
        // public static T Start<T>(string location, T comp) where T : IProtoComponent
        // {
        //     if (location != null)
        //     {
        //         location = Path.Combine(Environment.CurrentDirectory, location);
        //     }
        //     else
        //     {
        //         location = Environment.CurrentDirectory;
        //     }

        //     ProtoApp context = new ProtoApp
        //     {
        //         Name = comp.Name + ".app",
        //         Deployment = new ProtoDeployment { Machine = Environment.MachineName, Location = location }
        //     };

        //     comp.Context = context;

        //     context.Start();
        //     comp.Start();
        //     return comp;
        // }

        /// <summary>
        /// Tries get value from a watermark service
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="ws">watermark service</param>
        /// <param name="name">watermark name</param>
        /// <param name="tryParse">tryParse function</param>
        /// <param name="value">parsed value</param>
        /// <returns></returns>
        // public static bool TryGet<T>(this IWatermarkService ws, string name, TryParseDelegate<T> tryParse, out T value)
        // {
        //     string watermark = ws[name];
        //     return tryParse(watermark, out value);
        // }

    }
}