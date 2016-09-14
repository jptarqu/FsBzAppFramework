namespace CommonViewEditors.Wpf

module ViewComponents =
    open CommonViewEditors
    open CommonValidations
    open FsharpCommonTypes.InterfaceTypes
    open FsharpCommonTypes
  

//    let private createViewComponent 

    type DocChgListener< 'ParentType> = (('ParentType->'ParentType) -> unit)

    type ExternalChoicesViewComponent<'ViewModel>( wpfElementService:WpfElementInterfaces.IElementService, 
                                                    inputDef:ExternalChoicesInputDefinition<'ViewModel , 'Result> ) =
        let mutable currIdx =""
        let mutable chgListener:DocChgListener< 'ParentType> = (fun _ -> ())
        let mutable isValid = true
        let mutable refreshOnVmChange = false
        let mutable currChoices : seq<ExternalChoicesQueryResult<'Result>> =Seq.empty
    
        let propDisplayName = inputDef.PropDisplayName
        let docPull = inputDef.DocumentPull
        let docPush = inputDef.DocumentUpdate
        let choicesQry = inputDef.ExternalChoicesQueryExecutor
        let valueChangedInField (wpfField:WpfElementInterfaces.IChoicesField) (newVal:string) = 
            if newVal = "" then 
                isValid <- false
                wpfField.SetErrors  ["This is a required field"]
            else
                if (newVal <> currIdx) then 
                    isValid <- true
                    currIdx <- newVal
                    let newSelectedItem = currChoices |> Seq.find (fun i -> i.ResultId = currIdx)
                    let partialUpdate = docPush newSelectedItem
                    chgListener partialUpdate
        let wpfField  = wpfElementService.AddChoicesField propDisplayName //valueChangedInField
        let updateField newIdx =
            if (newIdx <> currIdx) then
                currIdx <- newIdx 
                wpfField.SetValue currIdx
        member this.AddValueSubscriber sub =
            chgListener <- sub
        with 
        interface IViewComponent<'ViewModel> with
            member this.Init<'ViewModel> vm =  // this is just for initialization routines not for the 1st data update
                wpfField.SetChangeCallback valueChangedInField
                currChoices <- choicesQry vm ""
                ()

            member this.OnDocUpdated<'ViewModel> vm =  // will be acalled at least onece for all inputs
                let newIdx = docPull vm
                updateField newIdx
                if refreshOnVmChange then
                    currChoices <- choicesQry vm (wpfField.GetTextEntered())
            member this.IsValid () = isValid
            
    type BoolViewComponent<'ViewModel>( wpfElementService:WpfElementInterfaces.IElementService, 
                                        inputDef: BooleanInputDefinition<'ViewModel > ) =
        let mutable currVal = false
        let mutable chgListener:DocChgListener< 'ParentType> = (fun _ -> ())
        let mutable isValid = true
        let propDisplayName = inputDef.PropDisplayName
        let docPull = inputDef.DocumentPull
        let docPush = inputDef.DocumentUpdate
        let boolSromStr str = 
           (str <> "True")
        let valueChangedInField (wpfField:WpfElementInterfaces.IBooleanField) (newVal:string) = 
            if (newVal <> "True" && newVal <> "False")  then 
                isValid <- false
                wpfField.SetErrors  ["This is a required field"]
            else
                let newBoolVal = boolSromStr newVal 
                if (currVal <> newBoolVal) then 
                    currVal <- newBoolVal
                    isValid <- true
                    let partialUpdate = docPush currVal
                    chgListener partialUpdate
        let wpfField  = wpfElementService.AddBooleanField propDisplayName //valueChangedInField 
        let updateField newVal =
            if (newVal <> currVal) then
                currVal <- newVal 
                wpfField.SetValue newVal
        member this.AddValueSubscriber sub =
            chgListener <- sub
        with 
        interface IViewComponent<'ViewModel> with
            member this.Init<'ViewModel> vm = 
                wpfField.SetChangeCallback valueChangedInField
                ()

            member this.OnDocUpdated<'ViewModel> vm = 
                let newVal = docPull vm
                updateField newVal
            member this.IsValid () = isValid

    type TextViewComponent<'ViewModel>( wpfElementService:WpfElementInterfaces.IElementService, 
                                        inputDef:TextInputDefinition<'ViewModel > ) =
        let mutable currVal = ""
        let mutable chgListener:DocChgListener< 'ParentType> = (fun _ -> ())
        let mutable isValid = true
        let propDisplayName = inputDef.PropDisplayName
        let docPull = inputDef.DocumentPull
        let docPush = inputDef.DocumentUpdate
        let contraint = inputDef.Constraint
        let validator = contraint.GetPropertyValidationErrors propDisplayName
        let valueChangedInField (wpfField:WpfElementInterfaces.ITextField) (newVal:string) = 
            let errors = validator newVal
            if Seq.isEmpty errors then 
                currVal <- newVal
                let partialUpdate = docPush currVal
                chgListener partialUpdate
            else
                isValid <- false
                wpfField.SetErrors  (errors |> Seq.map (fun e -> e.DisplayAsPropErrorString() ))
        let wpfField  = wpfElementService.AddTextField propDisplayName 
                                                        contraint.RegexPattern 
                                                        contraint.MinLen
                                                        contraint.MaxLen 
        let updateField newVal =
            if (newVal <> currVal) then
                currVal <- newVal 
                wpfField.SetValue newVal
                ()
        member this.AddValueSubscriber sub =
            chgListener <- sub
        with 
        interface IViewComponent<'ViewModel> with
            member this.Init<'ViewModel> vm = 
                wpfField.SetChangeCallback valueChangedInField
                ()

            member this.OnDocUpdated<'ViewModel> vm = 
                let newVal = docPull vm
                updateField newVal
            member this.IsValid () = isValid
