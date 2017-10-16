using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SevenZip.Compression
{
    public class LZMACompressStream : LZMAStream
    {
        public LZMACompressStream(byte[] uncompressedData) 
            : base(uncompressedData)
        {
        }

        public LZMACompressStream(Stream uncompressedStream, bool leaveUncompressedStreamOpen = false) 
            : base(uncompressedStream, leaveUncompressedStreamOpen)
        {
        }

        protected override void Process()
        {
            var encoder = new LZMA.Encoder();            

            encoder.WriteCoderProperties(OutputStream);

            OutputStream.Write(BitConverter.GetBytes(InputStream.Length), 0, 8);

            encoder.Code(InputStream, OutputStream, InputStream.Length, -1, null);
        }
    }
}
