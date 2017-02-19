/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations
 * under the License.
 *
 * Contains some contributions under the Thrift Software License.
 * Please see doc/old-thrift-license.txt in the Thrift distribution for
 * details.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Bench;

namespace Thrift.Transport
{
  public class TTransportStream : Stream
  {

    private TTransport transport;
    public TTransportStream(TTransport transport)
    {
      this.transport = transport;
    }

    public override long Position
    {
      get { throw new NotSupportedException(); }
      set { throw new NotSupportedException(); }
    }

    public override long Length
    {
      get { throw new NotSupportedException(); }
    }

    public override bool CanRead
    {
      get { return true; }
    }
    
    public override bool CanSeek
    {
      get { return false; }
    }

    public override bool CanWrite
    {
      get { return true; }
    }

    public override void Flush()
    {
      this.transport.Flush();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException();
    }
    public override void SetLength(long value)
    {
      throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      return this.transport.Read(buffer, offset, count);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      Console.WriteLine("write {0} bytes", count);
      this.transport.Write(buffer, offset, count);
    }
  }
}
