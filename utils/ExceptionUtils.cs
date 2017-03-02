
namespace Bench
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Xml;

    public static partial class ExceptionUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public static void PrintException(this Exception ex)
        {
            string str = FormatException(ex);
            Console.WriteLine(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="printStack"></param>
        /// <returns></returns>
        public static string FormatException(this Exception exception, bool printStack = false)
        {
            StringBuilder sb = new StringBuilder();

            while (exception != null)
            {
                if (exception is AggregateException)
                {
                }
                else
                {
                    sb.AppendLine(printStack ? exception.ToString() : exception.Message);
                }

                exception = exception.InnerException;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Checks if a given exception is fatal. If so, no attempt should be made to handle it
        /// </summary>
        /// <param name="e">Exception object</param>
        /// <returns>whether exception is fatal</returns>
        public static bool IsKnownFatalException(this Exception e)
        {
#if CORE
			return e is OutOfMemoryException
				|| e is BadImageFormatException
				|| e is TypeInitializationException
				|| e is TypeLoadException
				|| e is SEHException;
#else
            return e is OutOfMemoryException
                || e is AppDomainUnloadedException
                || e is BadImageFormatException
                || e is TypeInitializationException
                || e is TypeLoadException
                || e is TypeUnloadedException
                || e is InternalBufferOverflowException
                || e is ThreadAbortException
                || e is AccessViolationException
                || e is SEHException
                || e is StackOverflowException;
#endif
        }

        /// <summary>
        /// Checks whether given exception is known file read/write exception.
        /// </summary>
        /// <param name="exception">Exception object</param>
        /// <returns>true if exception is known</returns>
        public static bool IsKnownFileException(this Exception exception)
        {
            return
                exception is SerializationException
                || exception is FileNotFoundException
                || exception is UnauthorizedAccessException
                || exception is SecurityException
                || exception is PathTooLongException
                || exception is NotSupportedException
                || exception is DirectoryNotFoundException
                || exception is IOException;
        }

        /// <summary>
        /// Checks if the exception is one of the known system exceptions
        /// </summary>
        /// <param name="exception">The exception to check</param>
        /// <returns>True if the exception is known</returns>
        public static bool IsKnownSystemException(this Exception exception)
        {
#if CORE
			return exception is ArgumentException
				|| exception is ArithmeticException
				|| exception is FormatException
				|| exception is IndexOutOfRangeException
				|| exception is InvalidCastException
				|| exception is InvalidOperationException
				|| exception is UnauthorizedAccessException
				|| exception is KeyNotFoundException;
#else
            return exception is AccessViolationException
                || exception is ArgumentException
                || exception is ArithmeticException
                || exception is FormatException
                || exception is IndexOutOfRangeException
                || exception is InvalidCastException
                || exception is InvalidOperationException
                || exception is UnauthorizedAccessException
                || exception is KeyNotFoundException;
#endif
        }

        /// <summary>
        /// Checks if the exception is one of the known xml serializatioin exceptions
        /// </summary>
        /// <param name="exception">The exception to check</param>
        /// <returns>True if the exception is known</returns>
        public static bool IsKnownXmlSerializationException(this Exception exception)
        {
            return exception is ArgumentNullException
                || exception is InvalidOperationException
                || exception is NullReferenceException
                || exception is XmlException;
        }

        /// <summary>
        /// Checks if the exception is one of the known assembly loading exceptions
        /// </summary>
        /// <param name="exception">The exception to check</param>
        /// <returns>True if the exception is known</returns>
        public static bool IsKnownAssemblyLoadingException(this Exception exception)
        {
            return
                exception is ArgumentNullException ||
                exception is ArgumentException ||
                exception is FileNotFoundException ||
                exception is FileLoadException ||
                exception is BadImageFormatException;
        }

        /// <summary>
        /// Checks if the exception is one of the known network comunication exceptions
        /// </summary>
        /// <param name="exception">The exception to check</param>
        /// <returns>True if the exception is known</returns>
        public static bool IsKnownNetworkingException(this Exception exception)
        {
            return exception is SocketException ||
                exception is OperationCanceledException ||
                exception is ObjectDisposedException ||
                exception is EndOfStreamException ||
                (exception is AggregateException &&
                exception.cast<AggregateException>().InnerException is OperationCanceledException);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="se"></param>
        public static void HandleSocketException(this SocketException se)
        {
            if (se.SocketErrorCode == SocketError.TimedOut)
            {
            }
            else if (se.SocketErrorCode == SocketError.MessageSize)
            {
            }
            else
            {
                Console.WriteLine("ErrorCode = {0}, {1}", se.SocketErrorCode, se.Message);
            }
        }
    }
}
