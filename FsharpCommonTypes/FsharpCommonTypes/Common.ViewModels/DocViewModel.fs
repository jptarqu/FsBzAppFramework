namespace Common.ViewModels

open CommonViewEditors
open FsharpCommonTypes
open FSharp.ViewModule
open System.Collections.ObjectModel

type CommandViewModel<'ParentType when 'ParentType :> InterfaceTypes.ICanValidate >(isValid, commandToExec:CommandDefinition<'ParentType>, 
        onSuccess:('ParentType->CommandResult->unit), onFailure:(CommandResult->unit), getLatestDoc) as self =
    inherit ViewModelBase() 
    let mutable currCmdResult: CommandResult = { CommandResult.Message =""; Errors =Seq.empty };
    let execCmd token =
        async {
            let doc = getLatestDoc()
            let! result = commandToExec.CmdExecuter doc
            currCmdResult <- result
            let noErrors = currCmdResult.Errors |> Seq.isEmpty
            if (noErrors) then
                onSuccess doc currCmdResult
            else
                onFailure currCmdResult
            ()
            //updateEntityErrorsFromResult result
        }
    let primaryCmd = self.Factory.CommandAsyncChecked(execCmd, isValid)
    member self.Cmd with get() = primaryCmd
    member self.Name with get() = commandToExec.CmdName
    
type DocViewModel<'ParentType when 'ParentType :> InterfaceTypes.ICanValidate >(intialDoc:'ParentType, commandToExec:CommandDefinition<'ParentType>, onSuccess) as self =
    inherit ViewModelBase() 
    let mutable myDoc = intialDoc
    let currEntityErrors = ObservableCollection<PropertyError>()
    let isValid () =
//        currEntityErrors.Clear()
        let newErrors = (myDoc :> InterfaceTypes.ICanValidate).GetValidationErrors()
//        newErrors |> Seq.iter (fun i -> currEntityErrors.Add(i)) // Only if want to show entity level errors
        newErrors |> Seq.isEmpty
    let updateEntityErrorsFromResult result =
        currEntityErrors.Clear()
        result.Errors |> Seq.iter (fun i -> currEntityErrors.Add(i))
    let primaryCmd = CommandViewModel(isValid, commandToExec, onSuccess, updateEntityErrorsFromResult, (fun () -> myDoc))
    let root:Interfaces.IPanelViewModel<'ParentType> = RowsPanelViewModel("root") :> Interfaces.IPanelViewModel<'ParentType>
    let notifyChange childView newDoc =
        root.GetChildren() |> Seq.filter ((<>) childView)  |> Seq.iter (fun x -> x.OnDocUpdated newDoc) 
    let updateDoc childView newDoc =
        myDoc <-  newDoc
        notifyChange childView myDoc
        ()
    let choicesExecInt (qryExecutor:ExternalChoicesQueryExecutor< 'ParentType, int >) =
        qryExecutor myDoc

    member this.AddMyDocToFunc funcNeedeingDoc =
        funcNeedeingDoc myDoc 
    member this.GetChildViews() =
        root.GetChildren() |> Seq.rev
    member this.Init() =
        root.GetChildren() |> Seq.iter (fun x -> x.Init myDoc)
    member this.GetRootView () =
            root
    member self.CurrEntityErrors with get() = currEntityErrors
    member self.PrimaryCommand with get() = primaryCmd.Cmd
    member self.PrimaryCommandName with get() = primaryCmd.Name

    interface Interfaces.IDocViewModel<'ParentType> with
        member this.GetDocAccessor docUpdate =
            (fun newVal -> 
                docUpdate myDoc newVal
            )
        member this.UpdateDoc childView newDoc =
            updateDoc childView newDoc
        member this.GetRootView () =
            root
       
        
     
                
        
    