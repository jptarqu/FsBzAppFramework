namespace Common.ViewModels.Interfaces

open CommonViewEditors

type IPanelViewModel<'ParentType> =
    abstract AddChild: IViewComponent<'ParentType>->unit
    abstract GetChildren: unit->IViewComponent<'ParentType> seq

type IDocViewModel<'ParentType> =
    abstract GetDocAccessor:('ParentType->'A->'B)->('A->'B) 
    abstract UpdateDoc:IViewComponent<'ParentType>->'ParentType->unit
    abstract GetRootView:unit->IPanelViewModel<'ParentType>

