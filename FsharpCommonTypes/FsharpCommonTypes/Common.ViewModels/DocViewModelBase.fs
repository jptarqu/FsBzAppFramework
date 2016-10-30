namespace Common.ViewModels

open FsharpCommonTypes
open FSharp.ViewModule
open System.Collections.ObjectModel


type DocViewModelBase<'ParentType when 'ParentType :> InterfaceTypes.ICanValidate>(intialDoc : 'ParentType
                                                                               ) as self  = 
    inherit ViewModelBase()
    let mutable myDoc = intialDoc
    let currEntityErrors = ObservableCollection<PropertyError>()
    let primaryCommands = ObservableCollection<ICommandViewModel>()
    let mutable docIsValid = true
    let isValid() = docIsValid
    
    let updateValidationErrors() = 
        let newErrors = (myDoc :> InterfaceTypes.ICanValidate).GetValidationErrors()
        let entityErros = newErrors |> Seq.filter (fun e -> e.PropertyName = "")
        currEntityErrors.Clear()
        entityErros |> Seq.iter (fun i -> currEntityErrors.Add(i))
        docIsValid <- (newErrors |> Seq.isEmpty)
    
    let updateEntityErrorsFromResult result = 
        currEntityErrors.Clear()
        result.Errors |> Seq.iter (fun i -> currEntityErrors.Add(i))
        docIsValid <- (result.Errors |> Seq.isEmpty)
    
    
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
        primaryCommands |> Seq.iter (fun c -> c.Cmd.RaiseCanExecuteChanged())

    let updateDoc childView newDoc = 
        myDoc <- newDoc
        notifyChange childView myDoc
        updateValidationErrors()
        primaryCommands |> Seq.iter (fun c -> c.Cmd.RaiseCanExecuteChanged())
        ()
    
//    let choicesExecInt (qryExecutor : ExternalChoicesQueryExecutor<'ParentType, int>) = qryExecutor myDoc
    member this.AddMyDocToFunc funcNeedeingDoc = funcNeedeingDoc myDoc
    member this.GetChildViews() = root.GetTypedChildren() |> Seq.rev
    member this.Init() = 
        root.GetTypedChildren() |> Seq.iter (fun x -> x.Init myDoc)
        updateValidationErrors()
        primaryCommands |> Seq.iter (fun c -> c.Cmd.RaiseCanExecuteChanged())
        
    member this.ReloadDoc = reloadDoc
    member this.GetRootView() = root
    member self.CurrEntityErrors = currEntityErrors

    member self.PrimaryCommands = primaryCommands
    
    member self.SecondaryCommands:ObservableCollection<ICommandViewModel> = new ObservableCollection<ICommandViewModel>()

    member self.GetCurrentDoc = myDoc
    
    interface Interfaces.IDocViewModel<'ParentType> with
//        member this.Reload newDoc = reloadDoc newDoc
        member this.GetDocAccessor docUpdate = (fun newVal -> docUpdate myDoc newVal)
        member this.UpdateDoc childView newDoc = updateDoc childView newDoc
        member this.GetTypedRootView() = root
    
    interface Interfaces.IDocViewModel with
        member this.GetRootView() = root :> Common.ViewModels.Interfaces.IPanelViewModel

