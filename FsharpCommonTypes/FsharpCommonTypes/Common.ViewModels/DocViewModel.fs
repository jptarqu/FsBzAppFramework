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
    member this.GetDocAccessor docUpdate =
        (fun newVal -> 
            docUpdate myDoc newVal
        )
    member this.AddMyDocToFunc funcNeedeingDoc =
        funcNeedeingDoc myDoc 

    member this.UpdateDoc =
        updateDoc 

    member this.AddChild( newChild:CommonViewEditors.IViewComponent<'ParentType>) =
        childViewModels <- newChild :: childViewModels

    member this.GetChildViews() =
        childViewModels |> Seq.rev

    member this.Init() =
        childViewModels |> Seq.iter (fun x -> x.Init myDoc)
        
        
        
[<CLIMutable>]
type SampleDoc = 
    {Name: BusinessTypes.LongNameType; SalesRegion: BusinessTypes.IdNumberType } 
    with
        static member DefinitionName = { PropDefinition.Name ="Name"; Factory = BusinessTypes.LongName; Setter = (fun doc newValue -> {doc with SampleDoc.Name = newValue }); Getter = (fun doc -> doc.Name )}
        static member DefinitionSalesRegion = { PropDefinition.Name ="Sales Region"; Factory = BusinessTypes.IdNumber; Setter = (fun doc newValue -> {doc with SampleDoc.SalesRegion = newValue }); Getter = (fun doc -> doc.SalesRegion )}
            
//        static member Definitions = 
//            {
//                Name = { PropDefinition.Name ="Name"; Factory = BusinessTypes.LongName; Setter = (fun doc newValue -> {doc with SampleDoc.Name = newValue }); Getter = (fun doc -> doc.Name )}
//                SalesRegion = { PropDefinition.Name ="Sales Region"; Factory = BusinessTypes.IdNumber; Setter = (fun doc newValue -> {doc with SampleDoc.SalesRegion = newValue }); Getter = (fun doc -> doc.SalesRegion )}
//            }
//        member this.Definitions = SampleDoc.Definitions
//        static member PopNameSalesRegion = "Sales Region"
//        static member SetSalesRegion doc newValue = {doc with SampleDoc.SalesRegion = newValue }
        interface InterfaceTypes.ICanValidate with 
            member this.GetValidationErrors () = 
                [ SampleDoc.DefinitionName.GetValidationErrors(this) ; 
                   SampleDoc.DefinitionSalesRegion.GetValidationErrors(this) ] 
                |> Seq.collect (fun x -> x)
                

//
//type SampleDocDefinition =
//    {PropertyDefintions:PropDefinition<LanguagePrimitives.> }

module Sample =
    let singleInputViewModel docViewModel (propDef:PropDefinition)  =
        let docUpdateName = docViewModel.GetDocAccessor(propDef.Setter)
        SingleInputViewModel(propDef.Factory, propDef.Getter, docUpdateName,  docViewModel.UpdateDoc, propDef.Name, "")
    
    let CreateSampleDoc () =
        let model ={Name= BusinessTypes.LongName "Alabama" ; SalesRegion = BusinessTypes.IdNumber 1 } 
        let doc = DocViewModel(model)
//        let docUpdateName = doc.GetDocAccessor(SampleDoc.SetName)
        let txtInput = singleInputViewModel doc SampleDoc.DefinitionName 

        doc.AddChild(txtInput)
       
        let docUpdateSalesRegion = doc.GetDocAccessor(SampleDoc.SetSalesRegion) // diff SalesRegion
        let qryExec = doc.AddMyDocToFunc (fun doc filterStr -> 
                                                            let results:list<SimpleExternalChoicesQueryResult<int>> = 
                                                                            [ {ResultId= 1; ResultLabel= "Test 1";  };
                                                                            {ResultId= 2; ResultLabel= "Test 2";  } ;
                                                                            {ResultId= 3; ResultLabel= "Test 3";  }  ]
                                                            results |> Seq.filter (fun x -> x.ResultId.ToString() = filterStr) 
                                                         ) 
        let lblLkp = (fun index -> 
                "DUMMY Test 2"
                ) 
        let simpleChoices =  [ {ResultId= 1; ResultLabel= "Test 1";  };
                                {ResultId= 2; ResultLabel= "Test 2";  } ;
                                {ResultId= 3; ResultLabel= "Test 3";  }  ]
        let choicesInput = SimpleChoicesViewModel(BusinessTypes.IdNumber, (fun x -> x.SalesRegion), docUpdateSalesRegion,  doc.UpdateDoc, simpleChoices,   "SalesRegion", 0)
       
        doc.AddChild(choicesInput)
        doc