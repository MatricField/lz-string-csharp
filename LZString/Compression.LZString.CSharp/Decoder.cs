using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Compression.LZString.CSharp
{
    public class Decoder
    {
        private IReadOnlyDictionary<char, int> ReverseCodePage;

        private int BitsPerChar;

        public IEnumerable<char> Decode(IEnumerable<char> inputStream)
        {
            if((!inputStream?.Any()) ?? false)
            {
                yield break;
            }

            /* 
             * Note: 
             * Local function is new in C# 7, save us some trouble to hand write a state class.
             * Nested function is, however, trivial in JavaScript, same as generator.
             */

            //Read bits from encoded stream
            IEnumerator<int> GetBits()
            {
                foreach (var ch in inputStream)
                {
                    var buffer = ReverseCodePage[ch];
                    for (int i = BitsPerChar - 1; i >= 0; --i)
                    {
                        yield return (buffer >> i) & 1;
                    }
                }
            }

            var bits = GetBits();

            int ReadBitsOrThrow(uint numBits)
            {
                var tmp = 0;
                for (int i = 0; i < numBits; ++i)
                {
                    if (!bits.MoveNext())
                    {
                        throw new EndOfStreamException();
                    }
                    tmp |= bits.Current << i;
                }
                return tmp;
            }

            var reverseDictionary = new Dictionary<int, string>()
            {
                { Masks.Char8Bit, null},
                { Masks.Char16Bit, null},
                { Masks.EndOfStream, null}
            };
            var codePointWidth = 2u; // width of code point in bits
            var dictionaryCapacity = 4u; // possible number of code points under current width, value = 2 ^ codePointWidth

            void AddToDictionary(string value)
            {
                reverseDictionary.Add(reverseDictionary.Count, value);
                if(reverseDictionary.Count == dictionaryCapacity)
                {
                    codePointWidth++;
                    dictionaryCapacity *= 2;
                }
            }

            var w = "";
            bool ReadNextSegment(out string ret, out bool isCharEntry)
            {
                var codePoint = ReadBitsOrThrow(codePointWidth);
                switch (codePoint)
                {
                    case Masks.Char8Bit:
                        ret = Convert.ToChar(ReadBitsOrThrow(8)).ToString();
                        isCharEntry = true;
                        return true;
                    case Masks.Char16Bit:
                        ret = Convert.ToChar(ReadBitsOrThrow(16)).ToString();
                        isCharEntry = true;
                        return true;
                    case Masks.EndOfStream:
                        ret = default;
                        isCharEntry = default;
                        return false;
                    default:
                        isCharEntry = false;
                        if (reverseDictionary.TryGetValue(codePoint, out ret))
                        {
                            return null != ret ? true : throw new InvalidDataException();
                        }
                        else if (codePoint == reverseDictionary.Count)
                        {
                            ret = w + w[0];
                            return true;
                        }
                        else
                        {
                            throw new InvalidDataException();
                        }
                }
            }

            if(ReadNextSegment(out w, out var _))
            {
                foreach (var c in w) yield return c;
                AddToDictionary(w);
            }
            else
            {
                yield break;
            }
            while(ReadNextSegment(out var entry, out var isCharEntry))
            {
                if(isCharEntry)
                {
                    AddToDictionary(entry);
                }
                foreach (var c in entry) yield return c;
                AddToDictionary(w + entry[0]);
                w = entry;
            }
        }

        public Decoder(IReadOnlyDictionary<char, int> reverseCodePage, uint bitsPerChar)
        {
            ReverseCodePage = reverseCodePage;
            BitsPerChar = Convert.ToInt32(bitsPerChar);
        }
    }
}
