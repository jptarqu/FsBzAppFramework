namespace Common.ViewModels

open CommonViewEditors
open FsharpCommonTypes

type DocViewModel<'ParentType>(intialDoc:'ParentType) =
    let mutable myDoc = intialDoc
    let mutable childViewModels:List<CommonViewEditors.IViewComponent<'ParentType>> = List.empty
    let updateDoc newDoc =
        myDoc <-  newDoc
    member this.AddTextInput(definition:TextInputDefinition< 'ParentType>) =
        let docUpdate = definition.DocumentUpdate myDoc
        let docPull = definition.DocumentPull
        let newField = SingleInputViewModel<string, 'ParentType>(docPull, docUpdate,  updateDoc, definition.Constraint, definition.PropDisplayName ) :> CommonViewEditors.IViewComponent<'ParentType>
        childViewModels <- newField :: childViewModels

    member this.GetChildViews() =
        childViewModels |> Seq.rev

[<CLIMutable>]
type SampleDoc = 
    {Name: BusinessTypes.ShortName; SalesRegion: BusinessTypes.ShortName } 
    with
        interface InterfaceTypes.ICanValidate with 
            member this.GetValidationErrors () = 
                [ this.Name.GetValidationErrors(); this.SalesRegion.GetValidationErrors() ] |> InterfaceTypes.CollectAllError

module Sample =
    let CreateSampleDoc () =
        let model ={Name= (BusinessTypes.NewShortName "Alabama" "Name"); SalesRegion =(BusinessTypes.NewShortName "South" "Sales Region") } 
        let doc = DocViewModel(model)
        let txtInput = {
                        TextInputDefinition.PropDisplayName = "Name"; 
                        DocumentPull = (fun x -> x.Name.Value) ; 
                        DocumentUpdate = (fun doc x -> {doc with Name = doc.Name.WithValue x });
                        Constraint = model.Name.GetDefinition() 
                        }
        doc.AddTextInput(txtInput)
        doc