
namespace Bench
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    public static partial class CommonUtils
    {
        /// <summary>
        /// DateTime format string
        /// </summary>
        public const string DateTimeFormatString = "yyyy-MM-ddTHH:mm:ss";

        /// <summary>
        /// TimeSpan format string
        /// </summary>
        public const string TimeSpanFormatString = @"d\.hh\:mm\:ss";

        /// <summary>
        /// Default log category
        /// </summary>
        public const string DefaultCategory = "-";

        /// <summary>
        /// A look up table of the number of bits for each byte value
        /// </summary>
        private static int[] bitsCountLookup;

        /// <summary>
        /// 
        /// </summary>
        [ThreadStatic]
        private static Random random;

        /// <summary>
        /// End of line bytes for UTF8 encoding.
        /// </summary>
        public static readonly byte[] EOLBytes = Encoding.UTF8.GetBytes(Environment.NewLine);

        /// <summary>
        /// Empty Bytes Array
        /// </summary>
        public static readonly byte[] EmptyBytes = new byte[0];

        /// <summary>
        /// Create one base time instance to be used by everybody
        /// </summary>
        public static readonly DateTime BaseTime = new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        ///  Initializes static members of the CommonUtils class
        /// </summary>
        static CommonUtils()
        {
            int[] lookup = new int[256];
            for (int index = 0; index < 245; index++)
            {
                int bits = 0;
                int bitVal = index;

                while (bitVal > 0)
                {
                    bits += bitVal & 1;
                    bitVal <<= 1;
                }

                lookup[index] = bits;
            }

            bitsCountLookup = lookup;
        }

        public static Random Random
        {
            get
            {
                if (CommonUtils.random == null)
                {
                    CommonUtils.random = new Random();
                }

                return CommonUtils.random;
            }
        }


        #region Diagnostics Utilities

        /// <summary>
        /// throws argument exception if the specified condition is true
        /// </summary>
        /// <param name="condition">condition to be evluated</param>
        /// <param name="msg">exception message</param>
        /// <param name="args">exception arguments</param>
        public static void ThrowIfTrue(bool condition, string msg = null, params object[] args)
        {
            if (condition)
            {
                string message = CommonUtils.Format(msg, args);
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// throws argument exception if the specified condition is false
        /// </summary>
        /// <param name="condition">condition to be evluated</param>
        /// <param name="msg">exception message</param>
        /// <param name="args">exception arguments</param>
        public static void ThrowIfFalse(bool condition, string msg = null, params object[] args)
        {
            if (condition == false)
            {
                string message = CommonUtils.Format(msg, args);
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// throws argument exception if the specified string is null or empty
        /// </summary>
        /// <param name="condition">condition to be evluated</param>
        /// <param name="msg">exception message</param>
        /// <param name="args">exception arguments</param>
        public static void ThrowIfNullOrEmpty(string str, string msg = null, params object[] args)
        {
            if (string.IsNullOrEmpty(str))
            {
                string message = CommonUtils.Format(msg, args);
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// throws argument exception if the argument is true
        /// </summary>
        /// <param name="arg">argument to check null</param>
        /// <param name="msg">exception message</param>
        /// <param name="args">exception arguments</param>
        public static void ThrowIfNull(object arg, string msg = null, params object[] args)
        {
            if (arg == null)
            {
                string message = CommonUtils.Format(msg, args);
                throw new ArgumentNullException(message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void LogVerbose(string msg, params object[] args)
        {
            CommonUtils.LogVerbose(WorkingContext.Current, msg, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void LogInfo(string msg, params object[] args)
        {
            CommonUtils.LogInfo(WorkingContext.Current, msg, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void LogWarning(string msg, params object[] args)
        {
            CommonUtils.LogWarning(WorkingContext.Current, msg, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void LogError(string msg, params object[] args)
        {
            CommonUtils.LogError(WorkingContext.Current, msg, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        /// <param name="printStack"></param>
        public static void LogException(Exception ex, bool printStack = false)
        {
            CommonUtils.LogException(WorkingContext.Current, ex, printStack);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void LogVerbose(this IWorkingContext context, string msg, params object[] args)
        {
            string component = GetCallingMethod(2);
            context.Log(component, TraceLevel.Verbose, CommonUtils.DefaultCategory, msg, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void LogInfo(this IWorkingContext context, string msg, params object[] args)
        {
            string component = GetCallingMethod(2);
            context.Log(component, TraceLevel.Info, CommonUtils.DefaultCategory, msg, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void LogWarning(this IWorkingContext context, string msg, params object[] args)
        {
            string component = GetCallingMethod(2);
            context.Log(component, TraceLevel.Warning, CommonUtils.DefaultCategory, msg, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public static void LogError(this IWorkingContext context, string msg, params object[] args)
        {
            string component = GetCallingMethod(2);
            context.Log(component, TraceLevel.Error, CommonUtils.DefaultCategory, msg, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        /// <param name="printStack"></param>
        public static void LogException(this IWorkingContext context, Exception ex, bool printStack = false)
        {
            string component = GetCallingMethod(2);
            string exceptionStr = ex.FormatException(printStack);
            context.Log(component, TraceLevel.Error, CommonUtils.DefaultCategory, exceptionStr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consolePrefix"></param>
        /// <param name="msg"></param>
        public static void Print(string consolePrefix, string msg)
        {
            Console.WriteLine(consolePrefix + ":" + msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string GetCallingMethod(int level = 1)
        {
#if CORE
			return "NOT_SUPPORTED";
#else
            var sf = new StackTrace(level);
            var frame = sf.GetFrame(0);
            var mi = frame.GetMethod();
            return mi.DeclaringType.Name + "." + mi.Name;
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(string msg, params object[] args)
        {
            string message = args.Length == 0 ? msg : string.Format(msg, args);
            return message;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="lineWidth"></param>
        /// <returns></returns>
        public static string FormatBytes(byte[] bytes, int lineWidth = 16)
        {
            StringBuilder output = new StringBuilder();
            StringBuilder left = new StringBuilder();
            StringBuilder right = new StringBuilder();

            for (int index = 0; index < bytes.Length; index += 16)
            {
                left.Length = 0;
                right.Length = 0;

                int j = index;
                while (j < bytes.Length && j < index + 16)
                {
                    left.AppendFormat("{0:X2} ", bytes[j]);

                    if (bytes[j] >= 0x20 && bytes[j] <= 0xfe)
                    {
                        right.Append((char)bytes[j]);
                    }
                    else
                    {
                        right.Append(".");
                    }
                    j++;
                }

                while (j < index + 16)
                {
                    left.Append("   ");
                    right.Append(" ");
                    j++;
                }

                output.Append(left).Append(right).AppendLine();
            }

            return output.ToString();
        }

        #endregion

        #region Serialize Utilities

        /// <summary>
        ///
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int BitmaskSize(int length)
        {
            return (length + 7) / 8;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmarks"></param>
        /// <returns></returns>
        public static int BitmasksCount(this byte[] bitmarks)
        {
            return bitmarks.Sum(b => bitsCountLookup[b]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmarks"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public static void SetBitmask(this byte[] bitmarks, int index, bool value)
        {
            int bmIndex = index / 8;
            byte maskValue = (byte)(1 << (index % 8));
            if (value)
            {
                bitmarks[bmIndex] |= maskValue;
            }
            else
            {
                bitmarks[bmIndex] &= (byte)(~maskValue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmarks"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool GetBitmask(this byte[] bitmarks, int index)
        {
            int bmIndex = index / 8;
            byte maskValue = (byte)(1 << (index % 8));
            return bmIndex < bitmarks.Length && (bitmarks[bmIndex] & maskValue) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string GetXml<T>(this T instance)
        {
            XDocument xdoc = new XDocument();
            XmlSerializer ser = new XmlSerializer(instance.GetType());
            using (XmlWriter xdocWriter = xdoc.CreateWriter())
            {
                ser.Serialize(xdocWriter, instance);
                xdocWriter.Flush();
            }

            return xdoc.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T Parse<T>(string xml)
        {
            using (StringReader sr = new StringReader(xml))
            {
                using (XmlReader xr = XmlReader.Create(sr))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(T));
                    T deserializedObject = (T)ser.Deserialize(xr);

                    IXmlDeserializeCallback callback = deserializedObject as IXmlDeserializeCallback;
                    if (callback != null)
                    {
                        callback.OnDeserialize();
                    }

                    return deserializedObject;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <param name="deserializedObject"></param>
        /// <returns></returns>
        public static bool TryParse<T>(string xml, out T deserializedObject)
        {
            deserializedObject = default(T);

            try
            {
                deserializedObject = Parse<T>(xml);
                return true;
            }
            catch (Exception exception)
            {
                if (!exception.IsKnownXmlSerializationException())
                {
                    throw;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a property bag with all public properties of message object .
        /// </summary>
        /// <param name="message">message object</param>
        /// <returns>property bag</returns>
        public static Dictionary<string, string> ToDictionary(this object message)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            foreach (var prop in message.GetType().GetProperties())
            {
                if (!prop.CanRead)
                {
                    continue;
                }

                object propVal = prop.GetValue(message);
                if (propVal != null)
                {
                    properties.Add(prop.Name, propVal.ToString());
                }
            }

            return properties;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seq"></param>
        /// <returns></returns>
        public static IEnumerable<int> ParseSequence(string seq)
        {
            foreach (string s in seq.Split(',', ';'))
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    continue;
                }

                int value;
                if (int.TryParse(s, out value))
                {
                    yield return value;
                    continue;
                }

                string[] range = s.Split('-');
                int l, h;
                if (range.Length == 2 &&
                    int.TryParse(range[0], out l) &&
                    int.TryParse(range[1], out h) &&
                    l <= h)
                {
                    for (int i = l; i <= h; i++)
                    {
                        yield return i;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="escapeChar"></param>
        /// <param name="seperateChars"></param>
        /// <returns></returns>
        public static string Escape(string str, char escapeChar, params char[] seperateChars)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in EscapeChars(str, escapeChar, seperateChars))
            {
                sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="escapeChar"></param>
        /// <param name="seperateChars"></param>
        /// <returns></returns>
        public static IEnumerable<char> EscapeChars(IEnumerable<char> str, char escapeChar, params char[] seperateChars)
        {
            foreach (char ch in str)
            {
                if (ch == '\n')
                {
                    yield return escapeChar;
                    yield return 'n';
                }
                else if (ch == '\r')
                {
                    yield return escapeChar;
                    yield return 'r';
                }
                else if (ch == '\t')
                {
                    yield return escapeChar;
                    yield return 't';
                }
                //else if (ch == seperateChar)
                //{
                //    yield return escapeChar;
                //    yield return 'u';
                //}
                else if (ch == escapeChar)
                {
                    yield return escapeChar;
                    yield return escapeChar;
                }
                else
                {
                    bool isSep = false;
                    for (int i = 0; i < seperateChars.Length; i++)
                    {
                        if (ch == seperateChars[i])
                        {
                            isSep = true;
                            yield return escapeChar;
                            yield return (char)('u' + i);
                            break;
                        }
                    }

                    if (!isSep)
                    {
                        yield return ch;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="escapeChar"></param>
        /// <param name="seperateChars"></param>
        /// <returns></returns>
        public static string Unescape(string str, char escapeChar, params char[] seperateChars)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in UnescapeChars(str, escapeChar, seperateChars))
            {
                sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="escapeChar"></param>
        /// <param name="seperateChars"></param>
        /// <returns></returns>
        public static IEnumerable<char> UnescapeChars(IEnumerable<char> str, char escapeChar, params char[] seperateChars)
        {
            bool escaped = false;
            foreach (char ch in str)
            {
                if (ch == 0xFEFF)
                {
                    continue;
                }

                if (escaped)
                {
                    escaped = false;
                    if (ch == 'n')
                    {
                        yield return '\n';
                    }
                    else if (ch == 'r')
                    {
                        yield return '\r';
                    }
                    else if (ch == 't')
                    {
                        yield return '\t';
                    }
                    //else if (ch == 'u')
                    //{
                    //    yield return seperateChar;
                    //}
                    else if (ch == escapeChar)
                    {
                        yield return escapeChar;
                    }
                    else
                    {
                        bool isSep = false;
                        for (int i = 0; i < seperateChars.Length; i++)
                        {
                            if (ch == 'u' + i)
                            {
                                isSep = true;
                                yield return seperateChars[i];
                                break;
                            }
                        }

                        if (!isSep)
                        {
                            yield return ch;
                        }
                    }
                }
                else
                {
                    if (ch == escapeChar)
                    {
                        escaped = true;
                    }
                    else
                    {
                        yield return ch;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(string value)
        {
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(byte value)
        {
            return value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte ToByte(string str)
        {
            byte byteValue = default(byte);
            byte.TryParse(str, out byteValue);
            return byteValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(Enum value)
        {
            return value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToEnum<T>(string str)
        {
            return (T)Enum.Parse(typeof(T), str);
        }

        public static bool TryEnum<T>(string str, out T value) where T : struct
        {
            return Enum.TryParse<T>(str, out value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(short value)
        {
            return value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static short ToInt16(string str)
        {
            short shortValue = default(short);
            short.TryParse(str, out shortValue);
            return shortValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(ushort value)
        {
            return value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ushort ToUInt16(string str)
        {
            ushort shortValue = default(ushort);
            ushort.TryParse(str, out shortValue);
            return shortValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(int value)
        {
            return value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt32(string str)
        {
            int intValue = default(int);
            int.TryParse(str, out intValue);
            return intValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(uint value)
        {
            return value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static uint ToUInt32(string str)
        {
            uint intValue = default(uint);
            uint.TryParse(str, out intValue);
            return intValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(long value)
        {
            return value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long ToInt64(string str)
        {
            long longValue = default(long);
            long.TryParse(str, out longValue);
            return longValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(ulong value)
        {
            return value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ulong ToUInt64(string str)
        {
            ulong longValue = default(ulong);
            ulong.TryParse(str, out longValue);
            return longValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(byte[] value)
        {
            return value == null ? string.Empty : Convert.ToBase64String(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToBytes(string str)
        {
            return string.IsNullOrEmpty(str) ? null : Convert.FromBase64String(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(bool value)
        {
            return value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ToBoolean(string str)
        {
            bool boolValue = default(bool);
            bool.TryParse(str, out boolValue);
            return boolValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(double value)
        {
            return value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static double ToDouble(string str)
        {
            double doubleValue = default(double);
            double.TryParse(str, out doubleValue);
            return doubleValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(DateTime value)
        {
            DateTime utcValue = value.ToUniversalTime();
            return utcValue.ToString(DateTimeFormatString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(string str)
        {
            DateTime dtValue = DateTime.ParseExact(
                str, 
                DateTimeFormatString, 
                CultureInfo.InvariantCulture, 
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            return dtValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(TimeSpan value)
        {
            return value.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan(string str)
        {
            TimeSpan tsValue = TimeSpan.Parse(str);
            return tsValue;
        }

        /// <summary>
        /// Converts object to json string
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="obj">object instance</param>
        /// <returns>json string of the object</returns>
        public static string ToJson<T>(this T obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Convert json string to object
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="json">json string</param>
        /// <returns>object instance</returns>
        public static T FromJson<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        #endregion

        #region Async Utilities

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="beginMethod"></param>
        /// <param name="endMethod"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static Task FromAsync<T1>(Func<T1, AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, T1 t1)
        {
            return Task.Factory.FromAsync(beginMethod, endMethod, t1, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="beginMethod"></param>
        /// <param name="endMethod"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static Task FromAsync<T1, T2>(Func<T1, T2, AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, T1 t1, T2 t2)
        {
            return Task.Factory.FromAsync(beginMethod, endMethod, t1, t2, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="beginMethod"></param>
        /// <param name="endMethod"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <returns></returns>
        public static Task FromAsync<T1, T2, T3>(Func<T1, T2, T3, AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, T1 t1, T2 t2, T3 t3)
        {
            return Task.Factory.FromAsync(beginMethod, endMethod, t1, t2, t3, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="beginMethod"></param>
        /// <param name="endMethod"></param>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static Task<TR> FromAsync<T1, TR>(Func<T1, AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TR> endMethod, T1 t1)
        {
            return Task<TR>.Factory.FromAsync(beginMethod, endMethod, t1, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="beginMethod"></param>
        /// <param name="endMethod"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static Task<TR> FromAsync<T1, T2, TR>(Func<T1, T2, AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TR> endMethod, T1 t1, T2 t2)
        {
            return Task<TR>.Factory.FromAsync(beginMethod, endMethod, t1, t2, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="beginMethod"></param>
        /// <param name="endMethod"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <returns></returns>
        public static Task<TR> FromAsync<T1, T2, T3, TR>(Func<T1, T2, T3, AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TR> endMethod, T1 t1, T2 t2, T3 t3)
        {
            return Task<TR>.Factory.FromAsync(beginMethod, endMethod, t1, t2, t3, null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="beginMethod"></param>
        /// <param name="endMethod"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <returns></returns>
        public static Task FromAsync<T1, T2, T3, T4>(Func<T1, T2, T3, T4, AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            beginMethod(t1, t2, t3, t4, ar =>
                {
                    try
                    {
                        endMethod(ar);
                        tcs.TrySetResult(1);
                    }
                    catch (OperationCanceledException)
                    {
                        tcs.TrySetCanceled();
                    }
                    catch (Exception e)
                    {
                        tcs.TrySetException(e);
                    }
                }, null);

            return tcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="beginMethod"></param>
        /// <param name="endMethod"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <returns></returns>
        public static Task<TR> FromAsync<T1, T2, T3, T4, TR>(Func<T1, T2, T3, T4, AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TR> endMethod, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            TaskCompletionSource<TR> tcs = new TaskCompletionSource<TR>();
            beginMethod(t1, t2, t3, t4, ar =>
                {
                    try
                    {
                        TR result = endMethod(ar);
                        tcs.TrySetResult(result);
                    }
                    catch (OperationCanceledException)
                    {
                        tcs.TrySetCanceled();
                    }
                    catch (Exception e)
                    {
                        tcs.TrySetException(e);
                    }
                }, null);

            return tcs.Task;
        }

        public static Task FromAsync<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, AsyncCallback, object, IAsyncResult> beginMethod, Action<IAsyncResult> endMethod, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            beginMethod(t1, t2, t3, t4, t5, ar =>
                {
                    try
                    {
                        endMethod(ar);
                        tcs.TrySetResult(1);
                    }
                    catch (OperationCanceledException)
                    {
                        tcs.TrySetCanceled();
                    }
                    catch (Exception e)
                    {
                        tcs.TrySetException(e);
                    }
                }, null);

            return tcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="beginMethod"></param>
        /// <param name="endMethod"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="t4"></param>
        /// <param name="t5"></param>
        /// <returns></returns>
        public static Task<TR> FromAsync<T1, T2, T3, T4, T5, TR>(Func<T1, T2, T3, T4, T5, AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TR> endMethod, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            TaskCompletionSource<TR> tcs = new TaskCompletionSource<TR>();
            beginMethod(t1, t2, t3, t4, t5, ar =>
                {
                    try
                    {
                        TR result = endMethod(ar);
                        tcs.TrySetResult(result);
                    }
                    catch (OperationCanceledException)
                    {
                        tcs.TrySetCanceled();
                    }
                    catch (Exception e)
                    {
                        tcs.TrySetException(e);
                    }
                }, null);

            return tcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static TapAwaiter<DateTime> GetAwaiter(this TimeSpan timeSpan)
        {
            TapAwaiter<DateTime> awaiter = new TapAwaiter<DateTime>
            {
                TimeSpan = timeSpan,
                Result = DateTime.UtcNow
            };

            return awaiter;
        }


        #endregion

        #region Enumerable Utilities

        /// <summary>
        /// Make an Enumerable from parameters
        /// </summary>
        /// <typeparam name="T">item type</typeparam>
        /// <param name="args">input args</param>
        /// <returns>Enumerable from args</returns>
        public static IEnumerable<T> MakeEnumerable<T>(params T[] args)
        {
            return args;
        }


        #endregion

        #region Action utilities

        /// <summary>
        /// Calls actions with retry.
        /// </summary>
        /// <param name="attempts">attempts count</param>
        /// <param name="interval">retry interval</param>
        /// <param name="action">an action delegate.</param>
        /// <param name="func">whether the exception is retriable.</param>
        public static void CallActionWithRetry(int attempts, int interval, Action action, Func<Exception, bool> func)
        {
            while (attempts-- > 0)
            {
                try
                {
                    action();
                    break;
                }
                catch (Exception ex)
                {
                    if (func(ex))
                    {
                        System.Threading.Thread.Sleep(interval);
                        continue;
                    }

                    throw;
                }
            }
        }

        /// <summary>
        /// Invokes an action and catch all exceptions
        /// </summary>
        /// <param name="action">action to be invoked</param>
        public static void SafeAction(Action action)
        {
            try
            {
                action();
            }
            catch
            {
            }
        }

        #endregion

        #region Url utilities

        public static Uri CombineUrl(this Uri baseUri, string relative)
        {
            return new Uri(baseUri, relative);
        }

        public static string CombineUrl(this string baseUrl, string relative)
        {
            Uri baseUri = new Uri(baseUrl);
            return CombineUrl(baseUri, relative).ToString().Trim();
        }

        public static Uri ToUri(this string url)
        {
            return new Uri(url);
        }

        #endregion

        #region File and directory utilities

        public static void CreateDirectoryIfNecessary(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public static void DeleteDirectoryIfNecessary(string directory, bool recursive = true)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive);
            }
        }

        /// <summary>
        /// Updates file timestamp
        /// </summary>
        /// <param name="path">file path</param>
        public static void TouchFile(string path)
        {
            TouchFile(path, DateTime.UtcNow);
        }

        /// <summary>
        /// Updates file timestamp
        /// </summary>
        /// <param name="path">file path</param>
        /// <param name="dateTime">time stamp</param>
        public static void TouchFile(string path, DateTime dateTime)
        {

            FileInfo fi = new FileInfo(path);
            fi.LastWriteTimeUtc = dateTime;
        }

        public static FileStream OpenFileForRead(string filePath)
        {
            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        #endregion
    }

}
