namespace Common.ViewModels

open CommonViewEditors
open FsharpCommonTypes
open FSharp.ViewModule
open System.Collections.ObjectModel

type IScreen =
    abstract Name:string
//    abstract Doc: DocViewModel<InterfaceTypes.ICanValidate>
    abstract Init:unit->unit
    abstract Init:unit->unit

type CommandScreen<'ModelType  when 'ModelType :> InterfaceTypes.ICanValidate>(  name)  =
    let mutable _docViewModel:DocViewModel<'ModelType> = null
    member this.DocViewModel = _docViewModel
    member this.Init(docViewModel:DocViewModel<'ModelType>) = 
        _docViewModel <- docViewModel
        _docViewModel.Init()
//    {DocViewModel: DocViewModel<'ModelType>; 
////    CommandToExec:CommandDefinition<'ModelType>; 
//    AfterSuccessfulCmd:'ModelType->CommandResult->unit; 
//    Name: string} //Maybe we don't need the query here only in constructor; QueryForInitialization:unit->'ModelType } 
    with
        interface IScreen with
//            member this.Doc = 
//                docViewModel :> DocViewModel<#InterfaceTypes.ICanValidate>
            member this.Name = name
            member this.Init() = docViewModel.Init()
    
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module CommandScreen =
    let CreateScreen queryForInitialization viewModelBuilder screenName  =  
        let initDoc = queryForInitialization ()
        let viewModelDoc = viewModelBuilder initDoc
        CommandScreen(viewModelDoc,  screenName)

type ScreenManager() as self =
    inherit ViewModelBase() 
    let currScreens = ObservableCollection<IScreen>()
    member self.CurrentScreens with get() = currScreens
    member self.AddScreen newScreen = 
        currScreens.Add(newScreen) 
    member self.RemoveScreen screen = 
        currScreens.Remove(screen)