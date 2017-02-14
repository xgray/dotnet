﻿/*
 * Copyright 2013 Real Logic Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using Adaptive.SimpleBinaryEncoding.Util;

namespace Adaptive.SimpleBinaryEncoding.ir
{
    /// <summary>
    /// Class to encapsulate a token of information for the message schema stream. This Intermediate Representation (IR)
    /// is intended to be language, schema, platform independent.
    /// <p/>
    /// Processing and optimization could be run over a list of Tokens to perform various functions
    /// <ul>
    /// <li>re-ordering of fields based on size</li>
    /// <li>padding of fields in order to provide expansion room</li>
    /// <li>computing offsets of individual fields</li>
    /// <li>etc.</li>
    /// </ul>
    /// <p/>
    /// IR could be used to generate code or other specifications. It should be possible to do the
    /// following:
    /// <ul>
    /// <li>generate a FIX/SBE schema from IR</li>
    /// <li>generate an ASN.1 spec from IR</li>
    /// <li>generate a GPB spec from IR</li>
    /// <li>etc.</li>
    /// </ul>
    /// <p/>
    /// IR could be serialized to storage or network via code generated by SBE. Then read back in to
    /// a List of <seealso cref="Token"/>s.
    /// <p/>
    /// The entire IR of an entity is a <see cref="List{Token}"/>. The order of this list is
    /// very important. Encoding of fields is done by nodes pointing to specific encoding <seealso cref="PrimitiveType"/>
    /// objects. Each encoding node contains size, offset, byte order, and <seealso cref="Encoding"/>. Entities relevant
    /// to the encoding such as fields, messages, repeating groups, etc. are encapsulated in the list as nodes
    /// themselves. Although, they will in most cases never be serialized. The boundaries of these entities
    /// are delimited by BEGIN and END <seealso cref="Adaptive.SimpleBinaryEncoding.ir.Signal"/> values in the node <seealso cref="Encoding"/>.
    /// A list structure like this allows for each concatenation of encodings as well as easy traversal.
    /// <p/>
    /// An example encoding of a message headerStructure might be like this.
    /// <ul>
    /// <li>Token 0 - Signal = BEGIN_MESSAGE, schemaId = 100</li>
    /// <li>Token 1 - Signal = BEGIN_FIELD, schemaId = 25</li>
    /// <li>Token 2 - Signal = ENCODING, PrimitiveType = uint32, size = 4, offset = 0</li>
    /// <li>Token 3 - Signal = END_FIELD</li>
    /// <li>Token 4 - Signal = END_MESSAGE</li>
    /// </ul>
    /// <p/>
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Invalid ID value. </summary>
        public const int InvalidId = -1;

        /// <summary>
        /// Size not determined </summary>
        public const int VariableSize = -1;

        /// <summary>
        /// Offset not computed or set </summary>
        public const int UnknownOffset = -1;

        private readonly Signal _signal;
        private readonly string _name;
        private readonly int _schemaId;
        private readonly int _version;
        private readonly int _size;
        private readonly int _offset;
        private readonly Encoding _encoding;

        /// <summary>
        /// Construct an <seealso cref="Token"/> by providing values for all fields.
        /// </summary>
        /// <param name="signal"> for the token role </param>
        /// <param name="name"> of the token in the message </param>
        /// <param name="schemaId"> as the identifier in the message declaration </param>
        /// <param name="version"> application within the template </param>
        /// <param name="size"> of the component part </param>
        /// <param name="offset"> in the underlying message as octets </param>
        /// <param name="encoding"> of the primitive field </param>
        public Token(Signal signal, string name, int schemaId, int version, int size, int offset, Encoding encoding)
        {
            Verify.NotNull(signal, "signal");
            Verify.NotNull(name, "name");
            Verify.NotNull(encoding, "encoding");

            _signal = signal;
            _name = name;
            _schemaId = schemaId;
            _version = version;
            _size = size;
            _offset = offset;
            _encoding = encoding;
        }

        /// <summary>
        /// Signal the role of this token.
        /// </summary>
        /// <value>the &lt;seealso cref=&quot;Signal&quot;/&gt; for the token.</value>
        public Signal Signal
        {
            get { return _signal; }
        }

        /// <summary>
        /// Return the name of the token
        /// </summary>
        /// <value>name of the token</value>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Return the ID of the token assigned by the specification
        /// </summary>
        /// <value>ID of the token assigned by the specification</value>
        public int SchemaId
        {
            get { return _schemaId; }
        }

        /// <summary>
        /// The version context for this token.
        /// </summary>
        /// <value>version context for this token.</value>
        public int Version
        {
            get { return _version; }
        }

        /// <summary>
        /// The context in which the version field should be interpreted.
        /// </summary>
        /// <value>context in which the version field should be interpreted.</value>
        public VersionContext VersionContext
        {
            get
            {
                if (_signal == Signal.BeginMessage || _signal == Signal.EndMessage)
                {
                    return VersionContext.TemplateVersion;
                }
                return VersionContext.SinceTemplateVersion;
            }
        }

        /// <summary>
        /// The size of this token in bytes.
        /// </summary>
        /// <value>
        ///   the size of this node. A value of 0 means the node has no size when encoded. A value of &lt;seealso cref=&quot;
        ///   Token#VARIABLE_SIZE&quot;/&gt; means this node represents a variable length field.
        /// </value>
        public int Size
        {
            get { return _size; }
        }

        /// <summary>
        /// The number of encoded primitives in this type.
        /// </summary>
        /// <value>number of encoded primitives in this type.</value>
        public int ArrayLength
        {
            get
            {
                if (null == _encoding.PrimitiveType || 0 == _size)
                {
                    return 0;
                }

                return _size / _encoding.PrimitiveType.Size;
            }
        }

        /// <summary>
        /// The offset for this token in the message.
        /// </summary>
        /// <value>
        ///   the offset of this Token. A value of 0 means the node has no relevant offset. A value of &lt;seealso cref=&quot;
        ///   Token#UNKNOWN_OFFSET&quot;/&gt; means this nodes true offset is dependent on variable length fields ahead of it in
        ///   the encoding.
        /// </value>
        public int Offset
        {
            get { return _offset; }
        }

        /// <summary>
        /// Return the <seealso cref="Encoding"/> of the <seealso cref="Token"/>.
        /// </summary>
        /// <value>encoding of the &lt;seealso cref=&quot;Token&quot;/&gt;</value>
        public Encoding Encoding
        {
            get { return _encoding; }
        }

        public override string ToString()
        {
            return string.Format("Token{{" + "signal={0}, name='{1}\'" + ", schemaId={2}, version={3}, size={4}, offset={5}, encoding={6}{7}", Signal, Name, SchemaId, _version, _size, _offset, _encoding, '}');
        }

        public class Builder
        {
            private Signal _signal;
            private string _name;
            private int _schemaId = InvalidId;
            private int _version = 0;
            private int _size = 0;
            private int _offset = 0;
            private Encoding _encoding = new Encoding();

            public Builder Signal(Signal signal)
            {
                _signal = signal;
                return this;
            }

            public Builder Name(string name)
            {
                _name = name;
                return this;
            }

            public Builder SchemaId(int schemaId)
            {
                this._schemaId = schemaId;
                return this;
            }

            public Builder Version(int version)
            {
                _version = version;
                return this;
            }

            public Builder Size(int size)
            {
                _size = size;
                return this;
            }

            public Builder Offset(int offset)
            {
                _offset = offset;
                return this;
            }

            public Builder Encoding(Encoding encoding)
            {
                _encoding = encoding;
                return this;
            }

            public Token Build()
            {
                return new Token(_signal, _name, _schemaId, _version, _size, _offset, _encoding);
            }
        }
    }
}