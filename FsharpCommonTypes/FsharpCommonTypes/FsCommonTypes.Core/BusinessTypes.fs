namespace FsharpCommonTypes

module BusinessTypes =
    open TextType
    open InterfaceTypes

    type ErrorMessage = string
    
    // with types like this, validation occurs at the moment of creation. A prop is either valid or invlid based on 
    //  the construction parameters
    // do we need     [<CLIMutable>] here??
    type BzProp<'Primitive> = 
        | ValidProp of 'Primitive
        | InvalidProp of 'Primitive * seq<ErrorMessage>
    
    let ToPrimitive propState =
        match propState with
        | ValidProp primitive -> primitive
        | InvalidProp (badPrimitive, errors) -> badPrimitive

    type PropFactoryMethod<'Primitive> =
        'Primitive->BzProp<'Primitive>
        
    let GetStrErrors (propFactory:PropFactoryMethod<'Primitive>) (newPrimitiveVal:'Primitive) =
        let newPropState = propFactory newPrimitiveVal
        let strErrors = match newPropState with
                        | ValidProp primtive -> Seq.empty
                        | InvalidProp (badPrimitive, errors) -> errors |> Seq.map string
        strErrors |> Seq.toList

    let LongName (newValue:string) =
        if (newValue.Length < 3) then
            InvalidProp (newValue , ["too short"])
        else
            ValidProp newValue
    type LongNameType = BzProp<string> 

    let IdNumber (newValue:int) =
        if (newValue < 0) then
            InvalidProp (newValue , ["invalid id"])
        else
            ValidProp newValue
    type IdNumberType = BzProp<int> 
    // LongName wuld then be called by the ViewModel onChnage (maybe at the validation func passed to FSharp.ViewModule)
    //   where the validation would check if the creation method returned InvalidProp.
    //   SO, ViewModel needs to get passed the creation method as in creationMethod:'Primitive->BzProp<'Primitive>


    [<CLIMutable>]
    type ShortName =
        {Value : string; PropName: string}
        with
        static member private _fieldDefinition = RequiredTextProp 1 20
        member this.GetValidationErrors () = ShortName._fieldDefinition.GetPropertyValidationErrors this.PropName this.Value 
        member this.GetDefinition () = ShortName._fieldDefinition 
        member this.WithValue newValue = { this with Value = newValue}
        interface ITextEditableProperty with 
            member this.GetFieldDefinition () = ShortName._fieldDefinition 

    let NewShortName propName name =
        {Value = name; PropName = propName}

    
    [<CLIMutable>]
    type PositiveAmount =
        {TextValue : string; PropName: string; PositiveAmountValue: decimal}
        with
        static member private _fieldDefinition = DecimalTypes.DecimalPropertyDefinition((decimal 0), 999999999M, 2)
        member this.GetValidationErrors () = 
            PositiveAmount._fieldDefinition.GetPropertyValidationErrors this.PropName this.TextValue 
        interface IDecimalEditableProperty with 
            member this.GetFieldDefinition () = PositiveAmount._fieldDefinition 
        interface IConvertibleFromString<PositiveAmount> with 
            member this.SetValueFromText txtValue = 
                match PositiveAmount._fieldDefinition.GetValueFromText txtValue with
                | Some convertedValue -> {TextValue = txtValue; PropName = this.PropName; PositiveAmountValue = convertedValue}
                | None -> {TextValue = txtValue; PropName = this.PropName; PositiveAmountValue = 0M}
                
    let NewPositiveAmount propName amt =
         {TextValue = amt.ToString(); PropName= propName; PositiveAmountValue =amt }
            
//    [<CLIMutable>]
//    type IdNumber =
//        {TextValue : string; PropName: string; IdNumberValue: int}
//        with
//        static member private _fieldDefinition = IntegerTypes.IntegerPropertyDefinition(1, System.Int32.MaxValue)
//        member this.GetValidationErrors () = 
//            IdNumber._fieldDefinition.GetPropertyValidationErrors this.PropName this.TextValue 
//        interface IIntegerEditableProperty with 
//            member this.GetFieldDefinition () = IdNumber._fieldDefinition 
//        interface IConvertibleFromString<IdNumber> with 
//            member this.SetValueFromText txtValue = 
//                match IdNumber._fieldDefinition.GetValueFromText txtValue with
//                | Some convertedValue -> {TextValue = txtValue; PropName = this.PropName; IdNumberValue = convertedValue}
//                | None -> {TextValue = txtValue; PropName = this.PropName; IdNumberValue = 0}
//                
//
//    let NewIdNumber propName idValue =
//         {TextValue = idValue.ToString(); PropName= propName; IdNumberValue = idValue }
//
//    type IdWrapper<'Entity> = 
//        {Id : IdNumber; Entity: 'Entity} with
//        member this.GetValidationErrors () = 
//            [ this.Id.GetValidationErrors();  ] |> CollectAllError // TODO should Entity also validate??, maybe not becuase it would almost alwasy be the result of a lookup. we are not editing the associated relation
//            
//    let NewIdWrapper entity idValue =
//         {Entity= entity; Id = idValue }
        

