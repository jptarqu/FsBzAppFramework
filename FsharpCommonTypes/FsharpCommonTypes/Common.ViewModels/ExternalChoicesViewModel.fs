namespace Common.ViewModels

open FsharpCommonTypes
open FSharp.ViewModule
open Common.ViewModels.Interfaces

type ExternalChoicesViewModel<'InputPrimitive, 'PrimitiveType, 'ParentType when 'PrimitiveType : equality  and 'InputPrimitive : equality>(
        propFactory : PropFactoryMethod<'InputPrimitive, 'PrimitiveType>, 
        propToInput: BzProp<'PrimitiveType>->'InputPrimitive, 
        refreshValFromDoc : 'ParentType -> BzProp<'PrimitiveType>, 
        refreshDocFromVal : BzProp<'PrimitiveType> -> 'ParentType,
        pushUpdatedDoc : Common.ViewModels.Interfaces.IViewComponent<'ParentType> -> 'ParentType -> unit, 
        queryExecutor : string -> seq<SimpleExternalChoicesQueryResult<'PrimitiveType>>, labelLkup : 'InputPrimitive -> string, 
        propName : string, defaultValue : 'InputPrimitive) as self = 
    inherit ViewModelBase()
    let inputInternal =
            SingleInputViewModel
                (propFactory, propToInput, refreshValFromDoc, refreshDocFromVal, pushUpdatedDoc, propName, defaultValue, "", "")

    let txtLabel = self.Factory.Backing(<@ self.ResultLabel @>, "")
//    let isValueValid = currErrors |> Seq.isEmpty
    

    member self.Value 
        with get () = inputInternal.Value 
        and set value = 
            inputInternal.Value <- value
    
    member self.ResultLabel 
        with get () = txtLabel.Value
        and set value = 
            if (value <> txtLabel.Value) then txtLabel.Value <- value
    
    member self.PropName = propName
    
    interface Common.ViewModels.Interfaces.IViewComponent<'ParentType> with
        
        member this.Init<'ParentType> vm = 
            (inputInternal :> Common.ViewModels.Interfaces.IViewComponent<'ParentType>).Init vm
            self.ResultLabel <- labelLkup inputInternal.Value
        
        member this.OnDocUpdated<'ParentType> vm = 
            (inputInternal :> Common.ViewModels.Interfaces.IViewComponent<'ParentType>).OnDocUpdated vm
            self.ResultLabel <- labelLkup inputInternal.Value
    
    interface Interfaces.IViewComponent with
        member this.Label = propName
        member this.UiHint = "ExternalChoices"
    
    interface IExternalChoicesQry<'PrimitiveType> with
        member this.QueryExecutor filterStr = queryExecutor filterStr

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ExternalChoicesViewModel = 
    module UIHints = 
        let ExternalChoices = "ExternalChoices"