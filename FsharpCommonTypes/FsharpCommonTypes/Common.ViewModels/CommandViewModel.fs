namespace Common.ViewModels

open FsharpCommonTypes
open FSharp.ViewModule

type ICommandViewModel =
    abstract member Cmd : IAsyncNotifyCommand
    abstract member Name : string

type CommandViewModel<'ParentType when 'ParentType :> InterfaceTypes.ICanValidate>(commandToExec : CommandDefinition<'ParentType>, onSuccess : 'ParentType -> CommandResult -> unit, onFailure : CommandResult -> unit, getLatestDoc) as self = 
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
//            if (noErrors) then onSuccess parentViewModel doc currCmdResult
            if (noErrors) then onSuccess doc currCmdResult
            else onFailure currCmdResult
            ()
        }
    
    let canRun () = getLatestDoc() |> commandToExec.CanRunCheck
    let primaryCmd = self.Factory.CommandAsyncChecked(execCmd, canRun)
    member self.Cmd = primaryCmd
    member self.Name = commandToExec.CmdName
    with
        interface ICommandViewModel with
            member x.Cmd: IAsyncNotifyCommand = self.Cmd
            member x.Name: string = self.Name
            
