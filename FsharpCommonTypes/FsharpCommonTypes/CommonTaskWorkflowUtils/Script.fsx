// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.


type RequestError = 
    | ValidationError of string list
    | FatalError of string
    | NoDataFound of string

// Define your library scripting code here
let err = ValidationError (["lo";"behold"])
let txt = sprintf "Error %A" err
printfn "%s" txt
