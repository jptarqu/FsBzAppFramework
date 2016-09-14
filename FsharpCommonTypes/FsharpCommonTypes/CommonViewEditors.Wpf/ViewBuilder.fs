namespace CommonViewEditors.Wpf

module ViewBuilder =
    open CommonViewEditors
    open CommonValidations
    open FsharpCommonTypes.InterfaceTypes
    open FsharpCommonTypes
  
    type JsViewEditorBuilder = 
        {ElementService: WpfElementInterfaces.IElementService; JsDataService: WpfElementInterfaces.IDataService}
        with 
        interface ISceenBuilderService with 
            member this.CreateExternalCommand<'ViewModel, 'CmdType>  externalViewCommandDefinition =
                let exeCmd = fun vm ->
                                let cmdParam = externalViewCommandDefinition.CmdTransformer vm 
                                this.JsDataService.CallExternalCommand  externalViewCommandDefinition.CmdAddress cmdParam
                { new IViewCommand<'ViewModel> with
                    member this.Name = externalViewCommandDefinition.CmdName
                    member this.Execute vm = exeCmd vm
                    member this.CanExecute vm = true
                    }
            member this.CreateInputWrapperChoices< 'ViewModel, 'Result>  externalChoicesInputDefinition =
                ViewComponents.ExternalChoicesViewComponent(this.ElementService, externalChoicesInputDefinition) :> IViewComponent<'ViewModel> 
            member this.CreateInputWrapperBool< 'ViewModel>  boolInputDefinition =
                ViewComponents.BoolViewComponent(this.ElementService, boolInputDefinition) :> IViewComponent<'ViewModel> 
            member this.CreateInputWrapperText< 'ViewModel>  inputDefinition =
                ViewComponents.TextViewComponent(this.ElementService, inputDefinition) :> IViewComponent<'ViewModel> 
            (*    match inputDefinition with
                | ConstraintedInputDefinition i -> 
                    match i.Constraint with
                    | TextPropertyDefinition te(this.ElementServicextDef -> ViewComponents.TextViewComponent(this.ElementService, i, textDef) :> IViewComponent<'ViewModel> 
                    | IntegerPropertyDefinition intDef -> ViewComponents.IntViewComponent(this.ElementService, i.PropDisplayName, intDef) :> IViewComponent<'ViewModel> 
                    | DecimalPropertyDefinition decDef -> ViewComponents.DecimalViewComponent(this.ElementService, i.PropDisplayName, decDef) :> IViewComponent<'ViewModel> 
                | BooleanInputDefinition b ->
                | ExternalChoicesInputDefinition c -> *)
            member this.CreateContainerWrapper< 'ViewModel> viewContainerDefinition = 
                ViewComponents IViewComponent<'ViewModel>
