namespace Common.ViewModels

open CommonViewEditors
open FsharpCommonTypes
   
[<CLIMutable>]
type SampleDoc = 
    {Name: BusinessTypes.LongNameType; SalesRegion: BusinessTypes.IdNumberType; SalesDate : BusinessTypes.PastDateTimeType} 
    with
        static member DefinitionName = 
            { PropDefinition.Name ="Name"; Factory = BusinessTypes.LongName; Setter = (fun d v -> {d with SampleDoc.Name = v }); Getter = (fun d -> d.Name )}
        static member DefinitionSalesRegion = 
            { PropDefinition.Name ="Sales Region"; Factory = BusinessTypes.IdNumber; Setter = (fun d v -> {d with SampleDoc.SalesRegion = v }); Getter = (fun d -> d.SalesRegion )}  
        static member DefinitionSalesDate = 
            { PropDefinition.Name ="Sales Date"; Factory = BusinessTypes.PastDateTime; Setter = (fun d v -> {d with SampleDoc.SalesDate = v }); Getter = (fun d -> d.SalesDate )}
        interface InterfaceTypes.ICanValidate with 
            member this.GetValidationErrors () = 
                let isObjValid () = 
                    if this.Name = BusinessTypes.LongName "Looper" then  
                        seq [ { PropertyError.ErrorCode ="YY"; Description ="TestError"; PropertyName=""; } ] 
                    else 
                        Seq.empty
                [ SampleDoc.DefinitionName.GetValidationErrors(this) ; 
                   SampleDoc.DefinitionSalesRegion.GetValidationErrors(this);
                   SampleDoc.DefinitionSalesDate.GetValidationErrors(this);
                   isObjValid(); ] 
                |> Seq.collect (fun x -> x)

module Sample =
    let CreateSampleDoc () =
        let now = Some System.DateTime.Now
        let model ={Name= BusinessTypes.LongName "Alabama" ; SalesRegion = BusinessTypes.IdNumber 1; SalesDate =  BusinessTypes.PastDateTime now} 
        model
    let SampleCmd doc =
        async {
            return { CommandResult.Errors = Seq.empty; CommandResult.Message = "Thanks!" }
        }
    let BuildViewModels afterSuccess model  =
        let simpleChoices (doc:SampleDoc) = 
                                let newRand = System.Random().Next(1,6).ToString()
                                [ {ResultId= 1; ResultLabel= "Test 1";  };
                                {ResultId= 2; ResultLabel= "Test " + newRand ;  } ;
                                {ResultId= 3; ResultLabel= "Test 3 " + doc.Name.ToString();  }  ]
        let cmd = { CommandDefinition.CmdName = "Save"; CommandDefinition.CmdExecuter = SampleCmd}
        let doc = DocViewModel(model, cmd, afterSuccess)
        SingleInputViewModel.AddTextInputViewModel doc (doc.GetRootView()) SampleDoc.DefinitionName 
        SingleInputViewModel.AddDateInputViewModel doc (doc.GetRootView()) SampleDoc.DefinitionSalesDate 
        SimpleChoicesViewModel.AddSimpleChoicesViewModel doc (doc.GetRootView()) SampleDoc.DefinitionSalesRegion simpleChoices 
        doc
    let CreateSample (screenManager:ScreenManager) =
        let screenName = "Sales Person" 
        let screenId = CommandScreen.GenerateId screenName
        let afterSuccess doc cmdResult =
            screenManager.RemoveScreen screenId
            ()
            
        let afterFailure doc cmdResult =
            ()
        let screen = CommandScreen.CreateScreen CreateSampleDoc (BuildViewModels afterSuccess) screenName screenId
        screenManager.AddScreen screen // TODO should this be here or boorstrapper??
        screen