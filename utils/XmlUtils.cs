
namespace Bench
{
  using System.Xml;

  public interface IXmlDeserializeCallback
  {
    /// <summary>
    /// Validation method called after xml deserialze.
    /// </summary>
    /// <returns>true if the object is valid</returns>
    bool OnDeserialize();
  }

  public static partial class XmlUtils
  {
    public static string ReadElementString(this XmlReader reader)
    {
      if (reader.IsEmptyElement)
      {
        return string.Empty;
      }

      reader.Read();
      if (reader.NodeType == XmlNodeType.EndElement)
      {
        return string.Empty;
      }

      string value = reader.ReadContentAsString();
      return value;
    }
  }

}
