namespace FsharpCommonTypes

module IntegerTypes =
    open System

    type IntegerPropertyStates =
        | LessThanMinimum of string*int
        | MoreThanMax of string*int
        | ValidValue 

    type IntegerPropertyDefinition (min:int, max:int) =
        member this.Min = min
        member this.Max = max
        
    let private validateMin (def:IntegerPropertyDefinition) (value) =
        if (value >= def.Min) then
            ValidValue
        else 
            LessThanMinimum ((sprintf "cannot be less than %d" def.Min), def.Min)

    let private validateMax (def:IntegerPropertyDefinition) (value) =
        if (value <= def.Max) then
            ValidValue
        else 
            MoreThanMax ((sprintf "cannot be more than %d" def.Max), def.Max)
            
    let GetValidationStates (def:IntegerPropertyDefinition) (value) =
        let states = [validateMin; validateMax] |> Seq.map (fun valFunc -> valFunc def value) |> Seq.distinct
        states 

    let private propValidationErrorCode = "FieldValidation"
    let private convertStateToPropError propName state  =
        match state with 
        | LessThanMinimum (s,l) -> Some { ErrorCode = propValidationErrorCode; Description= s; PropertyName = propName}
        | MoreThanMax (s,l) -> Some { ErrorCode = propValidationErrorCode; Description= s; PropertyName = propName}
        | ValidValue -> None


    type IntegerPropertyDefinition with    
        member this.GetPropertyValidationErrors propName value = 
            match ConversionHelpers.tryParseInt value with
            | Some convertedValue -> 
                let states = GetValidationStates this (int convertedValue)
                let propErrors = states |> Seq.map (convertStateToPropError propName) |> Seq.where Option.isSome |> Seq.map Option.get
                propErrors
            | None -> 
                seq [ { ErrorCode = propValidationErrorCode; Description= "is not a valid integer number"; PropertyName = propName} ]

        member this.GetValueFromText strValue = 
            (ConversionHelpers.tryParseInt strValue) |> Option.map int