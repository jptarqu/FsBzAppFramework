namespace Common.ViewModels

open CommonViewEditors
open FsharpCommonTypes
   
[<CLIMutable>]
type SampleDoc = 
    {Name: BusinessTypes.LongNameType; SalesRegion: BusinessTypes.IdNumberType } 
    with
        static member DefinitionName = 
            { PropDefinition.Name ="Name"; Factory = BusinessTypes.LongName; Setter = (fun d v -> {d with SampleDoc.Name = v }); Getter = (fun d -> d.Name )}
        static member DefinitionSalesRegion = 
            { PropDefinition.Name ="Sales Region"; Factory = BusinessTypes.IdNumber; Setter = (fun d v -> {d with SampleDoc.SalesRegion = v }); Getter = (fun d -> d.SalesRegion )}
        interface InterfaceTypes.ICanValidate with 
            member this.GetValidationErrors () = 
                let isObjValid () = 
                    if this.Name = BusinessTypes.LongName "Looper" then  
                        seq [ { PropertyError.ErrorCode ="YY"; Description ="TestError"; PropertyName=""; } ] 
                    else 
                        Seq.empty
                [ SampleDoc.DefinitionName.GetValidationErrors(this) ; 
                   SampleDoc.DefinitionSalesRegion.GetValidationErrors(this);
                   isObjValid(); ] 
                |> Seq.collect (fun x -> x)

module Sample =
    let CreateSampleDoc () =
        let model ={Name= BusinessTypes.LongName "Alabama" ; SalesRegion = BusinessTypes.IdNumber 1 } 
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
        SingleInputViewModel.AddSingleInputViewModel doc (doc.GetRootView()) SampleDoc.DefinitionName 
        SimpleChoicesViewModel.AddSimpleChoicesViewModel doc (doc.GetRootView()) SampleDoc.DefinitionSalesRegion simpleChoices 
        doc
    let CreateSample (screenManager:ScreenManager) =
        let afterSuccess doc cmdResult =
            screenManager.RemoveScreen screen
            ()
            
        let afterFailure doc cmdResult =
            ()
        let screen = CommandScreen.CreateScreen afterSuccess CreateSampleDoc (BuildViewModels afterSuccess) "Sales Person" 
        screen