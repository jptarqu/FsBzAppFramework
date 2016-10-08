namespace FsharpCommonTypes

module DecimalTypes =
    open System

    type DecimalPropertyStates =
        | LessThanMinimum of string*decimal
        | MoreThanMax of string*decimal
        | ValidValue 

    type DecimalPropertyDefinition (min:decimal, max:decimal, decimalPlaces:int) =
        member this.Min = min
        member this.Max = max
        member this.DecimalPlaces = decimalPlaces
        
    let private validateMin (def:DecimalPropertyDefinition) (value) =
        if (value >= def.Min) then
            ValidValue
        else 
            LessThanMinimum ((sprintf "cannot be less than %M" def.Min), def.Min)

    let private validateMax (def:DecimalPropertyDefinition) (value) =
        if (value <= def.Max) then
            ValidValue
        else 
            MoreThanMax ((sprintf "cannot be more than %M" def.Max), def.Max)
            
    let GetValidationStates (def:DecimalPropertyDefinition) (value) =
        let states = [validateMin; validateMax] |> Seq.map (fun valFunc -> valFunc def value) |> Seq.distinct
        states 

    let private propValidationErrorCode = "FieldValidation"
    let private convertStateToPropError propName state  =
        match state with 
        | LessThanMinimum (s,l) -> Some { ErrorCode = propValidationErrorCode; Description= s; PropertyName = propName}
        | MoreThanMax (s,l) -> Some { ErrorCode = propValidationErrorCode; Description= s; PropertyName = propName}
        | ValidValue -> None

    type DecimalPropertyDefinition with    
        member this.GetPropertyValidationErrors propName value = 
            match ConversionHelpers.tryParseDecimal value with
            | Some convertedValue -> 
                let states = GetValidationStates this convertedValue
                let propErrors = states |> Seq.map (convertStateToPropError propName) |> Seq.where Option.isSome |> Seq.map Option.get
                propErrors
            | None -> 
                seq [ { ErrorCode = propValidationErrorCode; Description= "is not a valid decimal number"; PropertyName = propName} ]

        member this.GetValueFromText strValue = 
            ConversionHelpers.tryParseDecimal strValue