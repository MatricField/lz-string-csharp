using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Compression.LZString.CSharp
{
    public sealed class Decoder
    {
        public IReadOnlyDictionary<char, int> ReverseCodePage { get; }

        public int BitsPerChar { get; }

        public string Decode(string inputStream)
        {
            if(string.IsNullOrEmpty(inputStream))
            {
                return inputStream;
            }

            var bitReader = new BitReader(inputStream, this);

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
            var result = new StringBuilder();
            bool ReadNextSegment(out string ret, out bool isCharEntry)
            {
                var codePoint = bitReader.ReadBits(codePointWidth);
                switch (codePoint)
                {
                    case Masks.Char8Bit:
                        ret = Convert.ToChar(bitReader.ReadBits(8)).ToString();
                        isCharEntry = true;
                        return true;
                    case Masks.Char16Bit:
                        ret = Convert.ToChar(bitReader.ReadBits(16)).ToString();
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
                result.Append(w);
                AddToDictionary(w);
            }
            else
            {
                return "";
            }

            while(ReadNextSegment(out var entry, out var isCharEntry))
            {
                if(isCharEntry)
                {
                    AddToDictionary(entry);
                }
                result.Append(entry);
                AddToDictionary(w + entry[0]);
                w = entry;
            }
            return result.ToString();
        }

        public Decoder(IReadOnlyDictionary<char, int> reverseCodePage, uint bitsPerChar)
        {
            ReverseCodePage = reverseCodePage;
            BitsPerChar = Convert.ToInt32(bitsPerChar);
        }
    }
}
