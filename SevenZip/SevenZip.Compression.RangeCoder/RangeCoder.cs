namespace SevenZip.Compression.RangeCoder
{
    using System;

    public class Decoder
    {
        #region Fields

        public const uint kTopValue = (1 << 24);

        public uint Code;
        public uint Range;

        // public Buffer.InBuffer Stream = new Buffer.InBuffer(1 << 16);
        public System.IO.Stream Stream;

        #endregion Fields

        #region Methods

        public void CloseStream()
        {
            Stream.Close();
        }

        public void Decode(uint start, uint size, uint total)
        {
            Code -= start * Range;
            Range *= size;
            Normalize();
        }

        public uint DecodeBit(uint size0, int numTotalBits)
        {
            uint newBound = (Range >> numTotalBits) * size0;
            uint symbol;
            if (Code < newBound)
            {
                symbol = 0;
                Range = newBound;
            }
            else
            {
                symbol = 1;
                Code -= newBound;
                Range -= newBound;
            }
            Normalize();
            return symbol;
        }

        public uint DecodeDirectBits(int numTotalBits)
        {
            uint range = Range;
            uint code = Code;
            uint result = 0;
            for (int i = numTotalBits; i > 0; i--)
            {
                range >>= 1;
                /*
                result <<= 1;
                if (code >= range)
                {
                    code -= range;
                    result |= 1;
                }
                */
                uint t = (code - range) >> 31;
                code -= range & (t - 1);
                result = (result << 1) | (1 - t);

                if (range < kTopValue)
                {
                    code = (code << 8) | (byte)Stream.ReadByte();
                    range <<= 8;
                }
            }
            Range = range;
            Code = code;
            return result;
        }

        public uint GetThreshold(uint total)
        {
            return Code / (Range /= total);
        }

        public void Init(System.IO.Stream stream)
        {
            // Stream.Init(stream);
            Stream = stream;

            Code = 0;
            Range = 0xFFFFFFFF;
            for (int i = 0; i < 5; i++)
                Code = (Code << 8) | (byte)Stream.ReadByte();
        }

        public void Normalize()
        {
            while (Range < kTopValue)
            {
                Code = (Code << 8) | (byte)Stream.ReadByte();
                Range <<= 8;
            }
        }

        public void Normalize2()
        {
            if (Range < kTopValue)
            {
                Code = (Code << 8) | (byte)Stream.ReadByte();
                Range <<= 8;
            }
        }

        public void ReleaseStream()
        {
            // Stream.ReleaseStream();
            Stream = null;
        }

        #endregion Methods

        #region Other

        // ulong GetProcessedSize() {return Stream.GetProcessedSize(); }

        #endregion Other
    }

    public class Encoder
    {
        #region Fields

        public const uint kTopValue = (1 << 24);

        public UInt64 Low;
        public uint Range;

        long StartPosition;
        System.IO.Stream Stream;
        byte _cache;
        uint _cacheSize;

        #endregion Fields

        #region Methods

        public void CloseStream()
        {
            Stream.Close();
        }

        public void Encode(uint start, uint size, uint total)
        {
            Low += start * (Range /= total);
            Range *= size;
            while (Range < kTopValue)
            {
                Range <<= 8;
                ShiftLow();
            }
        }

        public void EncodeBit(uint size0, int numTotalBits, uint symbol)
        {
            uint newBound = (Range >> numTotalBits) * size0;
            if (symbol == 0)
                Range = newBound;
            else
            {
                Low += newBound;
                Range -= newBound;
            }
            while (Range < kTopValue)
            {
                Range <<= 8;
                ShiftLow();
            }
        }

        public void EncodeDirectBits(uint v, int numTotalBits)
        {
            for (int i = numTotalBits - 1; i >= 0; i--)
            {
                Range >>= 1;
                if (((v >> i) & 1) == 1)
                    Low += Range;
                if (Range < kTopValue)
                {
                    Range <<= 8;
                    ShiftLow();
                }
            }
        }

        public void FlushData()
        {
            for (int i = 0; i < 5; i++)
                ShiftLow();
        }

        public void FlushStream()
        {
            Stream.Flush();
        }

        public long GetProcessedSizeAdd()
        {
            return _cacheSize +
                Stream.Position - StartPosition + 4;
            // (long)Stream.GetProcessedSize();
        }

        public void Init()
        {
            StartPosition = Stream.Position;

            Low = 0;
            Range = 0xFFFFFFFF;
            _cacheSize = 1;
            _cache = 0;
        }

        public void ReleaseStream()
        {
            Stream = null;
        }

        public void SetStream(System.IO.Stream stream)
        {
            Stream = stream;
        }

        public void ShiftLow()
        {
            if ((uint)Low < (uint)0xFF000000 || (uint)(Low >> 32) == 1)
            {
                byte temp = _cache;
                do
                {
                    Stream.WriteByte((byte)(temp + (Low >> 32)));
                    temp = 0xFF;
                }
                while (--_cacheSize != 0);
                _cache = (byte)(((uint)Low) >> 24);
            }
            _cacheSize++;
            Low = ((uint)Low) << 8;
        }

        #endregion Methods
    }
}