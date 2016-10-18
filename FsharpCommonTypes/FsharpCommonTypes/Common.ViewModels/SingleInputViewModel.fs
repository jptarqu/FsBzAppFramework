namespace Common.ViewModels

open FSharp.ViewModule
open FsharpCommonTypes

type SingleInputViewModel<'PrimitiveType, 'ParentType when 'PrimitiveType : equality>(propFactory : PropFactoryMethod<'PrimitiveType>, refreshValFromDoc : 'ParentType -> BzProp<'PrimitiveType>, refreshDocFromVal : BzProp<'PrimitiveType> -> 'ParentType, // allow create new doc by sending the newly BzProp<'PrimitiveType>
                                                                                                                                                                                                                                                             pushUpdatedDoc : CommonViewEditors.IViewComponent<'ParentType> -> 'ParentType -> unit, propName : string, defaultValue : 'PrimitiveType, uiHint : string) as self = 
    inherit ViewModelBase()
    let getStrErrors = BusinessTypes.GetStrErrors propFactory
    let txtValue = self.Factory.Backing(<@ self.Value @>, defaultValue, getStrErrors)
    
    let alertParentOfDocChg newVal = 
        let newDoc = refreshDocFromVal newVal
        pushUpdatedDoc self newDoc
    
    member self.Value 
        with get () = txtValue.Value
        and set value = 
            if (value <> txtValue.Value) then 
                txtValue.Value <- value
                let newPropState = propFactory value
                alertParentOfDocChg newPropState // always send to doc, even if invalid state?
    
    member self.PropName = propName
    
    interface CommonViewEditors.IViewComponent<'ParentType> with
        member this.Init<'ParentType> vm = self.Value <- BusinessTypes.ToPrimitive(refreshValFromDoc vm)
        member this.OnDocUpdated<'ParentType> vm = 
            self.Value <- BusinessTypes.ToPrimitive(refreshValFromDoc vm)
            ()
    
    interface Interfaces.IViewComponent with
        member this.Label = propName
        member this.UiHint = uiHint

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module SingleInputViewModel = 
    module UIHints = 
        let SingleTextInput = "SingleInput"
        let DateInput = "DateInput"
        let DateTimeInput = "DateTimeInput"
    
    let AddSingleInputViewModel uiHint defVal (docViewModel : #Interfaces.IDocViewModel<'ParentType>) 
        (intoPanelViewModel : #Interfaces.IPanelViewModel<'ParentType>) 
        (propDef : PropDefinition<'ParentType, 'Primitive>) = 
        let docUpdateFunc = docViewModel.GetDocAccessor(propDef.Setter)
        let txtInput = 
            SingleInputViewModel
                (propDef.Factory, propDef.Getter, docUpdateFunc, docViewModel.UpdateDoc, propDef.Name, defVal, uiHint)
        intoPanelViewModel.AddChild(txtInput)
    
    let AddTextInputViewModel docViewModel intoPanelViewModel propDef = 
        AddSingleInputViewModel UIHints.SingleTextInput "" docViewModel intoPanelViewModel propDef
    let AddDateInputViewModel docViewModel intoPanelViewModel propDef = 
        AddSingleInputViewModel UIHints.DateInput (None) docViewModel intoPanelViewModel propDef
    let AddDateTimeInputViewModel docViewModel intoPanelViewModel propDef = 
        AddSingleInputViewModel UIHints.DateTimeInput System.DateTime.Now docViewModel intoPanelViewModel propDef