//
//    type ConstraintTextInputViewComponent<'ViewModel>( addFieldCall:(string->WpfElementInterfaces.TextChangedCallback<'FieldType>->'FieldType), 
//        inputDef:ConstraintedInputDefinition< 'PropType , 'ParentType> , 
//        textDef:TextType.TextPropertyDefinition,
//        valConversion:(string->'PrimitiveType )
//         ) =
//        let mutable currValue = ""
//        let mutable chgListener:DocChgListener< 'ParentType> = (fun _ -> _)
//        let mutable isValid = true
//        let propDisplayName = inputDef.PropDisplayName
//        let validator = textDef.GetPropertyValidationErrors propDisplayName
//        let docPull = inputDef.DocumentPull
//        let docPush = inputDef.DocumentUpdate
//        let valueChangedInField (wpfField:WpfElementInterfaces.ITextField) newVal = 
//            let errors = validator newVal
//            if Seq.isEmpty errors then 
//                currValue <- (valConversion newVal)
//                let partialUpdate = docPush currValue
//                chgListener partialUpdate
//            else
//                isValid <- false
//                wpfField.SetErrors  (errors |> Seq.map (fun e -> e.DisplayAsPropErrorString () ))
//        let wpfField  = addFieldCall propDisplayName valueChangedInField 
//        member this.AddValueSubscriber sub =
//            chgListener <- sub
//        with 
//        interface IViewComponent<'ViewModel> with
//            member this.OnDocUpdated<'ViewModel> vm = 
//                let newVal = docPull vm
//                if (newVal <> currValue) then
//                    currValue <- newVal // Should we call chgListener ?? no
//
//            member this.IsValid () = isValid
//
//    type IntViewComponent<'ViewModel>( wpfElementService:WpfElementInterfaces.IElementService, propDisplayName:string , intDef:IntegerTypes.IntegerPropertyDefinition ) =
//        let mutable currValue = 0
//        let mutable isValid = true
//        let validator = intDef.GetPropertyValidationErrors propDisplayName
//        let valueChangedInField (wpfField:WpfElementInterfaces.IIntegerField) newVal = 
//            let errors = validator newVal
//            if Seq.isEmpty errors then 
//                currValue <- (ConversionHelpers.tryParseInt newVal).Value
//            else
//                wpfField.SetErrors  (errors |> Seq.map (fun e -> e.DisplayAsPropErrorString () ))
//        let wpfField  = wpfElementService.AddIntegerField propDisplayName valueChangedInField 
//        with 
//        interface IViewComponent<'ViewModel> with
//            member this.OnDocUpdated<'ViewModel> vm = 
//                1 |> ignore
//            member this.IsValid () = isValid
//            
//    type DecimalViewComponent<'ViewModel>( wpfElementService:WpfElementInterfaces.IElementService, propDisplayName:string , intDef:DecimalTypes.DecimalPropertyDefinition ) =
//        let mutable currValue = 0M
//        let mutable isValid = true
//        let validator = intDef.GetPropertyValidationErrors propDisplayName
//        let valueChangedInField (wpfField:WpfElementInterfaces.IDecimalField) newVal = 
//            let errors = validator newVal
//            if Seq.isEmpty errors then 
//                currValue <- (ConversionHelpers.tryParseDecimal newVal).Value
//            else
//                wpfField.SetErrors  (errors |> Seq.map (fun e -> e.DisplayAsPropErrorString () ))
//        let wpfField  = wpfElementService.AddDecimalField propDisplayName valueChangedInField 
//        with 
//        interface IViewComponent<'ViewModel> with
//            member this.OnDocUpdated<'ViewModel> vm = 
//                1 |> ignore
//            member this.IsValid () = isValid
//    