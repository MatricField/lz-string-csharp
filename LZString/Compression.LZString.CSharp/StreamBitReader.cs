using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compression.LZString.CSharp
{
    class StreamBitReader
    {
        private Stream UnderlyingStream;

        private byte Buffer;

        private int Position;

        private const byte HIGHEST_BIT_MASK = 0b10000000;

        public StreamBitReader(Stream underlyingStream)
        {
            UnderlyingStream = underlyingStream ?? throw new ArgumentNullException(nameof(underlyingStream));
        }

        public bool TryReadBool(out bool ret)
        {
            if (Position != 8)
            {
                ret = (Buffer & HIGHEST_BIT_MASK) == HIGHEST_BIT_MASK;
                return true;
            }
            else
            {
                ret = default;
                return false;
            }
        }

        public bool TryRead(out byte ret)
        {
            if (Position != 8)
            {
                ret = Convert.ToByte((Buffer & HIGHEST_BIT_MASK) >> 7);
                return true;
            }
            else
            {
                ret = default;
                return false;
            }
        }

        private void Advance()
        {
            switch(Position)
            {
                case 8:
                    return;
                case 7:
                    var newBuffer = UnderlyingStream.ReadByte();
                    if (newBuffer == -1)
                    {
                        Position = 8;
                    }
                    else
                    {
                        Buffer = (byte)newBuffer;
                        Position = 0;
                    }
                    break;
                default:
                    ++Position;
                    Buffer <<= 1;
                    break;
            }
        }
    }
}
