namespace FsharpCommonTypes

module TextType =
    open System
    open CommonValidations
    
    type TextPropertyStates =
        | RequiredButBlank of string
        | LessThanMinimumLen of string*int
        | MoreThanMaxLen of string*int
        | NotMatchingPattern of string*string
        | ValidValue 

    type TextPropertyDefinition (minLen, maxLen, isRequired, regexPattern) =
        member this.MinLen = minLen
        member this.MaxLen = maxLen
        member this.IsRequired = isRequired
        member this.RegexPattern = regexPattern
        member this.UsesRegex = String.IsNullOrEmpty(regexPattern)


    let TextProp regexPattern isRequired minLen maxLen =
        TextPropertyDefinition(minLen, maxLen,isRequired, regexPattern ) 

    let RequiredTextProp  minLen maxLen =
        TextProp null true  minLen maxLen

    let NonRequiredTextProp  minLen maxLen =
        TextProp null false  minLen maxLen
        
    let RequiredPatternTextProp  minLen maxLen regexPattern =
        TextProp regexPattern true  minLen maxLen

    let NonRequiredPatternTextProp  minLen maxLen regexPattern =
        TextProp regexPattern false  minLen maxLen
    
    let inline isBlank txt =
        System.String.IsNullOrWhiteSpace(txt);

    let inline isNotBlank txt =
        not(System.String.IsNullOrWhiteSpace(txt))

    let private validateRequired (def:TextPropertyDefinition) (value:string) =
        if (def.IsRequired && isNotBlank value) then
            ValidValue
        else 
            RequiredButBlank "is required"

    let private validateMinLen (def:TextPropertyDefinition) (value:string) =
        if (isBlank value || (value.Length >= def.MinLen)) then
            ValidValue
        else 
            LessThanMinimumLen ((sprintf "is less than the minimum length of %d" def.MinLen), def.MinLen)

    let private validateMaxLen (def:TextPropertyDefinition) (value:string) =
        if (isBlank value || (value.Length <= def.MaxLen)) then
            ValidValue
        else 
            MoreThanMaxLen ((sprintf "is more than the maximum length of %d" def.MaxLen), def.MaxLen)

    let private validatePattern (def:TextPropertyDefinition) (value:string) =
        if (not(def.UsesRegex) || isBlank value || (Text.RegularExpressions.Regex.IsMatch(value, def.RegexPattern))) then
            ValidValue
        else 
            NotMatchingPattern ((sprintf "does not match pattern of %s" def.RegexPattern), def.RegexPattern)

    let GetTextValidationStates (def:TextPropertyDefinition) (value:string) =
        let states = [validateRequired; validateMinLen; validateMaxLen; validatePattern] |> Seq.map (fun valFunc -> valFunc def value) |> Seq.distinct
        states 
    
    let IsValidTextState states =
        states |> Seq.tryFind (fun s -> s <> ValidValue) |> Option.isNone
        
    let private propValidationErrorCode = "FieldValidation"
    let private convertStateToPropError propName state  =
        match state with 
        | RequiredButBlank s -> Some { ErrorCode = propValidationErrorCode; Description= s; PropertyName = propName}
        | LessThanMinimumLen (s,l) -> Some { ErrorCode = propValidationErrorCode; Description= s; PropertyName = propName}
        | MoreThanMaxLen (s,l) -> Some { ErrorCode = propValidationErrorCode; Description= s; PropertyName = propName}
        | NotMatchingPattern (s,p) -> Some { ErrorCode = propValidationErrorCode; Description= s; PropertyName = propName}
        | ValidValue -> None

    type TextPropertyDefinition with    
        member this.GetPropertyValidationErrors propName value  = 
            let states = GetTextValidationStates this value
            let propErrors = states |> Seq.map (convertStateToPropError propName) |> Seq.where Option.isSome |> Seq.map Option.get
            propErrors
                                                
    
    
