namespace CommonViewEditors.Js

module JsBuilder =
    open CommonViewEditors
    open CommonValidations
    open FsharpCommonTypes.InterfaceTypes
    open FsharpCommonTypes
  
    type JsViewEditorBuilder = 
        {HtmlElementService: JsElementInterfaces.IHtmlElementService; JsDataService: JsElementInterfaces.IJsDataService}
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
                ViewComponents.ExternalChoicesViewComponent(this.HtmlElementService, externalChoicesInputDefinition) :> IViewComponent<'ViewModel> 
            member this.CreateInputWrapperBool< 'ViewModel>  boolInputDefinition =
                ViewComponents.BoolViewComponent(this.HtmlElementService, boolInputDefinition) :> IViewComponent<'ViewModel> 
            member this.CreateInputWrapperText< 'ViewModel>  inputDefinition =
                ViewComponents.TextViewComponent(this.HtmlElementService, inputDefinition) :> IViewComponent<'ViewModel> 
            (*    match inputDefinition with
                | ConstraintedInputDefinition i -> 
                    match i.Constraint with
                    | TextPropertyDefinition te(this.HtmlElementServicextDef -> ViewComponents.TextViewComponent(this.HtmlElementService, i, textDef) :> IViewComponent<'ViewModel> 
                    | IntegerPropertyDefinition intDef -> ViewComponents.IntViewComponent(this.HtmlElementService, i.PropDisplayName, intDef) :> IViewComponent<'ViewModel> 
                    | DecimalPropertyDefinition decDef -> ViewComponents.DecimalViewComponent(this.HtmlElementService, i.PropDisplayName, decDef) :> IViewComponent<'ViewModel> 
                | BooleanInputDefinition b ->
                | ExternalChoicesInputDefinition c -> *)
            member this.CreateContainerWrapper< 'ViewModel> viewContainerDefinition = 
                IViewComponent<'ViewModel>
