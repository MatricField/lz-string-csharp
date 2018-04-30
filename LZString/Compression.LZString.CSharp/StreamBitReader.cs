using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compression.LZString.CSharp
{
    class StreamBitReader
    {
        private IEnumerable<char> UnderlyingStream;

        private int BitsPerChar;

        /// <remark>
        /// Use int as key for convenient use of StreamReader
        /// </remark>
        private IReadOnlyDictionary<char, int> ReverseCodePage;

        private int Buffer;

        private static IEnumerable<char> EnumerateStream(Stream stream)
        {
            var reader = new StreamReader(stream);
            var @char = reader.Read();
            while (@char >= 0)
            {
                yield return Convert.ToChar(@char);
                @char = reader.Read();
            }
        }

        public StreamBitReader(Stream underlyingStream, IReadOnlyDictionary<char, int> reverseCodePage, uint bitsPerChar):
            this(EnumerateStream(underlyingStream), reverseCodePage, bitsPerChar)
        {

        }

        public StreamBitReader(IEnumerable<char> underlyingStream, IReadOnlyDictionary<char, int> reverseCodePage, uint bitsPerChar)
        {
            UnderlyingStream = underlyingStream ?? throw new ArgumentNullException(nameof(underlyingStream));
            ReverseCodePage = reverseCodePage;
            BitsPerChar = Convert.ToInt32(bitsPerChar);
            SeekOrigin();
        }

        public IEnumerable<int> Bits
        {
            get
            {
                foreach(var ch in UnderlyingStream)
                {
                    var buffer = ReverseCodePage[ch];
                    for(int i = BitsPerChar - 1; i >= 0; --i)
                    {
                        yield return (buffer >> i) & 1;
                    }
                }
            }
        }

        private IEnumerator<int> Enumerator;

        public bool TryRead(out int ret, uint numBits = 1)
        {
            var tmp = 0;
            for(int i = 0; i < numBits; ++i)
            {
                if(!Enumerator.MoveNext())
                {
                    ret = default;
                    return false;
                }
                tmp |= Enumerator.Current << i;
            }
            ret = tmp;
            return true;
        }

        public void SeekOrigin()
        {
            Enumerator = Bits.GetEnumerator();
        }

        //private bool DoTryRead(out int ret)
        //{
        //    if (!EndOfStream)
        //    {
        //        ret = (Buffer >> BitPosition) & 0x1;
        //        Advance();
        //        return true;
        //    }
        //    else
        //    {
        //        ret = default;
        //        return false;
        //    }
        //}

        //public bool TryRead(out int ret, uint numBits = 1)
        //{
        //    var tmp = 0;
        //    for(int i = 0; i < numBits; ++ i)
        //    {
        //        if(DoTryRead(out var nextBit))
        //        {
        //            tmp |= nextBit << i;
        //        }
        //        else
        //        {
        //            ret = default;
        //            return false;
        //        }
        //    }
        //    ret = tmp;
        //    return true;
        //}

        //private void ReadOneChar()
        //{
        //    var charData = UnderlyingStream.Read();
        //    if (charData == -1)
        //    {
        //        EndOfStream = true;
        //    }
        //    else
        //    {
        //        Buffer = ReverseCodePage[Convert.ToChar(charData)];
        //        BitPosition = Convert.ToInt32(BitsPerChar - 1);
        //    }
        //}

        //private void Advance()
        //{
        //    if(!EndOfStream)
        //    {
        //        if(0 == BitPosition)
        //        {
        //            ReadOneChar();
        //        }
        //        else
        //        {
        //            --BitPosition;
 
        //        }
        //    }
        //}
    }
}
