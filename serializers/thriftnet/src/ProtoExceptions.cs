
namespace ProtoThrift
{
    using System;
    using Bench;
    public enum ProtoErrorCode : int
    {
        Unspecified = 0,
        InvalidArgument = 1,
        InvalidConfiguration = 2,
        InvalidState = 3,
        DataCorrupted = 4,
    }

    public class ProtoException : Exception
    {
        public ProtoException(ProtoErrorCode error, Exception innerException, string msg, params object[] args)
            : base(CommonUtils.Format(msg, args), innerException)
        {
            this.ErrorCode = error;
        }

        public ProtoException(Exception innerException, string msg, params object[] args)
            : this(ProtoErrorCode.Unspecified, innerException, msg, args)
        {
        }

        public ProtoException(ProtoErrorCode error, string msg, params object[] args)
            : this(error, null, msg, args)
        {
        }

        public ProtoException(string msg, params string[] args)
            : this(null, msg, args)
        {
        }

        public ProtoException(ProtoErrorCode error)
            : this(error, (Exception)null, error.ToString())
        {
        }

        internal ProtoException()
            : this(ProtoErrorCode.Unspecified)
        {
        }

        public ProtoErrorCode ErrorCode
        {
            get;
            private set;
        }
    }
}