// InBuffer.cs
namespace SevenZip
{
    public class InBuffer
    {
        #region Fields

        byte[] m_Buffer;
        uint m_BufferSize;
        uint m_Limit;
        uint m_Pos;
        ulong m_ProcessedSize;
        System.IO.Stream m_Stream;
        bool m_StreamWasExhausted;

        #endregion Fields

        #region Constructors

        public InBuffer(uint bufferSize)
        {
            m_Buffer = new byte[bufferSize];
            m_BufferSize = bufferSize;
        }

        #endregion Constructors

        #region Methods

        public ulong GetProcessedSize()
        {
            return m_ProcessedSize + m_Pos;
        }

        public void Init(System.IO.Stream stream)
        {
            m_Stream = stream;
            m_ProcessedSize = 0;
            m_Limit = 0;
            m_Pos = 0;
            m_StreamWasExhausted = false;
        }

        public bool ReadBlock()
        {
            if (m_StreamWasExhausted)
                return false;
            m_ProcessedSize += m_Pos;
            int aNumProcessedBytes = m_Stream.Read(m_Buffer, 0, (int)m_BufferSize);
            m_Pos = 0;
            m_Limit = (uint)aNumProcessedBytes;
            m_StreamWasExhausted = (aNumProcessedBytes == 0);
            return (!m_StreamWasExhausted);
        }

        // check it
        public bool ReadByte(byte b)
        {
            if (m_Pos >= m_Limit)
                if (!ReadBlock())
                    return false;
            b = m_Buffer[m_Pos++];
            return true;
        }

        public byte ReadByte()
        {
            // return (byte)m_Stream.ReadByte();
            if (m_Pos >= m_Limit)
                if (!ReadBlock())
                    return 0xFF;
            return m_Buffer[m_Pos++];
        }

        public void ReleaseStream()
        {
            // m_Stream.Close();
            m_Stream = null;
        }

        #endregion Methods
    }
}