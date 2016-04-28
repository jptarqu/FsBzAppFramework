namespace FsBzAppFramework.UserDefined.Core

// All possible error types raised by validation 

module ErrorTypes =
    open FsBzAppFramework.Utilities.Contracts.ErrorTypes

    type CustomerError = 
        | BlankName of ErrorDesc
        | NotFound of ErrorDesc