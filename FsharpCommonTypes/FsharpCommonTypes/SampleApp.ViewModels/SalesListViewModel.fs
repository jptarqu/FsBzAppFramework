namespace Common.ViewModels

open FsharpCommonTypes
open Common.ViewModels.Interfaces

type SalesListViewModel(dialogService:IDialogService, screenManager:ScreenManager) =
    let screenName = "List Sales People" 
    let screenId = CommandScreen.GenerateId screenName
    
    let createSampleListDoc () =
        let randDays = -(System.Random().Next(1,10))
        let now = ( (System.DateTime.Now.AddDays(float randDays)) ).ToString("yyyy-MM-dd")
//        let dt = ConversionHelpers.tryParseDateWithDefault readOnly.SalesDate  (System.DateTime.Today.AddDays(float 1))
        { 
            Rows =
                [
                    {SaleModelReadOnly.Name=  "Alabama" ; SalesRegion =   BusinessTypes.IdNumber 1; SalesDate =    now} 
                    {Name=  "Alabama" ; SalesRegion =  BusinessTypes.IdNumber 2; SalesDate =    now} 
                    {Name=  "Colorado" ; SalesRegion =  BusinessTypes.IdNumber 3; SalesDate =    now} 
                    {Name=  "California" ; SalesRegion = BusinessTypes.IdNumber 4; SalesDate =    now} 
                ]
            SelectedItem = None
            ApiErrorMessages = Seq.empty
        }
    let createAsyncSampleListDoc () =
        async {
            return createSampleListDoc()
        }
        
    let intialDoc = createSampleListDoc()
    let mutable baseDocViewModel = new DocViewModelBase<SaleListModel>(intialDoc)
    let docGetter () = baseDocViewModel.GetCurrentDoc

    let reloadDoc () =
        async {
            let! initialDoc = createAsyncSampleListDoc() //async in case op takes time (like fetching from server)
            baseDocViewModel.ReloadDoc initialDoc // works but Pivot not refreshing, maybe need to refresh at wpf level
        }
    let onEditSaved editedDoc =
        reloadDoc ()
    let onEditCmd doc =
        async {
            let! resp =  (dialogService.PromptMessage screenId "Test" "Test")
                        |> Async.AwaitIAsyncResult 
                        
            match  doc.SelectedItem  with
            | Some item ->
                let editDoc = Some (SaleModel.ConvertFromReadOnly item)
                let editViewModel = SaleEditViewModel(dialogService, screenManager, editDoc, onEditSaved)
                let! result = editViewModel.Init()
                result
            | None -> ()
            
            return { CommandResult.Errors = Seq.empty; CommandResult.Message = "Thanks!" }
        }
    let addCommands (baseDocVm:DocViewModelBase<SaleListModel>) docGetter =
        let afterSuccess doc cmdResult =
            ()
        let afterFailure cmdResult =
            ()
        let cmdEditDef = { CommandDefinition.CmdName = "Edit"; CommandDefinition.CmdExecuter = onEditCmd; CanRunCheck = (fun d -> d.SelectedItem.IsSome) }
        let cancelCmdDef = CommandDefinition.CancelCmdDefinition

        let cmdEdit = CommandViewModel(cmdEditDef, afterSuccess, afterFailure,  docGetter)
        let cancelCmd = CommandViewModel(cancelCmdDef, afterSuccess, afterFailure,  docGetter)
        baseDocVm.PrimaryCommands.Add cmdEdit
        baseDocVm.SecondaryCommands.Add cancelCmd
    let addChildViewsAsPivot (baseDoc:DocViewModelBase<SaleListModel>) docGetter =
        let loadList doc = doc.Rows
        let onSelectedItem doc item = 
            {doc with SelectedItem = item}
        let pivotSettings = {
            PivotGridDefinition.RowDimensionDefinitions  = 
                [
                   PivotGridDefinition.CreatePivotDimension "Sales Region" "SalesRegion"
                ]
                
            PivotGridDefinition.ColumnDimensionDefinitions = 
                [
                   PivotGridDefinition.CreatePivotDimension "Name" "Name"
                ]
                
            PivotGridDefinition.FactDefinitions = 
                [
                   PivotGridDefinition.CreateFactDimension "Sales Date" "SalesDate" PivotGridDefinition.FactTypes.TextFact
                ]
            }
        let pivotDef = { PivotSettings = pivotSettings; RefreshValFromDoc = loadList; SelectedItemSetter = onSelectedItem; PropName ="Docs" }
        PivotGridViewModel.AddPivotGridViewModel baseDoc (baseDoc.GetRootView())  pivotDef
    let addChildViewsAsTable (baseDoc:DocViewModelBase<SaleListModel>) docGetter =
        let loadList doc = doc.Rows
        let onSelectedItem doc item = 
            {doc with SelectedItem = item}
        
        let def = {  TablePropDefinition.RefreshValFromDoc = loadList; SelectedItemSetter = onSelectedItem; PropName ="Docs" }
        TableViewModel.AddTableViewModel baseDoc (baseDoc.GetRootView())  def
        
   
    member this.Init () = 
        async {
            addChildViewsAsTable baseDocViewModel docGetter
            addCommands baseDocViewModel docGetter
            let! r = reloadDoc()
            let screen = CommandScreen(baseDocViewModel, screenName, screenId)
            screenManager.AddScreen screen // all we need to disaplay because the viewModel has been setup already and ready to go
        }
        
    member this.StartUp () =
        (this.Init ())
        |> Async.Start

        
  