namespace Common.ViewModels

open CommonViewEditors
open FsharpCommonTypes
open FSharp.ViewModule

type ExternalChoicesViewModel<'ParentType>(docPull:'ParentType->int, 
                                                        docUpdate: int->'ParentType, 
                                                        pushUpdatedDoc: CommonViewEditors.IViewComponent<'ParentType> ->'ParentType->unit,
                                                        queryExecutor: string -> seq<ExternalChoicesQueryResult<int>>,
                                                        propName: string,
                                                        defaultValue: int) as self = 
    inherit ViewModelBase()
//    let mutable txtValue = defaultValue
    let mutable currErrors:seq<CommonValidations.PropertyError> = Seq.empty

    let txtValue = self.Factory.Backing(<@ self.Value @>, defaultValue)

    // ...
    let validate () = 
        currErrors <- [ ] // TODO add validation for primitive not being default
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
            self.Value <- (docPull vm)

        member this.OnDocUpdated<'ParentType> vm = 
            self.Value <- (docPull vm)
            ()

        member this.IsValid () = 
            isValueValid
        member this.Label = propName
        member this.UiHint = "ExternalChoices"
    interface IIntExternalChoicesQry with
        member this.QueryExecutor filterStr = queryExecutor filterStr
    

