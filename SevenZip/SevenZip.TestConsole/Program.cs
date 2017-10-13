namespace SevenZip.TestConsole
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SevenZip.Compression.LZMA;

    class Program
    {
        #region Methods

        static void Main(string[] args)
        {
            new Program().Run(RunOptions.EncodeString);
        }

        internal enum RunOptions
        {
            EncodeString = 1,
            DecodeString = 2
        }

        internal void Run(RunOptions option)
        {
            EncodeString();
        }

        private void EncodeString()
        {
            var testString = @"This is a test string. It will be used to test the LZMA algorithm";

            var uncompressedBytes = Encoding.UTF8.GetBytes(testString);

            byte[] compressedData = null;

            using (var input_stream = new MemoryStream(uncompressedBytes))
            {
                using (var output_stream = new MemoryStream())
                {
                    var encoder = new Compression.LZMA.Encoder();

                    encoder.WriteCoderProperties(output_stream);

                    output_stream.Write(BitConverter.GetBytes(input_stream.Length), 0, 8);

                    encoder.Code(input_stream, output_stream, input_stream.Length, -1, null);

                    compressedData = output_stream.ToArray();
                }
            }

            var savedData = Convert.ToBase64String(compressedData);

            Console.WriteLine(testString.Length);
            Console.WriteLine(savedData.Length);

            string uncompressedString = string.Empty;

            using (var input_stream = new MemoryStream(compressedData))
            {
                using (var output_stream = new MemoryStream())
                {
                    var decoder = new Compression.LZMA.Decoder();

                    byte[] properties = new byte[5];

                    input_stream.Read(properties, 0, 5);

                    byte[] stringLengthBytes = new byte[8];

                    input_stream.Read(stringLengthBytes, 0, 8);

                    long stringLength = BitConverter.ToInt64(stringLengthBytes, 0);

                    decoder.SetDecoderProperties(properties);
                    decoder.Code(input_stream, output_stream, input_stream.Length, stringLength, null);

                    string uncompressedData = Encoding.UTF8.GetString(output_stream.ToArray());

                    Console.WriteLine(uncompressedData.Length);
                }
            }
        }        

        #endregion Methods
    }
}