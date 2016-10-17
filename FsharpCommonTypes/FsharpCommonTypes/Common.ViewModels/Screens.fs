namespace Common.ViewModels

open CommonViewEditors
open FsharpCommonTypes
open FSharp.ViewModule
open System.Collections.ObjectModel
open Common.ViewModels.Interfaces

type IScreen =
    abstract ScreenId:string
    abstract DisplayName:string
    abstract DocModel:IDocViewModel
    abstract Init:unit->unit

type CommandScreen<'ModelType  when 'ModelType :> InterfaceTypes.ICanValidate>(
        docViewModel:DocViewModel<'ModelType>,   displayName, screenId)  =
    member this.DocViewModel = docViewModel
    with
        interface IScreen with
            member this.DisplayName = displayName
            member this.ScreenId = screenId
            member this.Init() = this.DocViewModel.Init()
            member this.DocModel = this.DocViewModel :> IDocViewModel
    
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module CommandScreen =
    let GenerateId screenName = 
        let timeStamp = System.DateTime.Now.Ticks.ToString("0")
        screenName + timeStamp
    let CreateScreen queryForInitialization viewModelBuilder screenName screenId  =  
        let initDoc = queryForInitialization ()
        let viewModelDoc = viewModelBuilder initDoc
        CommandScreen(viewModelDoc,  screenName, screenId)

type ScreenManager() as self =
    inherit ViewModelBase() 
    let currScreens = ObservableCollection<IScreen>()
    member self.CurrentScreens with get() = currScreens
    member self.AddScreen newScreen = 
        currScreens.Add(newScreen) 
    member self.RemoveScreen screenId = 
        currScreens 
        |> Seq.tryFind (fun i -> i.ScreenId = screenId)
        |> Option.iter (fun screenToRemove -> 
            currScreens.Remove(screenToRemove) |> ignore

            )

        