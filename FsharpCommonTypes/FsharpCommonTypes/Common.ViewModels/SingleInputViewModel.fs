namespace Common.ViewModels

open FSharp.ViewModule
open FsharpCommonTypes


type SingleInputViewModel<'PrimitiveType, 'ParentType>(propFactory:BusinessTypes.PropFactoryMethod<'PrimitiveType>,
                                                        refreshValFromDoc:'ParentType->'PrimitiveType, 
                                                        refreshDocFromVal:BusinessTypes.BzProp<'PrimitiveType>->'ParentType, // allow create new doc by sending the newly BzProp<'PrimitiveType>
                                                        pushUpdatedDoc: CommonViewEditors.IViewComponent<'ParentType> ->'ParentType->unit,
                                                        propName: string,
                                                        defaultValue: 'PrimitiveType) as self = 
    inherit ViewModelBase()
    
//    let mutable txtValue = defaultValue
    let mutable currErrors:seq<BusinessTypes.ErrorMessage> = Seq.empty
    
    //let validate () = 
    //    currErrors <- propConstraint.GetPropertyValidationErrors propName self.Value

    let getValErrors newPrimitiveVal =
        let newPropState = propFactory newPrimitiveVal
        let strErrors = match newPropState with
                        | ValidProp primtive -> Seq.empty
                        | InvalidProp (badPrimitive, errors) -> errors
        strErrors

    let txtValue = self.Factory.Backing(<@ self.Value @>, defaultValue, getValErrors)

    // ...
    
    let alertParentOfDocChg newVal =
        let newDoc = refreshDocFromVal newVal
        pushUpdatedDoc self newDoc

    member self.Value with get() = txtValue.Value 
                        and set value = 
                            if (value <> txtValue.Value) then
                                txtValue.Value <- value
                                let newPropState = propFactory newPrimitiveVal
                                alertParentOfDocChg newPropState // always send to doc, even if invalid state?

    member self.PropName with get() = propName

    interface CommonViewEditors.IViewComponent<'ParentType> with
        member this.Init<'ParentType> vm = 
            self.Value <- BusinessTypes.ToPrimitive (docPull vm)

        member this.OnDocUpdated<'ParentType> vm = 
            self.Value <- BusinessTypes.ToPrimitive (docPull vm)
            ()

        member this.Label = propName
        member this.UiHint = "SingleInput"

type OldSingleInputViewModel<'PrimitiveType, 'ParentType>(docPull:'ParentType->'PrimitiveType, 
                                                        docUpdate:string->'ParentType, 
                                                        pushUpdatedDoc: CommonViewEditors.IViewComponent<'ParentType> ->'ParentType->unit,
                                                        propConstraint: TextType.TextPropertyDefinition,
                                                        propName: string) as self = 
    inherit ViewModelBase()
    let defaultValue = ""
//    let mutable txtValue = defaultValue
    let mutable currErrors:seq<CommonValidations.PropertyError> = Seq.empty
    
    let validate () = 
        currErrors <- propConstraint.GetPropertyValidationErrors propName self.Value

    let getValErrors newVal =
        validate()
        let strErrors = currErrors |> Seq.map (fun e -> e.Description) |> Seq.toList
        strErrors

    let txtValue = self.Factory.Backing(<@ self.Value @>, "", getValErrors)

    // ...
    let isValueValid = 
        currErrors |> Seq.isEmpty
    let alertParentOfDocChg newVal =
        let newDoc = docUpdate newVal
        pushUpdatedDoc self newDoc

    member self.Value with get() = txtValue.Value 
                        and set value = 
                            if (value <> txtValue.Value) then
                                txtValue.Value <- value
                                validate()
                                if (isValueValid) then
                                   alertParentOfDocChg txtValue.Value

    member self.PropName with get() = propName

    interface CommonViewEditors.IViewComponent<'ParentType> with
        member this.Init<'ParentType> vm = 
            self.Value <- (docPull vm).ToString()

        member this.OnDocUpdated<'ParentType> vm = 
            self.Value <- (docPull vm).ToString()
            ()

        member this.IsValid () = 
            isValueValid
        member this.Label = propName
        member this.UiHint = "SingleInput"


