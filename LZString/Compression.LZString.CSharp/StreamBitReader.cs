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

        private IReadOnlyDictionary<char, int> ReverseCodePage;

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
    }
}
