namespace Common.ViewModels

open FsharpCommonTypes
open Common.ViewModels.Interfaces
open System.Threading;
open System.Threading.Tasks;

    //type dummyDialogService() =
    //    interface IDialogService with
    //        member this.PromptMessage _ msg _ =
    //            async {   
    //                "jfr".ToString() |> ignore
    //            }
    //            |> Async.

type SaleEditViewModel(dialogService:IDialogService, screenManager:ScreenManager, 
                            docToEdit:SaleModel option, onSaveFinished:SaleModel->Async<unit>) =
    let screenName = "Sales Person" 
    let screenId = CommandScreen.GenerateId screenName
    let createModel () =
       SaleModel.Empty
    let createModelAsync () =
        async {
            return createModel()
        }
    let doNothingCmd  doc =
        async {
            return { CommandResult.Errors = Seq.empty; CommandResult.Message = "Thanks!" }
        }
    let onSaved doc =
        async {
            let! resp =  (dialogService.PromptMessage screenId "Test" "Test")
                        |> Async.AwaitIAsyncResult 
            screenManager.RemoveScreen screenId
            let! result = onSaveFinished doc
            return { CommandResult.Errors = Seq.empty; CommandResult.Message = "Thanks!" }
        }
    let addCommands (baseDocVm:DocViewModelBase<SaleModel>) docGetter =
        let afterSuccess doc cmdResult =
            ()
        let afterFailure cmdResult =
            ()
        let cmdEditDef = { CommandDefinition.CmdName = "Save"; CommandDefinition.CmdExecuter = onSaved; CanRunCheck = BusinessTypes.IsValidModel }
        let cancelCmdDef = CommandDefinition.CancelCmdDefinition
        let cmdEdit = CommandViewModel(cmdEditDef, afterSuccess, afterFailure,  docGetter)
        let cancelCmd = CommandViewModel(cancelCmdDef, afterSuccess, afterFailure,  docGetter)
        baseDocVm.PrimaryCommands.Add cmdEdit
        baseDocVm.SecondaryCommands.Add cancelCmd
    let addChildViews (baseDoc:DocViewModelBase<SaleModel>) docGetter =
        let simpleChoices (doc:SaleModel) = 
            let newRand = System.Random().Next(1,6).ToString()
            [ {ResultId= 1; ResultLabel= "Test 1";  };
            {ResultId= 2; ResultLabel= "Test " ;  } ;
            {ResultId= 3; ResultLabel= "Test 3 " + doc.Name.ToString();  }  ]
        
        SingleInputViewModel.AddTextInputViewModel baseDoc (baseDoc.GetRootView()) SaleModel.DefinitionName 
        SingleInputViewModel.AddMaskedTextInputViewModel baseDoc (baseDoc.GetRootView()) SaleModel.DefinitionQuantityId "999999990"
        SingleInputViewModel.AddDateInputViewModel baseDoc (baseDoc.GetRootView()) SaleModel.DefinitionSalesDate 
        SimpleChoicesViewModel.AddSimpleChoicesViewModel baseDoc (baseDoc.GetRootView()) SaleModel.DefinitionSalesRegion simpleChoices 
        
    // TODO the rest of the class seems repeatable... try DRYing
    let initialDoc = match docToEdit with
                    | Some d -> d
                    | None -> createModel()
    let mutable baseDocViewModel = new DocViewModelBase<SaleModel>(initialDoc)
    let docGetter () = baseDocViewModel.GetCurrentDoc
   
    member this.Init () = 
        async {

            addChildViews baseDocViewModel docGetter
            addCommands baseDocViewModel docGetter
            baseDocViewModel.ReloadDoc initialDoc //maybe this makes more sense than Init
            let screen = CommandScreen(baseDocViewModel, screenName, screenId)
            screenManager.AddScreen screen // all we need to disaplay because the viewModel has been setup already and ready to go
        }
    member this.InitNoScreen () = 
        addChildViews baseDocViewModel docGetter
        addCommands baseDocViewModel docGetter
        baseDocViewModel.ReloadDoc initialDoc //maybe this makes more sense than Init
        baseDocViewModel

module SalesHelpers = 
    let doNothing _ =
        async {
            "jfr".ToString() |> ignore
        }
    let doNothingFunc =
        doNothing 
    
    let SampleModelEmpty = Some SaleModel.Empty
    let SampleViewModel dialogService  screenManager = SaleEditViewModel(dialogService, screenManager, SampleModelEmpty, doNothing )