using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compression.LZString.CSharp
{
    public class Decoder
    {
        private IReadOnlyDictionary<char, int> ReverseCodePage;

        private uint BitsPerChar;

        public IEnumerable<char> Decode(IEnumerable<char> inputStream)
        {
            var bitReader = new StreamBitReader(inputStream, ReverseCodePage, BitsPerChar);
            int ReadBitsOrThrow(uint numBits = 1)
            {
                if (bitReader.TryRead(out var ret, numBits))
                {
                    return ret;
                }
                else
                {
                    throw new EndOfStreamException();
                }
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
            // Local function is new in C# 7, save us some trouble to hand write a state class.
            // Nested function is, however, trivial in JavaScript.
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
                            return true;
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
            BitsPerChar = bitsPerChar;
        }
    }
}
