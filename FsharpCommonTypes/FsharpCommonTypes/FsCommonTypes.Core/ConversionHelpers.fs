﻿namespace FsharpCommonTypes

module ConversionHelpers =
    let tryParseWith tryParseFunc = tryParseFunc >> function
        | true, v    -> Some v
        | false, _   -> None

    let tryParseInt    = tryParseWith FSharp.Core.int.TryParse
    let tryParseDecimal = tryParseWith FSharp.Core.decimal.TryParse
    let tryParseDate = tryParseWith System.DateTime.TryParse
    
    let (|Int|_|)    = tryParseInt
    let (|Decimal|_|) = tryParseDecimal
    let (|Date|_|) = tryParseDate

    
    let tryParseIntWithDefault x defaultVal= defaultArg (tryParseInt x) defaultVal
    let tryParseDecimalWithDefault x defaultVal= defaultArg (tryParseDecimal x) defaultVal
    let tryParseDateWithDefault x defaultVal= defaultArg (tryParseDate x) defaultVal