﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Compression.LZString.CSharp
{
    public class BitReader
    {
        private IEnumerator<int> RawData;

        private int Buffer;

        private int BitsInBuffer;

        private int BitsInBufferMax;

        public BitReader(string input, Decoder decoder)
        {
            var rawData = new int[input.Length];
            BitsInBufferMax = decoder.BitsPerChar;
            var lookUpTable = BitReversalTable.Get(decoder.BitsPerChar);
            Parallel.For(0, input.Length, (i) => rawData[i] = lookUpTable[decoder.ReverseCodePage[input[i]]]);
            RawData = ((IEnumerable<int>)rawData).GetEnumerator();
        }

        public int ReadBits(uint _numBits)
       {
            var numBits = Convert.ToInt32(_numBits);
            int ret = 0;
            for(int bitsRead = 0; bitsRead != numBits;)
            {
                if(BitsInBuffer == 0 && !FetchBits())
                {
                    throw new EndOfStreamException();
                }
                var needToRead = numBits - bitsRead;
                if(BitsInBuffer <= needToRead)
                {
                    ret |= Buffer << bitsRead;
                    bitsRead += BitsInBuffer;
                    BitsInBuffer -= BitsInBuffer;
                }
                else
                {
                    var mask = 0;
                    for(int i = 0; i < needToRead; ++i)
                    {
                        mask |= 1 << i;
                    }
                    ret |= (Buffer & mask) << bitsRead;
                    BitsInBuffer -= needToRead;
                    bitsRead += needToRead;
                    Buffer >>= needToRead;
                }
            }
            return ret;
        }

        private bool FetchBits()
        {
            if(RawData.MoveNext())
            {
                Buffer = RawData.Current;
                BitsInBuffer = BitsInBufferMax;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
