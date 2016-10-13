namespace Common.ViewModels

open CommonViewEditors
open FsharpCommonTypes
open FSharp.ViewModule
open System.Collections.ObjectModel

type SimpleChoicesViewModel<'PrimitiveType , 'ParentType when 'PrimitiveType: equality >(propFactory:PropFactoryMethod<'PrimitiveType>,
                                                            refreshValFromDoc:'ParentType->BzProp<'PrimitiveType>, 
                                                            refreshDocFromVal:BzProp<'PrimitiveType>->'ParentType, // allow create new doc by sending the newly BzProp<'PrimitiveType>
                                                            pushUpdatedDoc: CommonViewEditors.IViewComponent<'ParentType> ->'ParentType->unit,
                                                            choices: seq<SimpleExternalChoicesQueryResult<'PrimitiveType>>,
                                                            propName: string,
                                                            defaultValue: 'PrimitiveType) as self = 
    inherit ViewModelBase()
//    let mutable txtValue = defaultValue
    let mutable currErrors:seq<PropertyError> = Seq.empty
    

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
    
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module SimpleChoicesViewModel =
    let AddSimpleChoicesViewModel (docViewModel:#Interfaces.IDocViewModel<'ParentType>) (propDef:PropDefinition<'ParentType, 'Primitive>) simpleChoices  =
        let docUpdate = docViewModel.GetDocAccessor(propDef.Setter)
        let choicesInput = SimpleChoicesViewModel(propDef.Factory, propDef.Getter, docUpdate,  docViewModel.UpdateDoc, simpleChoices, propDef.Name, 0)
        docViewModel.AddChild(choicesInput)