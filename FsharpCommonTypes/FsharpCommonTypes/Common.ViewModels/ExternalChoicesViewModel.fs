namespace Common.ViewModels

open CommonViewEditors
open FsharpCommonTypes
open FSharp.ViewModule
open System.Collections.ObjectModel

type ExternalChoicesViewModel<'PrimitiveType , 'ParentType when 'PrimitiveType: equality >(propFactory:PropFactoryMethod<'PrimitiveType>,
                                                            refreshValFromDoc:'ParentType->BzProp<'PrimitiveType>, 
                                                            refreshDocFromVal:BzProp<'PrimitiveType>->'ParentType, // allow create new doc by sending the newly BzProp<'PrimitiveType>
                                                            pushUpdatedDoc: CommonViewEditors.IViewComponent<'ParentType> ->'ParentType->unit,
                                                            queryExecutor: string -> seq<SimpleExternalChoicesQueryResult<'PrimitiveType>>,
                                                            labelLkup: 'PrimitiveType -> string,
                                                            propName: string,
                                                            defaultValue: 'PrimitiveType) as self = 
    inherit ViewModelBase()
//    let mutable txtValue = defaultValue
    let mutable currErrors:seq<PropertyError> = Seq.empty
    
    let getStrErrors = BusinessTypes.GetStrErrors propFactory

    let txtValue = self.Factory.Backing(<@ self.Value @>, defaultValue, getStrErrors)
    let txtLabel = self.Factory.Backing(<@ self.ResultLabel @>, "")


    let isValueValid = 
        currErrors |> Seq.isEmpty
    let alertParentOfDocChg newVal =
        let newDoc = refreshDocFromVal newVal
        pushUpdatedDoc self newDoc

        
    member self.Value with get() = txtValue.Value 
                        and set value = 
                            if (value <> txtValue.Value) then
                                txtValue.Value <- value
                                let newPropState = propFactory value
                                alertParentOfDocChg newPropState 

    member self.ResultLabel with get() = txtLabel.Value 
                            and set value = 
                                if (value <> txtLabel.Value) then
                                    txtLabel.Value <- value
                                   
    member self.PropName with get() = propName

    interface CommonViewEditors.IViewComponent<'ParentType> with
        member this.Init<'ParentType> vm = 
            let primitiveVal = BusinessTypes.ToPrimitive (refreshValFromDoc vm)
            self.Value <- primitiveVal
            self.ResultLabel <- labelLkup primitiveVal

        member this.OnDocUpdated<'ParentType> vm = 
            let primitiveVal = BusinessTypes.ToPrimitive (refreshValFromDoc vm)
            self.Value <- primitiveVal
            self.ResultLabel <- labelLkup primitiveVal
            
    interface Interfaces.IViewComponent with 
        member this.Label = propName
        member this.UiHint = "ExternalChoices"
    interface IExternalChoicesQry<'PrimitiveType> with
        member this.QueryExecutor filterStr = 
            queryExecutor filterStr
    