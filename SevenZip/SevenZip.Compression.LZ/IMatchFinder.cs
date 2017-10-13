#region Header

// IMatchFinder.cs

#endregion Header

namespace SevenZip.Compression.LZ
{
    using System;

    public interface IInWindowStream
    {
        #region Methods

        Byte GetIndexByte(Int32 index);

        UInt32 GetMatchLen(Int32 index, UInt32 distance, UInt32 limit);

        UInt32 GetNumAvailableBytes();

        void Init();

        void ReleaseStream();

        void SetStream(System.IO.Stream inStream);

        #endregion Methods
    }

    public interface IMatchFinder : IInWindowStream
    {
        #region Methods

        void Create(UInt32 historySize, UInt32 keepAddBufferBefore,
            UInt32 matchMaxLen, UInt32 keepAddBufferAfter);

        UInt32 GetMatches(UInt32[] distances);

        void Skip(UInt32 num);

        #endregion Methods
    }
}