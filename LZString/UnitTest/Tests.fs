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
    let decompressed = 
        let chars =
            LZNew.Base64Decoder.Decode(compressed)
            |>Array.ofSeq
        String(chars)
    Assert.Equal((if null <> raw then raw else ""), decompressed)
