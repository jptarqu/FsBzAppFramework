namespace Common.ViewModels

open FsharpCommonTypes
open FSharp.ViewModule
open System.Collections.ObjectModel
open Common.ViewModels.Interfaces

type SimpleChoicesViewModel<'PrimitiveType, 'ParentType when 'PrimitiveType : equality>(propFactory : PropFactoryMethod<'PrimitiveType>, refreshValFromDoc : 'ParentType -> BzProp<'PrimitiveType>, refreshDocFromVal : BzProp<'PrimitiveType> -> 'ParentType, // allow create new doc by sending the newly BzProp<'PrimitiveType>
                                                                                                                                                                                                                                                               pushUpdatedDoc : Common.ViewModels.Interfaces.IViewComponent<'ParentType> -> 'ParentType -> unit, choicesQry : 'ParentType -> list<SimpleExternalChoicesQueryResult<'PrimitiveType>>, propName : string, defaultValue : 'PrimitiveType) as self = 
    inherit ViewModelBase()
    let mutable currErrors : seq<PropertyError> = Seq.empty
    let mutable currPossibleChoicesRaw : list<SimpleExternalChoicesQueryResult<'PrimitiveType>> = List.empty
    let getStrErrors = BusinessTypes.GetStrErrors propFactory
    let txtValue = self.Factory.Backing(<@ self.Value @>, defaultValue, getStrErrors)
    let possibleChoices = ObservableCollection<SimpleExternalChoicesQueryResult<'PrimitiveType>>()
    let isValueValid = currErrors |> Seq.isEmpty
    
    let alertParentOfDocChg newVal = 
        let newDoc = refreshDocFromVal newVal
        pushUpdatedDoc self newDoc
    
    let refreshObservableCollection() = 
        possibleChoices.Clear()
        for item in currPossibleChoicesRaw do
            possibleChoices.Add item
    
    let refreshPossibleChoice newDoc = 
        let newChoices = choicesQry newDoc
        
        let comparisonResult = 
            newChoices |> List.compareWith (fun x y -> 
                              if (x = y) then 0
                              else 1) currPossibleChoicesRaw
        if (comparisonResult <> 0) then 
            currPossibleChoicesRaw <- newChoices
            refreshObservableCollection()
    
    member self.Value 
        with get () = txtValue.Value
        and set value = 
            if (value <> txtValue.Value) then 
                txtValue.Value <- value
                let newPropState = propFactory value
                alertParentOfDocChg newPropState
    
    member self.PossibleChoices = possibleChoices
    member self.PropName = propName
    
    interface Common.ViewModels.Interfaces.IViewComponent<'ParentType> with
        
        member this.Init<'ParentType> vm = 
            let primitiveVal = BusinessTypes.ToPrimitive(refreshValFromDoc vm)
            self.Value <- primitiveVal
            refreshPossibleChoice vm
        
        member this.OnDocUpdated<'ParentType> vm = 
            let primitiveVal = BusinessTypes.ToPrimitive(refreshValFromDoc vm)
            self.Value <- primitiveVal
            refreshPossibleChoice vm
    
    interface Interfaces.IViewComponent with
        member this.Label = propName
        member this.UiHint = "SimpleChoices"

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module SimpleChoicesViewModel = 
    module UIHints = 
        let SimpleChoices = "SimpleChoices"

    let AddSimpleChoicesViewModel (docViewModel : #Interfaces.IDocViewModel<'ParentType>) 
        (intoPanelViewModel : #Interfaces.IPanelViewModel<'ParentType>) 
        (propDef : PropDefinition<'ParentType, 'Primitive>) simpleChoices = 
        let docUpdate = docViewModel.GetDocAccessor(propDef.Setter)
        let choicesInput = 
            SimpleChoicesViewModel
                (propDef.Factory, propDef.Getter, docUpdate, docViewModel.UpdateDoc, simpleChoices, propDef.Name, 0)
        intoPanelViewModel.AddChild(choicesInput)
