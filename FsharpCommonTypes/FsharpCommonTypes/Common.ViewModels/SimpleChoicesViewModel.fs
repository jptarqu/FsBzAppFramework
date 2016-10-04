namespace Common.ViewModels

open CommonViewEditors
open FsharpCommonTypes
open FSharp.ViewModule
open System.Collections.ObjectModel

type SimpleChoicesViewModel<'PrimitiveType , 'ParentType when 'PrimitiveType: equality >(propFactory:BusinessTypes.PropFactoryMethod<'PrimitiveType>,
                                                            refreshValFromDoc:'ParentType->BusinessTypes.BzProp<'PrimitiveType>, 
                                                            refreshDocFromVal:BusinessTypes.BzProp<'PrimitiveType>->'ParentType, // allow create new doc by sending the newly BzProp<'PrimitiveType>
                                                            pushUpdatedDoc: CommonViewEditors.IViewComponent<'ParentType> ->'ParentType->unit,
                                                            choices: seq<SimpleExternalChoicesQueryResult<'PrimitiveType>>,
                                                            propName: string,
                                                            defaultValue: 'PrimitiveType) as self = 
    inherit ViewModelBase()
//    let mutable txtValue = defaultValue
    let mutable currErrors:seq<CommonValidations.PropertyError> = Seq.empty
    

    let getStrErrors = BusinessTypes.GetStrErrors propFactory
    
    let txtValue = self.Factory.Backing(<@ self.Value @>, defaultValue, getStrErrors)


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

    member self.PossibleChoices with get() = choices
                                   
    member self.PropName with get() = propName

    interface CommonViewEditors.IViewComponent<'ParentType> with
        member this.Init<'ParentType> vm = 
            let primitiveVal = BusinessTypes.ToPrimitive (refreshValFromDoc vm)
            self.Value <- primitiveVal

        member this.OnDocUpdated<'ParentType> vm = 
            let primitiveVal = BusinessTypes.ToPrimitive (refreshValFromDoc vm)
            self.Value <- primitiveVal

        member this.Label = propName
        member this.UiHint = "SimpleChoices"
    