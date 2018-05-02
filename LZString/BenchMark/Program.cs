using System;
using LZOld = LZString.LZString;
using LZNew = Compression.LZString.CSharp.LZString;
using Compression.LZString.CSharp;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Running;
using System.IO;
using System.Linq;

namespace BenchMark
{
    public class MyBenchMark
    {
        private string Compressed = LZOld.compressToBase64(File.ReadAllText("LZString.cs.txt"));

        [Benchmark]
        public string OldLZString() => LZOld.decompressFromBase64(Compressed);

        [Benchmark]
        public string Reimplement() => new string(LZNew.Base64Decoder.Decode(Compressed).ToArray());
    }

    class Program
    {
        static void Main(string[] args)
        {
            string Compressed = LZOld.compressToBase64(File.ReadAllText("LZString.cs.txt"));
            var decompressed = new string(LZNew.Base64Decoder.Decode(Compressed).ToArray());
            BenchmarkRunner.Run<MyBenchMark>();
        }
    }
}
