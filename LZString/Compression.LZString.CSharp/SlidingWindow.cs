using System;
using System.Collections.Generic;
using System.Text;

namespace Compression.LZString.CSharp
{
    public class SlidingWindow<T>
    {
        private ReadOnlyMemory<T> Data;

        private int Begin;

        private int Length;

        public ReadOnlyMemory<T> Window
        {
            get
            {
                return Data.Slice(Begin, Length);
            }
        }

        public bool CanAdvance => Begin + Length < Data.Length;

        public SlidingWindow(ReadOnlyMemory<T> data)
        {
            if(data.Length < 1)
            {
                throw new ArgumentException();
            }
            Data = data;
            Begin = 0;
            Length = 1;
        }

        public void Slide()
        {
            Begin = Begin + Length;
        }

        public void Enlarge()
        {
            ++Length;
        }

        public void ResetSize()
        {
            Length = 1;
        }

        public void SlideAndResetSize()
        {
            Slide();
            ResetSize();
        }
    }
}
