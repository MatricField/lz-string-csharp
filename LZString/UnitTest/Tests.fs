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
    let decompressed = LZNew.Base64Decoder.Decode(compressed)
    Assert.Equal((if null = raw then "" else raw), decompressed)
