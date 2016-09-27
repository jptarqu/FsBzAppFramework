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
        notifyChange childView newDoc
        ()

    let choicesExecInt (qryExecutor:ExternalChoicesQueryExecutor< 'ParentType, int >) =
        qryExecutor myDoc
        

    member this.AddTextInput(definition:TextInputDefinition< 'ParentType>) =
        let docUpdate = definition.DocumentUpdate myDoc
        let docPull = definition.DocumentPull
        let newField = SingleInputViewModel<string, 'ParentType>(docPull, docUpdate,  updateDoc, definition.Constraint, definition.PropDisplayName ) :> CommonViewEditors.IViewComponent<'ParentType>
        childViewModels <- newField :: childViewModels

    member this.AddExternalChoicesInput(definition:IntExternalChoicesInputDefinition< 'ParentType>) =
        let docUpdate = definition.DocumentUpdate myDoc
        let docPull = definition.DocumentPull
        let qry = choicesExecInt definition.ExternalChoicesQueryExecutor
        let newField = ExternalChoicesViewModel<'ParentType>(docPull, docUpdate,  updateDoc, qry,  definition.PropDisplayName, 0 ) :> CommonViewEditors.IViewComponent<'ParentType>
        childViewModels <- newField :: childViewModels

    member this.GetChildViews() =
        childViewModels |> Seq.rev

[<CLIMutable>]
type SampleDoc = 
    {Name: BusinessTypes.ShortName; SalesRegion: int } 
    with
        interface InterfaceTypes.ICanValidate with 
            member this.GetValidationErrors () = 
                [ this.Name.GetValidationErrors() ] |> InterfaceTypes.CollectAllError

module Sample =
    let CreateSampleDoc () =
        let model ={Name= (BusinessTypes.NewShortName "Alabama" "Name"); SalesRegion = 0 } 
        let doc = DocViewModel(model)
        let txtInput = {
                        TextInputDefinition.PropDisplayName = "Name"; 
                        DocumentPull = (fun x -> x.Name.Value) ; 
                        DocumentUpdate = (fun doc x -> {doc with Name = doc.Name.WithValue x });
                        Constraint = model.Name.GetDefinition() 
                        }
        doc.AddTextInput(txtInput)

        let choicesInput = {
                        IntExternalChoicesInputDefinition.PropDisplayName = "Name"; 
                        DocumentPull = (fun x -> x.SalesRegion) ; 
                        DocumentUpdate = (fun doc x -> {doc with SalesRegion =  x });
                        ExternalChoicesQueryExecutor = (fun doc filterStr -> 
                                                            let results:list<ExternalChoicesQueryResult<int>> = 
                                                                            [ {ResultId= "1"; ResultLabel= "Test 1"; Content= 1  };
                                                                            {ResultId= "1"; ResultLabel= "Test 1"; Content= 1  } ;
                                                                            {ResultId= "1"; ResultLabel= "Test 1"; Content= 1  }  ]
                                                            results |> List.toSeq
                                                         )
                        }
        doc.AddExternalChoicesInput(choicesInput)
        doc