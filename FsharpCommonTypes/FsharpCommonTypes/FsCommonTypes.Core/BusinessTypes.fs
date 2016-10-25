namespace FsharpCommonTypes

module BusinessTypes =
    open TextType
    open InterfaceTypes

   
    let ToPrimitive propState =
        match propState with
        | ValidProp primitive -> primitive
        | InvalidProp (badPrimitive, errors) -> badPrimitive

    let ToPrimitiveStr propState =
        match propState with
        | ValidProp primitive -> primitive.ToString()
        | InvalidProp (badPrimitive, errors) -> badPrimitive.ToString()

    let formatObj strFormat (obj:System.Object) =
        System.String.Format(strFormat, obj)

    let ToPrimitiveStrFormat (strFormat:string) propState =
        match propState with
        | ValidProp (primitive) -> formatObj strFormat primitive
        | InvalidProp (badPrimitive, errors) -> formatObj strFormat badPrimitive
        
    let GetStrErrors (propFactory:PropFactoryMethod<'InputPrimitive, 'Primitive>) (newPrimitiveVal:'InputPrimitive) =
        let newPropState = propFactory newPrimitiveVal
        let strErrors = match newPropState with
                        | ValidProp primtive -> Seq.empty
                        | InvalidProp (badPrimitive, errors) -> errors |> Seq.map string
        strErrors |> Seq.toList
       
    let GetValidationErrors doc (propDef:#seq<#IPropValidator<'ParentType>>) =
         propDef |> Seq.collect (fun p -> p.GetValidationErrors doc)
         
//    type PropNameValuePair =   

    type OptPastDateTimeType =  BzProp<Option<System.DateTime>> 
    let OptPastDateTime (newValue:Option<System.DateTime>) =
        let now = System.DateTime.Now
        match  newValue with
        | Some newDateVal ->
            if (newDateVal > now) then
                InvalidProp (newValue , ["date is in the future"])
            else
                ValidProp newValue
        | None -> InvalidProp (newValue , ["date is required"])
            
    type PastDateTimeType =  BzProp<System.DateTime>
    let PastDateTime (newValue:System.DateTime) =
        let now = System.DateTime.Now
        if (newValue > now) then
            InvalidProp (newValue , ["date is in the future"])
        else
            ValidProp newValue
    let PastDateTimeFromStr (newValue:string) =
        match (ConversionHelpers.tryParseDate newValue) with
        | Some d ->
            PastDateTime d
        | None ->  
            let tomorrow = System.DateTime.Now.AddDays(float 1)
            InvalidProp (tomorrow , ["invalid date string"])

    type ShortNameType =  BzProp<string> 
    let ShortName (newValue:string) =
        if (newValue.Length < 1) then
            InvalidProp (newValue , ["too short"])
        else if (newValue.Length > 30) then
            InvalidProp (newValue , ["too long"])
        else
            ValidProp newValue

    type LongNameType =  BzProp<string> 
    let LongName (newValue:string) =
        if (newValue.Length < 3) then
            InvalidProp (newValue , ["too short"])
        else
            ValidProp newValue
    
    type IdNumberType = BzProp<int> 
    let IdNumber (newValue:int) =
        if (newValue < 0) then
            InvalidProp (newValue , ["invalid id"])
        else
            ValidProp newValue
    let IdNumberFromStr (newValue:string) =
        let cleanMask = newValue.Replace("_","")
        match (ConversionHelpers.tryParseInt cleanMask) with
        | Some i ->
            IdNumber i
        | None ->  InvalidProp (-1 , ["invalid number"])


    // LongName wuld then be called by the ViewModel onChnage (maybe at the validation func passed to FSharp.ViewModule)
    //   where the validation would check if the creation method returned InvalidProp.
    //   SO, ViewModel needs to get passed the creation method as in creationMethod:'Primitive->BzProp<'Primitive>


//    [<CLIMutable>]
//    type ShortName =
//        {Value : string; PropName: string}
//        with
//        static member private _fieldDefinition = RequiredTextProp 1 20
//        member this.GetValidationErrors () = ShortName._fieldDefinition.GetPropertyValidationErrors this.PropName this.Value 
//        member this.GetDefinition () = ShortName._fieldDefinition 
//        member this.WithValue newValue = { this with Value = newValue}
//        interface ITextEditableProperty with 
//            member this.GetFieldDefinition () = ShortName._fieldDefinition 
//
//    let NewShortName propName name =
//        {Value = name; PropName = propName}

    
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
        

