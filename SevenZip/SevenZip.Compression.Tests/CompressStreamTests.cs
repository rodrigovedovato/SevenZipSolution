using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SevenZip.Compression.Tests
{
    [TestClass]
    public class CompressStreamTests
    {
        [TestMethod]
        public void Check_If_String_Is_Successfully_Compressed()
        {
            byte[] uncompressedBytes = System.Text.Encoding.UTF8.GetBytes("This is a test string. It will be used to test the LZMA algorithm");

            byte[] compressedData = null;

            using (var compressionStream = new LZMACompressStream(uncompressedBytes))
            {
                compressedData = compressionStream.ToArray();
            }

            Assert.AreEqual("XQAAQABBAAAAAAAAAAAqGgknZByHik/KTPT4HW0wFZfVyH7ZoLbflKRtlDLA1EhZVQvyHKoU9JGTk5VCeM0BJGxxonImkpI9ey/T", System.Convert.ToBase64String(compressedData));
        }
    }
}
