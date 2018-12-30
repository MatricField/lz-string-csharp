﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Compression.LZString.CSharp
{
    public sealed class DataEncoding
    {
        public int BitsPerChar { get; }

        public string CodePage { get; }

        public IReadOnlyDictionary<char, int> ReverseCodePage { get; }

        public DataEncoding(string alphabet, int bitsPerChar):
            this(bitsPerChar, alphabet, GetReverseCodePage(alphabet))
        {

        }

        public DataEncoding(int bitsPerChar, string codepage, IReadOnlyDictionary<char, int> reverseCodePage)
        {
            BitsPerChar = bitsPerChar;
            CodePage = codepage;
            ReverseCodePage = reverseCodePage;
        }

        private static Dictionary<string, IReadOnlyDictionary<char, int>> ReverseCodePages { get; }
            = new Dictionary<string, IReadOnlyDictionary<char, int>>();

        public static IReadOnlyDictionary<char, int> GetReverseCodePage(string alphabet)
        {
            if (ReverseCodePages.TryGetValue(alphabet, out var ret))
            {
                return ret;
            }
            else
            {
                var tmp = new Dictionary<char, int>();
                for (int i = 0; i < alphabet.Length; ++i)
                {
                    tmp[alphabet[i]] = Convert.ToUInt16(i);
                }
                ReverseCodePages[alphabet] = tmp;
                return tmp;
            }
        }
    }
}
