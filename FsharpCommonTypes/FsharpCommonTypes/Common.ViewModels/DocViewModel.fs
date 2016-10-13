namespace Common.ViewModels

open CommonViewEditors
open FsharpCommonTypes

type DocViewModel<'ParentType>(intialDoc:'ParentType) =
    let mutable myDoc = intialDoc
    let mutable childViewModels:List<CommonViewEditors.IViewComponent<'ParentType>> = List.empty
    let notifyChange childView newDoc =
        childViewModels |> Seq.filter ((<>) childView)  |> Seq.iter (fun x -> x.OnDocUpdated newDoc) 

    let updateDoc childView newDoc =
        myDoc <-  newDoc
        notifyChange childView myDoc
        ()

    let choicesExecInt (qryExecutor:ExternalChoicesQueryExecutor< 'ParentType, int >) =
        qryExecutor myDoc
        

//    member this.AddTextInput(definition:TextInputDefinition< 'ParentType>) =
//        let docUpdate = (fun newVal ->  
//            definition.DocumentUpdate myDoc newVal
//            )
//        let docPull = definition.DocumentPull
//        let newField = SingleInputViewModel<string, 'ParentType>(docPull, docUpdate,  updateDoc, definition.Constraint, definition.PropDisplayName ) :> CommonViewEditors.IViewComponent<'ParentType>
//        childViewModels <- newField :: childViewModels
//
//    member this.AddExternalChoicesInput(definition:IntExternalChoicesInputDefinition< 'ParentType>) =
//        let docUpdate = (fun newVal -> 
//            definition.DocumentUpdate myDoc newVal
//            )
//        let docPull = definition.DocumentPull
//        let qry = choicesExecInt definition.ExternalChoicesQueryExecutor
//        let newField = ExternalChoicesViewModel<'ParentType>(docPull, docUpdate,  updateDoc, qry,  definition.PropDisplayName, 0 ) :> CommonViewEditors.IViewComponent<'ParentType>
//        childViewModels <- newField :: childViewModels
    
    member this.AddMyDocToFunc funcNeedeingDoc =
        funcNeedeingDoc myDoc 



    member this.GetChildViews() =
        childViewModels |> Seq.rev

    member this.Init() =
        childViewModels |> Seq.iter (fun x -> x.Init myDoc)
    interface Interfaces.IDocViewModel<'ParentType> with 
        member this.GetDocAccessor docUpdate =
            (fun newVal -> 
                docUpdate myDoc newVal
            )
        member this.UpdateDoc childView newDoc =
            updateDoc childView newDoc
        member this.AddChild( newChild:CommonViewEditors.IViewComponent<'ParentType>) =
            childViewModels <- newChild :: childViewModels
        
        
[<CLIMutable>]
type SampleDoc = 
    {Name: BusinessTypes.LongNameType; SalesRegion: BusinessTypes.IdNumberType } 
    with
        static member DefinitionName = { PropDefinition.Name ="Name"; Factory = BusinessTypes.LongName; Setter = (fun doc newValue -> {doc with SampleDoc.Name = newValue }); Getter = (fun doc -> doc.Name )}
        static member DefinitionSalesRegion = { PropDefinition.Name ="Sales Region"; Factory = BusinessTypes.IdNumber; Setter = (fun doc newValue -> {doc with SampleDoc.SalesRegion = newValue }); Getter = (fun doc -> doc.SalesRegion )}
        interface InterfaceTypes.ICanValidate with 
            member this.GetValidationErrors () = 
                [ SampleDoc.DefinitionName.GetValidationErrors(this) ; 
                   SampleDoc.DefinitionSalesRegion.GetValidationErrors(this) ] 
                |> Seq.collect (fun x -> x)
                
type CommandScreen<'ModelType> =
    {DocViewModel: DocViewModel<'ModelType>; CommandToExec:'ModelType->CommandResult; Name: string} //Maybe we don't need the query here only in constructor; QueryForInitialization:unit->'ModelType } 
    
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module CommandScreen =
    let CreateScreen queryForInitialization viewModelBuilder screenName cmdToExec = 
        let initDoc = queryForInitialization ()
        let viewModelDoc = viewModelBuilder initDoc
        { CommandScreen.DocViewModel = viewModelDoc; CommandToExec =cmdToExec; Name=screenName}
        
    
module Sample =
    
    let CreateSampleDoc () =
        let model ={Name= BusinessTypes.LongName "Alabama" ; SalesRegion = BusinessTypes.IdNumber 1 } 
        model
    let BuildViewModels model =
        let simpleChoices (doc:SampleDoc) = 
                                let newRand = System.Random().Next(1,6).ToString()
                                [ {ResultId= 1; ResultLabel= "Test 1";  };
                                {ResultId= 2; ResultLabel= "Test " + newRand ;  } ;
                                {ResultId= 3; ResultLabel= "Test 3 " + doc.Name.ToString();  }  ]
        let doc = DocViewModel(model)
        SingleInputViewModel.AddSingleInputViewModel doc SampleDoc.DefinitionName 
        SimpleChoicesViewModel.AddSimpleChoicesViewModel doc SampleDoc.DefinitionSalesRegion simpleChoices 
        doc
    let Cmd doc =
        { CommandResult.Errors = Seq.empty; CommandResult.Message = "Thanks!" }
    let CreateSample () =
        let screen = CommandScreen.CreateScreen CreateSampleDoc BuildViewModels "Sales Person" Cmd
        screen