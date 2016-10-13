namespace Common.ViewModels.Interfaces

open CommonViewEditors

type IDocViewModel<'ParentType> =
    abstract AddChild: IViewComponent<'ParentType>->unit
    abstract GetDocAccessor:('ParentType->'A->'B)->('A->'B) 
    abstract UpdateDoc:IViewComponent<'ParentType>->'ParentType->unit

