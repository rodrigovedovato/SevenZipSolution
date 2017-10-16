using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SevenZip.Compression
{
    public class LZMAUncompressionStream : LZMAStream
    {
        public LZMAUncompressionStream(byte[] compressedData) : base(compressedData)
        {
        }

        public LZMAUncompressionStream(Stream compressedStream, bool leaveUncompressedStreamOpen) : base(compressedStream, leaveUncompressedStreamOpen)
        {
        }

        protected override void Process()
        {
            var decoder = new LZMA.Decoder();

            byte[] properties = new byte[5];

            InputStream.Read(properties, 0, 5);

            byte[] stringLengthBytes = new byte[8];

            InputStream.Read(stringLengthBytes, 0, 8);

            long stringLength = BitConverter.ToInt64(stringLengthBytes, 0);

            decoder.SetDecoderProperties(properties);
            decoder.Code(InputStream, OutputStream, InputStream.Length, stringLength, null);            
        }
    }
}
