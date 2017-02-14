
namespace Bench
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// XmlSerializerComponent is a helper class for Xml serialization
    /// </summary>
    /// <typeparam name="T">object type</typeparam>
    public class XmlSerializerComponent<T> : IXmlSerializable
    {
        /// <summary>
        /// Type attribute name
        /// </summary>
        public const string TypeAttributeName = "type";

        /// <summary>
        /// Assembly attribute name
        /// </summary>
        public const string AssemblyAttributeName = "assembly";

        /// <summary>
        /// Separator char
        /// </summary>
        public const char SeperatorChar = ',';

        /// <summary>
        /// Separator string
        /// </summary>
        public const string SeperatorString = ",";

        /// <summary>
        /// Generic type open tag in serialized string
        /// </summary>
        public const string GenericTypeOpen = "[";

        /// <summary>
        /// Generic type close tag in serialized string
        /// </summary>
        public const string GenericTypeClose = "]";

        /// <summary>
        /// Generic type label in type name
        /// </summary>
        public const string GenericTypeLabel = "`";

        /// <summary>
        /// serializer cache
        /// </summary>
        private static Dictionary<string, XmlSerializer> serializerCache = new Dictionary<string, XmlSerializer>();

        /// <summary>
        /// serilizer cache lock
        /// </summary>
        private static object serializerCacheLock = new object();

        /// <summary>
        /// instance field
        /// </summary>
        private T instance;

        /// <summary>
        /// Instance object
        /// </summary>
        [XmlIgnore]
        public T Instance
        {
            get { return this.instance; }
            set { this.instance = value; }
        }

        /// <summary>
        /// GetSchema method
        /// </summary>
        /// <returns>Not implemented</returns>
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserialize from Xml
        /// </summary>
        /// <param name="reader">XmlReader object</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            for (int index = 0; index < reader.AttributeCount; index++)
            {
                reader.MoveToAttribute(index);
                attributes.Add(reader.LocalName, reader.Value);
            }

            reader.MoveToElement();

            Type type = this.ReadTypeAttribute(reader);
            XmlSerializer ser = XmlSerializerComponent<T>.GetSerializer(reader.LocalName, type);
            this.instance = (T)ser.Deserialize(reader);
        }

        /// <summary>
        /// Serialize instance to Xml
        /// </summary>
        /// <param name="writer">XmlWriter object</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            XDocument xdoc = new XDocument();
            XmlSerializer ser = new XmlSerializer(this.Instance.GetType());
            using (XmlWriter xdocWriter = xdoc.CreateWriter())
            {
                ser.Serialize(xdocWriter, this.Instance);
                xdocWriter.Flush();
            }

            foreach (XAttribute attr in xdoc.Root.Attributes())
            {
                if (!attr.IsNamespaceDeclaration)
                {
                    writer.WriteAttributeString(attr.Name.LocalName, attr.Value);
                }
            }

            this.WriteTypeAttribute(writer, this.Instance.GetType());

            foreach (XNode node in xdoc.Root.Nodes())
            {
                node.WriteTo(writer);
            }
        }

        /// <summary>
        /// Get XmlSerializer for a type and tag name
        /// </summary>
        /// <param name="localName">tag name</param>
        /// <param name="type">type to be serialized</param>
        /// <returns>XmlSerializer object</returns>
        private static XmlSerializer GetSerializer(string localName, Type type)
        {
            string key = localName + ":" + type.FullName;

            lock (serializerCacheLock)
            {
                XmlSerializer ser;
                if (serializerCache.TryGetValue(key, out ser))
                {
                    return ser;
                }

                XmlAttributeOverrides overrides = new XmlAttributeOverrides();
                XmlAttributes attrsOverrides = new XmlAttributes();
                attrsOverrides.XmlType = new XmlTypeAttribute(localName);

                overrides.Add(type, attrsOverrides);
                ser = new XmlSerializer(type, overrides);
                serializerCache.Add(key, ser);
                return ser;
            }
        }

        /// <summary>
        /// Write type in a xml attribute
        /// </summary>
        /// <param name="writer">xmlwriter object</param>
        /// <param name="type">Type to be serialized</param>
        private void WriteTypeAttribute(XmlWriter writer, Type type)
        {
            HashSet<string> assemblies = new HashSet<string>();
			var typeInfo = type.GetTypeInfo();
			assemblies.Add(typeInfo.Assembly.GetName().Name);

            if (typeInfo.IsGenericType)
            {
                Type genericDef = typeInfo.GetGenericTypeDefinition();
                Type[] genericArguments = genericDef.GetGenericArguments();

				assemblies.Add(genericDef.GetTypeInfo().Assembly.GetName().Name);

                StringBuilder name = new StringBuilder();
                name.Append(genericDef.FullName.Substring(0, genericDef.FullName.IndexOf(XmlSerializerComponent<T>.GenericTypeLabel)));
                name.Append(XmlSerializerComponent<T>.GenericTypeOpen);

                for (int index = 0; index < genericArguments.Length; index++)
                {
                    Type argumentType = typeInfo.GenericTypeArguments[index];
                    assemblies.Add(argumentType.GetTypeInfo().Assembly.GetName().Name);

                    if (index > 0)
                    {
                        name.Append(XmlSerializerComponent<T>.SeperatorChar);
                    }

                    name.Append(argumentType.FullName);
                }

                name.Append(XmlSerializerComponent<T>.GenericTypeClose);
                writer.WriteAttributeString(XmlSerializerComponent<T>.TypeAttributeName, name.ToString());
            }
            else
            {
                writer.WriteAttributeString(XmlSerializerComponent<T>.TypeAttributeName, type.FullName);
            }

            writer.WriteAttributeString(XmlSerializerComponent<T>.AssemblyAttributeName, string.Join(XmlSerializerComponent<T>.SeperatorString, assemblies.ToArray()));
        }

        /// <summary>
        /// Read Type info from the type attribute in the xml
        /// </summary>
        /// <param name="reader">XmlReader object</param>
        /// <returns>Type to be deserialized</returns>
        private Type ReadTypeAttribute(XmlReader reader)
        {
            string typename = reader.GetAttribute(XmlSerializerComponent<T>.TypeAttributeName);
            if (typename == null)
            {
                throw new ArgumentException(XmlSerializerComponent<T>.TypeAttributeName);
            }

            string assembly = reader.GetAttribute(XmlSerializerComponent<T>.AssemblyAttributeName);
            if (!string.IsNullOrEmpty(assembly))
            {
                string[] assemblies = assembly.Split(XmlSerializerComponent<T>.SeperatorChar);
                foreach (string assemblyName in assemblies)
                {
					System.Reflection.Assembly.Load(new AssemblyName(assemblyName));
                }
            }

            int gIndex = typename.IndexOf(XmlSerializerComponent<T>.GenericTypeOpen);
            if (gIndex <= 0)
            {
                return TypeUtils.GetType(typename);
            }

            string genericname = typename.Substring(0, gIndex);
            string parameters = typename.Substring(gIndex + 1, typename.IndexOf(XmlSerializerComponent<T>.GenericTypeClose) - gIndex - 1);
            string[] parametertypes = parameters.Split(XmlSerializerComponent<T>.SeperatorChar);

            genericname = genericname + XmlSerializerComponent<T>.GenericTypeLabel + parametertypes.Length;

            Type genericType = TypeUtils.GetType(genericname);

            if (genericType == null)
            {
                throw new ArgumentException("Could not get type for type: " + genericname);
            }

            Type[] parameterTypes = parametertypes.Select(parametertype => TypeUtils.GetType(parametertype)).Where(type => type != null).ToArray();

            if (parameterTypes == null || parameterTypes.Length == 0)
            {
                throw new ArgumentException("Could not get type for parameters: " + parameters);
            }

            return genericType.MakeGenericType(parameterTypes);
        }
    }
}
