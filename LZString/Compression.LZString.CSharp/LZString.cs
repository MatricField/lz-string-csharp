using System;
using System.Collections.Generic;
using System.Text;

namespace Compression.LZString.CSharp
{
    public static class LZString
    {
        public static Decoder GetDecoder(string alphabet, uint bitsPerChar) => new Decoder(Predefined.GetReverseCodePage(alphabet), bitsPerChar);

        public static Decoder Base64Decoder { get; } = GetDecoder(Predefined.KeyStrBase64, 6);
    }
}
