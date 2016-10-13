﻿namespace Common.ViewModels

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
                

module Sample =
    
    let CreateSampleDoc () =
        let simpleChoices =  [ {ResultId= 1; ResultLabel= "Test 1";  };
                                {ResultId= 2; ResultLabel= "Test 2";  } ;
                                {ResultId= 3; ResultLabel= "Test 3";  }  ]
        let model ={Name= BusinessTypes.LongName "Alabama" ; SalesRegion = BusinessTypes.IdNumber 1 } 
        let doc = DocViewModel(model)
        SingleInputViewModel.AddSingleInputViewModel doc SampleDoc.DefinitionName 
        SimpleChoicesViewModel.AddSimpleChoicesViewModel doc SampleDoc.DefinitionSalesRegion simpleChoices 
        doc