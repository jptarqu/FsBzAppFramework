namespace Common.ViewModels

open CommonViewEditors
open FsharpCommonTypes

type CommandScreen<'ModelType  when 'ModelType :> InterfaceTypes.ICanValidate> =
    {DocViewModel: DocViewModel<'ModelType>; 
//    CommandToExec:CommandDefinition<'ModelType>; 
    AfterSuccessfulCmd:'ModelType->CommandResult->unit; 
    Name: string} //Maybe we don't need the query here only in constructor; QueryForInitialization:unit->'ModelType } 
    with
        member this.Init () = this.DocViewModel.Init()
    
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module CommandScreen =
    let CreateScreen afterSuccess queryForInitialization viewModelBuilder screenName  =  
        let initDoc = queryForInitialization ()
        let viewModelDoc = viewModelBuilder initDoc
        { CommandScreen.DocViewModel = viewModelDoc;  AfterSuccessfulCmd = afterSuccess; Name=screenName}
