#region Header

// LzInWindow.cs

#endregion Header

namespace SevenZip.Compression.LZ
{
    using System;

    public class InWindow
    {
        #region Fields

        public UInt32 _blockSize; // Size of Allocated memory block
        public Byte[] _bufferBase = null; // pointer to buffer with data
        public UInt32 _bufferOffset;
        public UInt32 _pos; // offset (from _buffer) of curent byte
        public UInt32 _streamPos; // offset (from _buffer) of first not read byte from Stream

        UInt32 _keepSizeAfter; // how many BYTEs must be kept buffer after _pos
        UInt32 _keepSizeBefore; // how many BYTEs must be kept in buffer before _pos
        UInt32 _pointerToLastSafePosition;
        UInt32 _posLimit; // offset (from _buffer) of first byte when new block reading must be done
        System.IO.Stream _stream;
        bool _streamEndWasReached; // if (true) then _streamPos shows real end of stream

        #endregion Fields

        #region Methods

        public void Create(UInt32 keepSizeBefore, UInt32 keepSizeAfter, UInt32 keepSizeReserv)
        {
            _keepSizeBefore = keepSizeBefore;
            _keepSizeAfter = keepSizeAfter;
            UInt32 blockSize = keepSizeBefore + keepSizeAfter + keepSizeReserv;
            if (_bufferBase == null || _blockSize != blockSize)
            {
                Free();
                _blockSize = blockSize;
                _bufferBase = new Byte[_blockSize];
            }
            _pointerToLastSafePosition = _blockSize - keepSizeAfter;
        }

        public Byte GetIndexByte(Int32 index)
        {
            return _bufferBase[_bufferOffset + _pos + index];
        }

        // index + limit have not to exceed _keepSizeAfter;
        public UInt32 GetMatchLen(Int32 index, UInt32 distance, UInt32 limit)
        {
            if (_streamEndWasReached)
                if ((_pos + index) + limit > _streamPos)
                    limit = _streamPos - (UInt32)(_pos + index);
            distance++;
            // Byte *pby = _buffer + (size_t)_pos + index;
            UInt32 pby = _bufferOffset + _pos + (UInt32)index;

            UInt32 i;
            for (i = 0; i < limit && _bufferBase[pby + i] == _bufferBase[pby + i - distance]; i++);
            return i;
        }

        public UInt32 GetNumAvailableBytes()
        {
            return _streamPos - _pos;
        }

        public void Init()
        {
            _bufferOffset = 0;
            _pos = 0;
            _streamPos = 0;
            _streamEndWasReached = false;
            ReadBlock();
        }

        public void MoveBlock()
        {
            UInt32 offset = (UInt32)(_bufferOffset) + _pos - _keepSizeBefore;
            // we need one additional byte, since MovePos moves on 1 byte.
            if (offset > 0)
                offset--;

            UInt32 numBytes = (UInt32)(_bufferOffset) + _streamPos - offset;

            // check negative offset ????
            for (UInt32 i = 0; i < numBytes; i++)
                _bufferBase[i] = _bufferBase[offset + i];
            _bufferOffset -= offset;
        }

        public void MovePos()
        {
            _pos++;
            if (_pos > _posLimit)
            {
                UInt32 pointerToPostion = _bufferOffset + _pos;
                if (pointerToPostion > _pointerToLastSafePosition)
                    MoveBlock();
                ReadBlock();
            }
        }

        public virtual void ReadBlock()
        {
            if (_streamEndWasReached)
                return;
            while (true)
            {
                int size = (int)((0 - _bufferOffset) + _blockSize - _streamPos);
                if (size == 0)
                    return;
                int numReadBytes = _stream.Read(_bufferBase, (int)(_bufferOffset + _streamPos), size);
                if (numReadBytes == 0)
                {
                    _posLimit = _streamPos;
                    UInt32 pointerToPostion = _bufferOffset + _posLimit;
                    if (pointerToPostion > _pointerToLastSafePosition)
                        _posLimit = (UInt32)(_pointerToLastSafePosition - _bufferOffset);

                    _streamEndWasReached = true;
                    return;
                }
                _streamPos += (UInt32)numReadBytes;
                if (_streamPos >= _pos + _keepSizeAfter)
                    _posLimit = _streamPos - _keepSizeAfter;
            }
        }

        public void ReduceOffsets(Int32 subValue)
        {
            _bufferOffset += (UInt32)subValue;
            _posLimit -= (UInt32)subValue;
            _pos -= (UInt32)subValue;
            _streamPos -= (UInt32)subValue;
        }

        public void ReleaseStream()
        {
            _stream = null;
        }

        public void SetStream(System.IO.Stream stream)
        {
            _stream = stream;
        }

        void Free()
        {
            _bufferBase = null;
        }

        #endregion Methods
    }
}