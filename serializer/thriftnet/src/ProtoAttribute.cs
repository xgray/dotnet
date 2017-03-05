
namespace Thrift.Net
{
    using System;

    /// <summary>
    /// ProtoAttribute denotes the type can be serialized by Proto Serializer.
    /// Only public properties and fields that are decorated by ProtoColumn attribute 
    /// will be serialized. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ProtoAttribute : Attribute
    {
    }
}
