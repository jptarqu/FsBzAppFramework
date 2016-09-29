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

    member this.AddVhild( newChild:CommonViewEditors.IViewComponent<'ParentType>) =
        childViewModels <- newChild :: childViewModels

    member this.GetChildViews() =
        childViewModels |> Seq.rev

    member this.Init() =
        childViewModels |> Seq.iter (fun x -> x.Init myDoc)
        

[<CLIMutable>]
type SampleDoc = 
    {Name: BusinessTypes.LongNameType; SalesRegion: BusinessTypes.IdNumberType } 
    with
        interface InterfaceTypes.ICanValidate with 
            member this.GetValidationErrors () = 
                Seq.empty //TODO check for invlaid states //[ this.Name.GetValidationErrors() ] |> InterfaceTypes.CollectAllError

module Sample =
    let CreateSampleDoc () =
        let model ={Name= BusinessTypes.LongName "Alabama" ; SalesRegion = BusinessTypes.IdNumber 0 } 
        let doc = DocViewModel(model)
        let docUpdateName = doc.GetDocAccessor((fun doc x -> {doc with Name = x }))
        let txtInput = SingleInputViewModel(BusinessTypes.LongName, (fun x -> x.Name), docUpdateName,  doc.UpdateDoc, "Name", "")

        doc.AddVhild(txtInput)
        
        let docUpdateSalesRegion = doc.GetDocAccessor((fun doc x -> {doc with SalesRegion = x })) // diff SalesRegion
        let qryExec = doc.AddMyDocToFunc (fun doc filterStr -> 
                                                            let results:list<SimpleExternalChoicesQueryResult<int>> = 
                                                                            [ {ResultId= 1; ResultLabel= "Test 1";  };
                                                                            {ResultId= 2; ResultLabel= "Test 2";  } ;
                                                                            {ResultId= 3; ResultLabel= "Test 3";  }  ]
                                                            results |> Seq.filter (fun x -> x.ResultId.ToString() = filterStr) 
                                                         ) 
        let lblLkp = (fun index -> "DUMMY Test 2") 
        let choicesInput = ExternalChoicesViewModel(BusinessTypes.IdNumber, (fun x -> x.SalesRegion), docUpdateSalesRegion,  doc.UpdateDoc, qryExec, lblLkp,  "SalesRegion", 0)
       
        doc.AddVhild(choicesInput)
        doc