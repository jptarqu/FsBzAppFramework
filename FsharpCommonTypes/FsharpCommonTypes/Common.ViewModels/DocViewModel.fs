namespace Common.ViewModels

open FsharpCommonTypes
open FSharp.ViewModule
open System.Collections.ObjectModel

type ICommandViewModel =
    abstract member Cmd : IAsyncNotifyCommand
    abstract member Name : string

type CommandViewModel<'ParentType when 'ParentType :> InterfaceTypes.ICanValidate>(isValid, commandToExec : CommandDefinition<'ParentType>, onSuccess : 'ParentType -> CommandResult -> unit, onFailure : CommandResult -> unit, getLatestDoc) as self = 
    inherit ViewModelBase()
    
    let mutable currCmdResult : CommandResult = 
        { CommandResult.Message = ""
          Errors = Seq.empty }
    
    let execCmd token = 
        async { 
            let doc = getLatestDoc()
            let! result = commandToExec.CmdExecuter doc
            currCmdResult <- result
            let noErrors = currCmdResult.Errors |> Seq.isEmpty
            if (noErrors) then onSuccess doc currCmdResult
            else onFailure currCmdResult
            ()
        }
    
    let primaryCmd = self.Factory.CommandAsyncChecked(execCmd, isValid)
    member self.Cmd = primaryCmd
    member self.Name = commandToExec.CmdName
    with
        interface ICommandViewModel with
            member x.Cmd: IAsyncNotifyCommand = self.Cmd
            member x.Name: string = self.Name
            

type DocViewModel<'ParentType when 'ParentType :> InterfaceTypes.ICanValidate>(intialDoc : 'ParentType, 
                                                                                commandToExec : CommandDefinition<'ParentType>, 
                                                                                onSuccess, 
                                                                                cancelCommand : CommandDefinition<'ParentType>, 
                                                                                onCancel,
                                                                                primaryCommands :  CommandDefinition<'ParentType> seq)  = 
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
        CommandViewModel((fun () -> commandToExec.CanRunCheck myDoc), commandToExec, onSuccess, updateEntityErrorsFromResult, (fun () -> myDoc))
    let primaryCmds = 
        primaryCommands
        |> Seq.map (fun c -> CommandViewModel((fun () -> c.CanRunCheck myDoc), c, onSuccess, updateEntityErrorsFromResult, (fun () -> myDoc)) :> ICommandViewModel)
    let cancelCmd = 
        CommandViewModel((fun () -> cancelCommand.CanRunCheck myDoc), cancelCommand, onCancel, updateEntityErrorsFromResult, (fun () -> myDoc))

    let root : Interfaces.IPanelViewModel<'ParentType> = 
        RowsPanelViewModel("root") :> Interfaces.IPanelViewModel<'ParentType>
    
    let notifyChange childView newDoc = 
        root.GetTypedChildren()
        |> Seq.filter ((<>) childView)
        |> Seq.iter (fun x -> x.OnDocUpdated newDoc)
    
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
        member this.GetDocAccessor docUpdate = (fun newVal -> docUpdate myDoc newVal)
        member this.UpdateDoc childView newDoc = updateDoc childView newDoc
        member this.GetTypedRootView() = root
    
    interface Interfaces.IDocViewModel with
        member this.GetRootView() = root :> Common.ViewModels.Interfaces.IPanelViewModel
