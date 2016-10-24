namespace Common.ViewModels

open FsharpCommonTypes
open FSharp.ViewModule
open System.Collections.ObjectModel
open Common.ViewModels.Interfaces

type IDialogService =
    abstract PromptMessage : string->string->unit
    
type IScreen = 
    abstract ScreenId : string
    abstract DisplayName : string
    abstract DocModel : IDocViewModel
    abstract Init : unit -> unit

type CommandScreen<'ModelType  when 'ModelType :> InterfaceTypes.ICanValidate>(
        docViewModel:DocViewModel<'ModelType>,   displayName, screenId)  =
    member this.DocViewModel = docViewModel
    with
        interface IScreen with
            member this.DisplayName = displayName
            member this.ScreenId = screenId
            member this.Init() = this.DocViewModel.Init()
            member this.DocModel = this.DocViewModel :> IDocViewModel
            
type ScreenManager() as self = 
    inherit ViewModelBase()
    let currScreens = ObservableCollection<IScreen>()
    member self.CurrentScreens = currScreens
    member self.AddScreen newScreen = currScreens.Add(newScreen)
    member self.RemoveScreen screenId = 
        currScreens
        |> Seq.tryFind (fun i -> i.ScreenId = screenId)
        |> Option.iter (currScreens.Remove >> ignore)  // (fun screenToRemove -> currScreens.Remove(screenToRemove) |> ignore)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module CommandScreen = 
    module Helpers =
        let CloseAfterSuccess (screenManager:ScreenManager) screenId doc cmdResult = // TODO maybe add this to helper funcs in common?
                screenManager.RemoveScreen screenId
                ()
        let DoNothingAfterFailure doc cmdResult =
                ()
        let AddAndInitScreen (screenManager:ScreenManager) screen =
            screenManager.AddScreen screen 
            (screen :> IScreen).Init()
            screen

    let GenerateId screenName = 
        let timeStamp = System.DateTime.Now.Ticks.ToString("0")
        screenName + timeStamp
    
    let CreateScreen queryForInitialization viewModelBuilder screenName screenId = 
        let initDoc = queryForInitialization()
        let viewModelDoc = viewModelBuilder initDoc
        CommandScreen(viewModelDoc, screenName, screenId)

