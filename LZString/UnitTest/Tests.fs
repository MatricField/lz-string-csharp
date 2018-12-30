module Tests

open System
open Xunit
open FsCheck.Xunit
open FsCheck

type LZOld = LZString.LZString
type LZNew = Compression.LZString.CSharp.LZString

[<Property()>]
let ``can decompress from base64`` (raw: string)=
    let compressed = LZOld.compressToBase64(raw)
    let decompressed = LZNew.DecompressFromBase64(compressed)
    Assert.Equal(raw, decompressed)
