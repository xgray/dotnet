
namespace ProtoInsight
{
    /// <summary>
    /// IProto provides property bag interface for a strong typed object
    /// </summary>
    public interface IPartitionProto
    {
        /// <summary>
        /// Partition number
        /// </summary>
        int PartitionNumber { get; }
    }
}
