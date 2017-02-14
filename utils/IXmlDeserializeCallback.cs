namespace Bench
{
    /// <summary>
    /// Interface to provide a fixup and validate callback method after Xml deserialization.
    /// </summary>
    public interface IXmlDeserializeCallback
    {
        /// <summary>
        /// Validation method called after xml deserialze.
        /// </summary>
        /// <returns>true if the object is valid</returns>
        bool OnDeserialize();
    }
}
