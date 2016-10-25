namespace Common.ViewModels

open FsharpCommonTypes
open FSharp.ViewModule
open System.Collections.ObjectModel
open Common.ViewModels.Interfaces

type SimpleChoicesViewModel<'InputPrimitive, 'PrimitiveType, 'ParentType when 'PrimitiveType : equality  and 'InputPrimitive : equality>(
    propFactory : PropFactoryMethod<'InputPrimitive, 'PrimitiveType>, 
    propToInput: BzProp<'PrimitiveType>->'InputPrimitive, 
    refreshValFromDoc : 'ParentType -> BzProp<'PrimitiveType>, 
    refreshDocFromVal : BzProp<'PrimitiveType> -> 'ParentType, // allow create new doc by sending the newly BzProp<'PrimitiveType>
    pushUpdatedDoc : Common.ViewModels.Interfaces.IViewComponent<'ParentType> -> 'ParentType -> unit, choicesQry : 'ParentType -> list<SimpleExternalChoicesQueryResult<'PrimitiveType>>,
     propName : string, defaultValue : 'InputPrimitive) as self = 
    inherit ViewModelBase()
    let inputInternal =
            SingleInputViewModel
                (propFactory, propToInput, refreshValFromDoc, refreshDocFromVal, pushUpdatedDoc, propName, defaultValue, "", "")
    let mutable currPossibleChoicesRaw : list<SimpleExternalChoicesQueryResult<'PrimitiveType>> = List.empty
    

    let possibleChoices = ObservableCollection<SimpleExternalChoicesQueryResult<'PrimitiveType>>()
    let isValueValid ()= inputInternal.IsValid 
//    
//    let alertParentOfDocChg newVal = 
//        let newDoc = refreshDocFromVal newVal
//        pushUpdatedDoc self newDoc
    
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
        with get () = inputInternal.Value 
        and set value = 
            inputInternal.Value <- value
    
    member self.PossibleChoices = possibleChoices
    member self.PropName = propName
    
    interface Common.ViewModels.Interfaces.IViewComponent<'ParentType> with
        
        member this.Init<'ParentType> vm = 
            (inputInternal :> Common.ViewModels.Interfaces.IViewComponent<'ParentType>).Init vm
            refreshPossibleChoice vm
        
        member this.OnDocUpdated<'ParentType> vm = 
            (inputInternal :> Common.ViewModels.Interfaces.IViewComponent<'ParentType>).OnDocUpdated vm
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
        (propDef : PropDefinition<'ParentType, 'Primitive, 'InputPrimitive>) simpleChoices = 
        let docUpdate = docViewModel.GetDocAccessor(propDef.Setter)
        let choicesInput = 
            SimpleChoicesViewModel
                (propDef.Factory, propDef.PropToInput, propDef.Getter, docUpdate, docViewModel.UpdateDoc, simpleChoices, propDef.Name, 0)
        intoPanelViewModel.AddChild(choicesInput)
