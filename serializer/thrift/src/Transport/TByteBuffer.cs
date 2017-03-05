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
 */

using System;
using System.IO;

namespace Thrift.Transport
{
  public class TByteBuffer : TTransport
  {

    private byte[] byteStream;

    private int position = 0;
    private int length = 0;

    public TByteBuffer(int capacity)
    {
      byteStream = new byte[capacity];
    }

    public TByteBuffer(byte[] buf)
    {
      byteStream = buf;
    }

    public int Length
    {
      get { return this.length; }
    }

    public int Position
    {
      get { return this.position; }
    }

    public int Capacity
    {
      get { return byteStream.Length; }
      set { EnsureCapacity(value); }
    }
    public override bool IsOpen
    {
      get { return true; }
    }

    public void SetLength(int length)
    {
      EnsureCapacity(length);
      this.length = length;
    }

    private void EnsureCapacity(int capacity)
    {
      if (this.byteStream.Length < capacity)
      {
        this.byteStream = new byte[capacity];
      }
    }

    public byte[] GetBuffer()
    {
      return byteStream;
    }

    public override void Open()
    {
      /** do nothing **/
    }

    public override void Close()
    {
      /** do nothing **/
    }

    public void Clear()
    {
      this.position = 0;
      this.length = 0;
    }

    public long Seek(long offset, SeekOrigin loc)
    {
      if (loc == SeekOrigin.Begin)
      {
        this.position = (int)offset;
      }
      else if (loc == SeekOrigin.Current)
      {
        this.position += (int)offset;
      }
      else if (loc == SeekOrigin.End)
      {
        this.position = (int)(byteStream.Length - offset);
      }

      EnsureCapacity(position);
      return this.position;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      int read = Math.Min(this.length - this.position, count);
      Array.Copy(this.byteStream, this.position, buffer, offset, read);
      this.position += read;
      return read;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      EnsureCapacity(position + count);
      Array.Copy(buffer, offset, this.byteStream, this.position, count);
      this.position += count;
      this.length = Math.Max(this.position, this.length);
    }

    protected override void Dispose(bool disposing)
    {
    }

  }
}