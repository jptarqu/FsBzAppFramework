namespace Common.ViewModels

open FSharp.ViewModule
open FsharpCommonTypes


type SingleInputViewModel<'PrimitiveType, 'ParentType>(docPull:'ParentType->'PrimitiveType, 
                                                        docUpdate:string->'ParentType, 
                                                        pushUpdatedDoc: 'ParentType->unit,
                                                        propConstraint: TextType.TextPropertyDefinition,
                                                        propName: string) as self = 
    inherit ViewModelBase()
    let defaultValue = ""
//    let mutable txtValue = defaultValue
    let mutable currErrors:seq<CommonValidations.PropertyError> = Seq.empty

    let txtValue = self.Factory.Backing(<@ self.Value @>, "")

    // ...
    let validate () = 
        currErrors <- propConstraint.GetPropertyValidationErrors propName self.Value
    let isValueValid = 
        currErrors |> Seq.isEmpty
    let alertParentOfDocChg newVal =
        let newDoc = docUpdate newVal
        pushUpdatedDoc newDoc

    member self.Value with get() = txtValue.Value 
                        and set value = 
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


