#region GNU License

//The LZMAStream class is based on the 7-zip SDK created by Igor Pavlov (http://www.7-zip.org)

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.If not, see<http://www.gnu.org/licenses/>. 

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SevenZip.Compression
{
    /// <summary>
    /// A stream that uses the LZMA compression algorithm. Supports compression and decompression
    /// </summary>
    public abstract class LZMAStream : Stream       
    {
        #region Unsupported

        public override long Length => throw new NotSupportedException();

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        #endregion

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        protected Stream InputStream
        {
            get;
            private set;
        }

        protected MemoryStream OutputStream
        {
            get;
            private set;
        }

        private readonly bool _leaveUncompressedStreamOpen;        

        public LZMAStream(byte[] uncompressedData)
            :this(new MemoryStream(uncompressedData), false)
        {            
        }

        public LZMAStream(Stream uncompressedStream, bool leaveUncompressedStreamOpen)
        {
            InputStream = uncompressedStream;
            OutputStream = new MemoryStream();

            _leaveUncompressedStreamOpen = leaveUncompressedStreamOpen;

            Process();
        }

        public override void Flush()
        {            
            OutputStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return OutputStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }       
        
        public byte[] ToArray()
        {
            return OutputStream.ToArray();
        }

        protected abstract void Process();

        public override void Close()
        {
            OutputStream.Close();

            if (!_leaveUncompressedStreamOpen)
            {
                InputStream.Close();
            }
        }
    }
}
