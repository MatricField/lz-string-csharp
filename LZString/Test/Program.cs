using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ = LZString.LZString;
using Compression.LZString.CSharp;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var raw = File.ReadAllText("LZString.cs");
            var compressed = LZ.compressToBase64(raw);
            LZStringPredefined.Base64Decoder.Decode(compressed, Console.OpenStandardOutput());
        }
    }
}
