namespace Common.ViewModels

open FSharp.ViewModule
open FsharpCommonTypes

type RowsPanelViewModel<'ParentType >(groupName: string) as self = 

    let mutable childViewModels:List<CommonViewEditors.IViewComponent<'ParentType>> = List.empty
    
    interface Interfaces.IPanelViewModel<'ParentType> with
        member this.AddChild( newChild:CommonViewEditors.IViewComponent<'ParentType>) =
            childViewModels <- newChild :: childViewModels
        member this.GetChildren () =
            childViewModels |> Seq.rev
    interface CommonViewEditors.IViewComponent<'ParentType> with
        member this.Init<'ParentType> vm = 
            childViewModels |> List.iter (fun i -> i.Init vm)

        member this.OnDocUpdated<'ParentType> vm = 
            childViewModels |> List.iter (fun i -> i.OnDocUpdated vm)

        member this.Label = groupName
        member this.UiHint = "RowsPanel"
