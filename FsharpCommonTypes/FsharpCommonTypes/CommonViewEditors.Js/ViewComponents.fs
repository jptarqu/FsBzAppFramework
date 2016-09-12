namespace CommonViewEditors.Js

module ViewComponents =
    open CommonViewEditors
    open CommonValidations
    open FsharpCommonTypes.InterfaceTypes
    open FsharpCommonTypes
  

//    let private createHtmlViewComponent 

    type DocChgListener< 'ParentType> = (('ParentType->'ParentType) -> unit)

    type ExternalChoicesViewComponent<'ViewModel>( htmlElementService:JsElementInterfaces.IHtmlElementService, 
            inputDef: ExternalChoicesInputDefinition<'ViewModel , 'Result> ) =
        let mutable currIdx =""
        let mutable chgListener:DocChgListener< 'ParentType> = (fun _ -> ())
        let mutable isValid = true
        let mutable refreshOnVmChange = false
        let mutable currChoices : seq<ExternalChoicesQueryResult<'Result>> =Seq.empty
    
        let propDisplayName = inputDef.PropDisplayName
        let docPull = inputDef.DocumentPull
        let docPush = inputDef.DocumentUpdate
        let choicesQry = inputDef.ExternalChoicesQueryExecutor
        let valueChangedInField (htmlField:JsElementInterfaces.IHtmlChoicesField) (newVal:string) = 
            if newVal = "" then 
                isValid <- false
                htmlField.SetErrors  ["This is a required field"]
            else
                if (newVal <> currIdx) then 
                    isValid <- true
                    currIdx <- newVal
                    let newSelectedItem = currChoices |> Seq.find (fun i -> i.ResultId = currIdx)
                    let partialUpdate = docPush newSelectedItem
                    chgListener partialUpdate
        let htmlField  = htmlElementService.AddChoicesField propDisplayName //valueChangedInField
        let updateHtmlField newIdx =
            if (newIdx <> currIdx) then
                currIdx <- newIdx 
                htmlField.SetValue currIdx
        member this.AddValueSubscriber sub =
            chgListener <- sub
        with 
        interface IViewComponent<'ViewModel> with
            member this.Init<'ViewModel> vm =  // this is just for initialization routines not for the 1st data update
                htmlField.SetChangeCallback valueChangedInField
                currChoices <- choicesQry vm ""
                ()

            member this.OnDocUpdated<'ViewModel> vm =  // will be acalled at least onece for all inputs
                let newIdx = docPull vm
                updateHtmlField newIdx
                if refreshOnVmChange then
                    currChoices <- choicesQry vm (htmlField.GetTextEntered())
            member this.IsValid () = isValid
            
    type BoolViewComponent<'ViewModel>( htmlElementService:JsElementInterfaces.IHtmlElementService, 
            inputDef: BooleanInputDefinition<'ViewModel > ) =
        let mutable currVal = false
        let mutable chgListener:DocChgListener< 'ParentType> = (fun _ -> ())
        let mutable isValid = true
        let propDisplayName = inputDef.PropDisplayName
        let docPull = inputDef.DocumentPull
        let docPush = inputDef.DocumentUpdate
        let boolSromStr str = 
           (str <> "True")
        let valueChangedInField (htmlField:JsElementInterfaces.IHtmlBooleanField) (newVal:string) = 
            if (newVal <> "True" && newVal <> "False")  then 
                isValid <- false
                htmlField.SetErrors  ["This is a required field"]
            else
                let newBoolVal = boolSromStr newVal 
                if (currVal <> newBoolVal) then 
                    currVal <- newBoolVal
                    isValid <- true
                    let partialUpdate = docPush currVal
                    chgListener partialUpdate
        let htmlField  = htmlElementService.AddBooleanField propDisplayName //valueChangedInField 
        let updateHtmlField newVal =
            if (newVal <> currVal) then
                currVal <- newVal 
                htmlField.SetValue newVal
        member this.AddValueSubscriber sub =
            chgListener <- sub
        with 
        interface IViewComponent<'ViewModel> with
            member this.Init<'ViewModel> vm = 
                htmlField.SetChangeCallback valueChangedInField
                ()

            member this.OnDocUpdated<'ViewModel> vm = 
                let newVal = docPull vm
                updateHtmlField newVal
            member this.IsValid () = isValid

    type TextViewComponent<'ViewModel>( htmlElementService:JsElementInterfaces.IHtmlElementService, 
            inputDef: TextInputDefinition<'ViewModel > ) =
        let mutable currVal = ""
        let mutable chgListener:DocChgListener< 'ParentType> = (fun _ -> ())
        let mutable isValid = true
        let propDisplayName = inputDef.PropDisplayName
        let docPull = inputDef.DocumentPull
        let docPush = inputDef.DocumentUpdate
        let validator = inputDef.GetPropertyValidationErrors propDisplayName
        let valueChangedInField (htmlField:JsElementInterfaces.IHtmlBooleanField) (newVal:string) = 
            let errors = validator newVal
            if Seq.isEmpty errors then 
                currVal <- newVal
                let partialUpdate = docPush currVal
                chgListener partialUpdate
            else
                isValid <- false
                htmlField.SetErrors  (errors |> Seq.map (fun e -> e.DisplayAsPropErrorString() ))
        let htmlField  = htmlElementService.AddBooleanField propDisplayName //valueChangedInField 
        let updateHtmlField newVal =
            if (newVal <> currVal) then
                currVal <- newVal 
                htmlField.SetValue newVal
                ()
        member this.AddValueSubscriber sub =
            chgListener <- sub
        with 
        interface IViewComponent<'ViewModel> with
            member this.Init<'ViewModel> vm = 
                htmlField.SetChangeCallback valueChangedInField
                ()

            member this.OnDocUpdated<'ViewModel> vm = 
                let newVal = docPull vm
                updateHtmlField newVal
            member this.IsValid () = isValid

    type ConstraintTextInputViewComponent<'ViewModel>( addFieldCall:(string->JsElementInterfaces.TextChangedCallback<'FieldType>->'FieldType), 
        inputDef:ConstraintedInputDefinition< 'PropType , 'ParentType> , 
        textDef:TextType.TextPropertyDefinition,
        valConversion:(string->'PrimitiveType )
         ) =
        let mutable currValue = ""
        let mutable chgListener:DocChgListener< 'ParentType> = (fun _ -> _)
        let mutable isValid = true
        let propDisplayName = inputDef.PropDisplayName
        let validator = textDef.GetPropertyValidationErrors propDisplayName
        let docPull = inputDef.DocumentPull
        let docPush = inputDef.DocumentUpdate
        let valueChangedInField (htmlField:JsElementInterfaces.IHtmlTextField) newVal = 
            let errors = validator newVal
            if Seq.isEmpty errors then 
                currValue <- (valConversion newVal)
                let partialUpdate = docPush currValue
                chgListener partialUpdate
            else
                isValid <- false
                htmlField.SetErrors  (errors |> Seq.map (fun e -> e.DisplayAsPropErrorString () ))
        let htmlField  = addFieldCall propDisplayName valueChangedInField 
        member this.AddValueSubscriber sub =
            chgListener <- sub
        with 
        interface IViewComponent<'ViewModel> with
            member this.OnDocUpdated<'ViewModel> vm = 
                let newVal = docPull vm
                if (newVal <> currValue) then
                    currValue <- newVal // Should we call chgListener ?? no

            member this.IsValid () = isValid

    type IntViewComponent<'ViewModel>( htmlElementService:JsElementInterfaces.IHtmlElementService, propDisplayName:string , intDef:IntegerTypes.IntegerPropertyDefinition ) =
        let mutable currValue = 0
        let mutable isValid = true
        let validator = intDef.GetPropertyValidationErrors propDisplayName
        let valueChangedInField (htmlField:JsElementInterfaces.IHtmlIntegerField) newVal = 
            let errors = validator newVal
            if Seq.isEmpty errors then 
                currValue <- (ConversionHelpers.tryParseInt newVal).Value
            else
                htmlField.SetErrors  (errors |> Seq.map (fun e -> e.DisplayAsPropErrorString () ))
        let htmlField  = htmlElementService.AddIntegerField propDisplayName valueChangedInField 
        with 
        interface IViewComponent<'ViewModel> with
            member this.OnDocUpdated<'ViewModel> vm = 
                1 |> ignore
            member this.IsValid () = isValid
            
    type DecimalViewComponent<'ViewModel>( htmlElementService:JsElementInterfaces.IHtmlElementService, propDisplayName:string , intDef:DecimalTypes.DecimalPropertyDefinition ) =
        let mutable currValue = 0M
        let mutable isValid = true
        let validator = intDef.GetPropertyValidationErrors propDisplayName
        let valueChangedInField (htmlField:JsElementInterfaces.IHtmlDecimalField) newVal = 
            let errors = validator newVal
            if Seq.isEmpty errors then 
                currValue <- (ConversionHelpers.tryParseDecimal newVal).Value
            else
                htmlField.SetErrors  (errors |> Seq.map (fun e -> e.DisplayAsPropErrorString () ))
        let htmlField  = htmlElementService.AddDecimalField propDisplayName valueChangedInField 
        with 
        interface IViewComponent<'ViewModel> with
            member this.OnDocUpdated<'ViewModel> vm = 
                1 |> ignore
            member this.IsValid () = isValid
    