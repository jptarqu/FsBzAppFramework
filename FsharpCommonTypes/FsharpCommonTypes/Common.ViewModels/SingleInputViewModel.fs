namespace Common.ViewModels

open FSharp.ViewModule
open FsharpCommonTypes

type SingleInputViewModel<'InputPrimitive, 'PrimitiveType, 'ParentType when 'PrimitiveType : equality  and 'InputPrimitive : equality>(
    propFactory : PropFactoryMethod<'InputPrimitive, 'PrimitiveType>, 
    propToInput: BzProp<'PrimitiveType>->'InputPrimitive, 
    refreshValFromDoc : 'ParentType -> BzProp<'PrimitiveType>, 
    refreshDocFromVal : BzProp<'PrimitiveType> -> 'ParentType, // allow create new doc by sending the newly BzProp<'PrimitiveType>
                                                                                                                                                                                                                                                             pushUpdatedDoc : Common.ViewModels.Interfaces.IViewComponent<'ParentType> -> 'ParentType -> unit, propName : string, defaultValue : 'InputPrimitive, mask: string, uiHint : string) as self = 
    inherit ViewModelBase()
    let getStrErrors = BusinessTypes.GetStrErrors propFactory
    let txtValue = self.Factory.Backing(<@ self.Value @>, defaultValue, getStrErrors)
    
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
                alertParentOfDocChg newPropState // always send to doc, even if invalid state?
    
    member self.PropName = propName
    member self.Mask = mask
    
    interface Common.ViewModels.Interfaces.IViewComponent<'ParentType> with
        member this.Init<'ParentType> vm = updateInternalPrimitive (propToInput (refreshValFromDoc vm)) // go directly to field because we do not need to alert the doc model of the change
        member this.OnDocUpdated<'ParentType> vm = 
            updateInternalPrimitive  (propToInput (refreshValFromDoc vm))
            ()
    
    interface Interfaces.IViewComponent with
        member this.Label = propName
        member this.UiHint = uiHint

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module SingleInputViewModel = 
    module UIHints = 
        let SingleTextInput = "SingleInput"
        let ReadOnlyText = "ReadOnlyText"
        let DateInput = "DateInput"
        let DateTimeInput = "DateTimeInput"
        let IntInput = "IntInput"
    
    let AddSingleInputViewModel mask uiHint defVal (docViewModel : #Interfaces.IDocViewModel<'ParentType>) 
        (intoPanelViewModel : #Interfaces.IPanelViewModel<'ParentType>) 
        (propDef : PropDefinition<'ParentType, 'Primitive, 'InputPrimitive>) = 
        let docUpdateFunc = docViewModel.GetDocAccessor(propDef.Setter)
        let txtInput = 
            SingleInputViewModel
                (propDef.Factory, propDef.PropToInput, propDef.Getter, docUpdateFunc, docViewModel.UpdateDoc, propDef.Name, defVal, mask, uiHint)
        intoPanelViewModel.AddChild(txtInput)
    
    let AddTextInputViewModel docViewModel intoPanelViewModel propDef = 
        AddSingleInputViewModel "" UIHints.SingleTextInput "" docViewModel intoPanelViewModel propDef

    let AddReadOnlyTextViewModel docViewModel intoPanelViewModel propDef = 
        AddSingleInputViewModel "" UIHints.ReadOnlyText "" docViewModel intoPanelViewModel propDef

    let AddIntInputViewModel docViewModel intoPanelViewModel propDef = 
        AddSingleInputViewModel "999999990" UIHints.IntInput 0 docViewModel intoPanelViewModel propDef

    let AddOptDateInputViewModel docViewModel intoPanelViewModel propDef = 
        AddSingleInputViewModel "" UIHints.DateInput (None) docViewModel intoPanelViewModel propDef

    let AddDateInputViewModel docViewModel intoPanelViewModel propDef = 
        AddSingleInputViewModel "" UIHints.DateInput System.DateTime.Now docViewModel intoPanelViewModel propDef

    let AddDateTimeInputViewModel docViewModel intoPanelViewModel propDef = 
        AddSingleInputViewModel "" UIHints.DateTimeInput System.DateTime.Now docViewModel intoPanelViewModel propDef
