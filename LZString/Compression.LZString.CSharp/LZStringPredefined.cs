using System;
using System.Collections.Generic;
using System.Text;

namespace Compression.LZString.CSharp
{
    public static class LZStringPredefined
    {
        public const string KeyStrBase64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

        private static Dictionary<string, IReadOnlyDictionary<char, int>> ReverseCodePages { get; }
            = new Dictionary<string, IReadOnlyDictionary<char, int>>();

        public static IReadOnlyDictionary<char, int> GetReverseCodePage(string alphabet)
        {
            if(ReverseCodePages.TryGetValue(alphabet, out var ret))
            {
                return ret;
            }
            else
            {
                var tmp = new Dictionary<char, int>();
                for(int i = 0; i < alphabet.Length; ++i)
                {
                    tmp[alphabet[i]] = Convert.ToUInt16(i);
                }
                ReverseCodePages[alphabet] = tmp;
                return tmp;
            }
        }

        public static LZStringDecoder GetDecoder(string alphabet, uint bitsPerChar) => new LZStringDecoder(GetReverseCodePage(alphabet), bitsPerChar);

        public static LZStringDecoder Base64Decoder { get; } = GetDecoder(KeyStrBase64, 6);
    }
}
