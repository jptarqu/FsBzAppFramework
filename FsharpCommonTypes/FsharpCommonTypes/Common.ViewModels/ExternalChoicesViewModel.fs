namespace Common.ViewModels

open FsharpCommonTypes
open FSharp.ViewModule
open Common.ViewModels.Interfaces

type ExternalChoicesViewModel<'InputPrimitive, 'PrimitiveType, 'ParentType when 'PrimitiveType : equality  and 'InputPrimitive : equality>(
        propFactory : PropFactoryMethod<'InputPrimitive, 'PrimitiveType>, refreshValFromDoc : 'ParentType -> 'InputPrimitive, 
        refreshDocFromVal : BzProp<'PrimitiveType> -> 'ParentType, // allow create new doc by sending the newly BzProp<'PrimitiveType>
        pushUpdatedDoc : Common.ViewModels.Interfaces.IViewComponent<'ParentType> -> 'ParentType -> unit, 
        queryExecutor : string -> seq<SimpleExternalChoicesQueryResult<'PrimitiveType>>, labelLkup : 'InputPrimitive -> string, 
        propName : string, defaultValue : 'InputPrimitive) as self = 
    inherit ViewModelBase()
    //    let mutable txtValue = defaultValue
    let mutable currErrors : seq<PropertyError> = Seq.empty
    let getStrErrors = BusinessTypes.GetStrErrors propFactory
    let txtValue = self.Factory.Backing(<@ self.Value @>, defaultValue, getStrErrors)
    let txtLabel = self.Factory.Backing(<@ self.ResultLabel @>, "")
    let isValueValid = currErrors |> Seq.isEmpty
    
    let alertParentOfDocChg newVal = 
        let newDoc = refreshDocFromVal newVal
        pushUpdatedDoc self newDoc
    
    let updateInternalPrimitive newVal =
        if (newVal <> txtValue.Value) then 
                txtValue.Value <- newVal

    member self.Value 
        with get () = txtValue.Value
        and set value = 
            if (value <> txtValue.Value) then 
                updateInternalPrimitive value
                let newPropState = propFactory value
                alertParentOfDocChg newPropState
    
    member self.ResultLabel 
        with get () = txtLabel.Value
        and set value = 
            if (value <> txtLabel.Value) then txtLabel.Value <- value
    
    member self.PropName = propName
    
    interface Common.ViewModels.Interfaces.IViewComponent<'ParentType> with
        
        member this.Init<'ParentType> vm = 
            updateInternalPrimitive (refreshValFromDoc vm)
            self.ResultLabel <- labelLkup txtValue.Value
        
        member this.OnDocUpdated<'ParentType> vm = 
            updateInternalPrimitive (refreshValFromDoc vm)
            self.ResultLabel <- labelLkup txtValue.Value
    
    interface Interfaces.IViewComponent with
        member this.Label = propName
        member this.UiHint = "ExternalChoices"
    
    interface IExternalChoicesQry<'PrimitiveType> with
        member this.QueryExecutor filterStr = queryExecutor filterStr

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ExternalChoicesViewModel = 
    module UIHints = 
        let ExternalChoices = "ExternalChoices"