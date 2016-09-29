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
        

    member this.AddTextInput(definition:TextInputDefinition< 'ParentType>) =
        let docUpdate = (fun newVal ->  
            definition.DocumentUpdate myDoc newVal
            )
        let docPull = definition.DocumentPull
        let newField = SingleInputViewModel<string, 'ParentType>(docPull, docUpdate,  updateDoc, definition.Constraint, definition.PropDisplayName ) :> CommonViewEditors.IViewComponent<'ParentType>
        childViewModels <- newField :: childViewModels

    member this.AddExternalChoicesInput(definition:IntExternalChoicesInputDefinition< 'ParentType>) =
        let docUpdate = (fun newVal -> 
            definition.DocumentUpdate myDoc newVal
            )
        let docPull = definition.DocumentPull
        let qry = choicesExecInt definition.ExternalChoicesQueryExecutor
        let newField = ExternalChoicesViewModel<'ParentType>(docPull, docUpdate,  updateDoc, qry,  definition.PropDisplayName, 0 ) :> CommonViewEditors.IViewComponent<'ParentType>
        childViewModels <- newField :: childViewModels

    member this.GetChildViews() =
        childViewModels |> Seq.rev

    member this.Init() =
        childViewModels |> Seq.iter (fun x -> x.Init myDoc)
        

[<CLIMutable>]
type SampleDoc = 
    {Name: BusinessTypes.ShortName; SalesRegion: int } 
    with
        interface InterfaceTypes.ICanValidate with 
            member this.GetValidationErrors () = 
                [ this.Name.GetValidationErrors() ] |> InterfaceTypes.CollectAllError

module Sample =
    let CreateSampleDoc () =
        let model ={Name= (BusinessTypes.NewShortName "Name" "Alabama" ); SalesRegion = 0 } 
        let doc = DocViewModel(model)
        let txtInput = {
                        TextInputDefinition.PropDisplayName = "Name"; 
                        DocumentPull = (fun x -> x.Name.Value) ; 
                        DocumentUpdate = (fun doc x -> {doc with Name = doc.Name.WithValue x });
                        Constraint = model.Name.GetDefinition() 
                        }
        doc.AddTextInput(txtInput)

        let choicesInput = {
                        IntExternalChoicesInputDefinition.PropDisplayName = "Sales Region"; 
                        DocumentPull = (fun x -> x.SalesRegion) ; 
                        DocumentUpdate = (fun doc x -> {doc with SalesRegion =  x });
                        ExternalChoicesQueryExecutor = (fun doc filterStr -> 
                                                            let results:list<ExternalChoicesQueryResult<int>> = 
                                                                            [ {ResultId= "1"; ResultLabel= "Test 1"; Content= 1  };
                                                                            {ResultId= "2"; ResultLabel= "Test 2"; Content= 2  } ;
                                                                            {ResultId= "3"; ResultLabel= "Test 3"; Content= 3  }  ]
                                                            results |> Seq.filter (fun x -> x.ResultId = filterStr) 
                                                         )
                        }
        doc.AddExternalChoicesInput(choicesInput)
        doc