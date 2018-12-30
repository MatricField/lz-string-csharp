using System;
using System.Collections.Generic;
using System.Text;

namespace Compression.LZString.CSharp
{
    public static class Predefined
    {
        public static DataEncoding Base64Encoding { get; }
            = new DataEncoding("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=", 6);
    }
}
