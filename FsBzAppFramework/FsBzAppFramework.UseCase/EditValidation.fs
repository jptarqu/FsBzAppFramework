﻿namespace FsBzAppFramework.UseCase

//Funcs to validate view as a whole
module EditValidation =
    open Chessie.ErrorHandling
    // maybe AppConstructorGUI auto concats funs defined here into one railway to exec in Validate??
    let Validate cmd =
        // return Ok (use railway)
        Ok cmd

