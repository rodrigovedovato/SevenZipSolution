// OutBuffer.cs
namespace SevenZip
{
    public class OutBuffer
    {
        #region Fields

        byte[] m_Buffer;
        uint m_BufferSize;
        uint m_Pos;
        ulong m_ProcessedSize;
        System.IO.Stream m_Stream;

        #endregion Fields

        #region Constructors

        public OutBuffer(uint bufferSize)
        {
            m_Buffer = new byte[bufferSize];
            m_BufferSize = bufferSize;
        }

        #endregion Constructors

        #region Methods

        public void CloseStream()
        {
            m_Stream.Close();
        }

        public void FlushData()
        {
            if (m_Pos == 0)
                return;
            m_Stream.Write(m_Buffer, 0, (int)m_Pos);
            m_Pos = 0;
        }

        public void FlushStream()
        {
            m_Stream.Flush();
        }

        public ulong GetProcessedSize()
        {
            return m_ProcessedSize + m_Pos;
        }

        public void Init()
        {
            m_ProcessedSize = 0;
            m_Pos = 0;
        }

        public void ReleaseStream()
        {
            m_Stream = null;
        }

        public void SetStream(System.IO.Stream stream)
        {
            m_Stream = stream;
        }

        public void WriteByte(byte b)
        {
            m_Buffer[m_Pos++] = b;
            if (m_Pos >= m_BufferSize)
                FlushData();
        }

        #endregion Methods
    }
}