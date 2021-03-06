﻿namespace Common.ViewModels

open FsharpCommonTypes
open FSharp.ViewModule
open System.Collections.ObjectModel
open Common.ViewModels.Interfaces


type DocViewModel<'ParentType when 'ParentType :> InterfaceTypes.ICanValidate>(intialDoc : 'ParentType, 
                                                                                commandToExec : CommandDefinition<'ParentType>, 
                                                                                onSuccess, 
                                                                                cancelCommand : CommandDefinition<'ParentType>, 
                                                                                onCancel,
                                                                                primaryCommands :  CommandDefinition<'ParentType> seq) as self  = 
    inherit ViewModelBase()
    let mutable myDoc = intialDoc
    let currEntityErrors = ObservableCollection<PropertyError>()
    let mutable docIsValid = true
    let isValid() = docIsValid
    
    let updateValidationErrors() = 
        //        currEntityErrors.Clear()
        let newErrors = (myDoc :> InterfaceTypes.ICanValidate).GetValidationErrors()
        let entityErros = newErrors |> Seq.filter (fun e -> e.PropertyName = "")
        currEntityErrors.Clear()
        entityErros |> Seq.iter (fun i -> currEntityErrors.Add(i))
        docIsValid <- (newErrors |> Seq.isEmpty)
    
    let updateEntityErrorsFromResult result = 
        currEntityErrors.Clear()
        result.Errors |> Seq.iter (fun i -> currEntityErrors.Add(i))
        docIsValid <- (result.Errors |> Seq.isEmpty)
    
    let primaryCmd = 
        CommandViewModel( commandToExec, onSuccess, updateEntityErrorsFromResult, (fun () -> myDoc))
    let primaryCmds = 
        primaryCommands
        |> Seq.map (fun c -> CommandViewModel(c, onSuccess, updateEntityErrorsFromResult, (fun () -> myDoc)) :> ICommandViewModel)
    let cancelCmd = 
        CommandViewModel( cancelCommand, onCancel, updateEntityErrorsFromResult, (fun () -> myDoc))

    let root : Interfaces.IPanelViewModel<'ParentType> = 
        RowsPanelViewModel("root") :> Interfaces.IPanelViewModel<'ParentType>
    


    let notifyChange childView newDoc = 
        root.GetTypedChildren()
        |> Seq.filter ((<>) childView)
        |> Seq.iter (fun x -> x.OnDocUpdated newDoc)
    
    let reloadDoc newDoc = 
        myDoc <- newDoc
        root.GetTypedChildren() |> Seq.iter (fun x -> x.Init myDoc)
        updateValidationErrors()
        primaryCmd.Cmd.RaiseCanExecuteChanged()

    let updateDoc childView newDoc = 
        myDoc <- newDoc
        notifyChange childView myDoc
        updateValidationErrors()
        primaryCmd.Cmd.RaiseCanExecuteChanged()
        ()
    
//    let choicesExecInt (qryExecutor : ExternalChoicesQueryExecutor<'ParentType, int>) = qryExecutor myDoc
    member this.AddMyDocToFunc funcNeedeingDoc = funcNeedeingDoc myDoc
    member this.GetChildViews() = root.GetTypedChildren() |> Seq.rev
    member this.Init() = 
        root.GetTypedChildren() |> Seq.iter (fun x -> x.Init myDoc)
        updateValidationErrors()
        primaryCmd.Cmd.RaiseCanExecuteChanged()

    member this.GetRootView() = root
    member self.CurrEntityErrors = currEntityErrors
    member self.PrimaryCommand = primaryCmd.Cmd
    member self.PrimaryCommandName = primaryCmd.Name
    member self.PrimaryCommands:ICommandViewModel seq = primaryCmds

    member self.CancelCommand = cancelCmd.Cmd
    member self.CancelCommandName = cancelCmd.Name
    member self.GetCurrentDoc = myDoc
    
    interface Interfaces.IDocViewModel<'ParentType> with
//        member this.Reload newDoc = reloadDoc newDoc
        member this.GetDocAccessor docUpdate = (fun newVal -> docUpdate myDoc newVal)
        member this.UpdateDoc childView newDoc = updateDoc childView newDoc
        member this.GetTypedRootView() = root
    
    interface Interfaces.IDocViewModel with
        member this.GetRootView() = root :> Common.ViewModels.Interfaces.IPanelViewModel
