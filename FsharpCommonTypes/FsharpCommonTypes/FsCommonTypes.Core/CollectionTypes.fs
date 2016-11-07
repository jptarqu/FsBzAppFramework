namespace FsharpCommonTypes

[<CLIMutable>]
type SelectableList<'ChildViewModel> = 
    {Rows: 'ChildViewModel seq; SelectedItem: Option<'ChildViewModel>; ApiErrorMessages: string seq} 
    with
        interface InterfaceTypes.ICanValidate with 
            member this.GetValidationErrors () = Seq.empty
            
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module SelectableList = 
    let HasSelectedItem (x:SelectableList<'ChildViewModel>) =
        Option.isSome x.SelectedItem
    let Empty ():SelectableList<'ChildViewModel> = {Rows = Seq.empty; SelectedItem = None; ApiErrorMessages = Seq.empty} 