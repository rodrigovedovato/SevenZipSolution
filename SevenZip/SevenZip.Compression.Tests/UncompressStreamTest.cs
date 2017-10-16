using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenZip.Compression.Tests
{
    [TestClass]
    public class UncompressStreamTest
    {
        [TestMethod]
        public void Check_If_String_Is_Successfully_Uncompressed()
        {
            byte[] compressedData = Convert.FromBase64String("XQAAQABBAAAAAAAAAAAqGgknZByHik/KTPT4HW0wFZfVyH7ZoLbflKRtlDLA1EhZVQvyHKoU9JGTk5VCeM0BJGxxonImkpI9ey/T");
            byte[] uncompressedData = null;

            using (var uncompressionStream = new LZMAUncompressionStream(compressedData))
            {
                uncompressedData = uncompressionStream.ToArray();
            }

            Assert.AreEqual("This is a test string. It will be used to test the LZMA algorithm", Encoding.UTF8.GetString(uncompressedData));
        }
    }
}
