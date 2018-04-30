using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Compression.LZString.CSharp
{
    public class LZStringDecoder
    {
        private IReadOnlyDictionary<char, int> ReverseCodePage;

        private uint BitsPerChar;

        public void Decode(IEnumerable<char> inputStream, Stream outPutStream)
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
                { LZStringMasks.Char8Bit, null},
                { LZStringMasks.Char16Bit, null},
                { LZStringMasks.EndOfStream, null}
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

            var outWriter = new StreamWriter(outPutStream);
            outWriter.AutoFlush = true;
            var w = "";
            bool ReadNextSegment(out string ret, out bool isCharEntry)
            {
                var codePoint = ReadBitsOrThrow(codePointWidth);
                switch (codePoint)
                {
                    case LZStringMasks.Char8Bit:
                        ret = Convert.ToChar(ReadBitsOrThrow(8)).ToString();
                        isCharEntry = true;
                        return true;
                    case LZStringMasks.Char16Bit:
                        ret = Convert.ToChar(ReadBitsOrThrow(16)).ToString();
                        isCharEntry = true;
                        return true;
                    case LZStringMasks.EndOfStream:
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
                outWriter.Write(w);
                AddToDictionary(w);
            }
            else
            {
                return;
            }
            while(ReadNextSegment(out var entry, out var isCharEntry))
            {
                if(isCharEntry)
                {
                    AddToDictionary(entry);
                }
                outWriter.Write(entry);
                AddToDictionary(w + entry[0]);
                w = entry;
            }
        }

        public LZStringDecoder(IReadOnlyDictionary<char, int> reverseCodePage, uint bitsPerChar)
        {
            ReverseCodePage = reverseCodePage;
            BitsPerChar = bitsPerChar;
        }
    }
}
