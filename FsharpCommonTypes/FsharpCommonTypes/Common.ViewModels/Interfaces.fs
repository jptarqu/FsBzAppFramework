namespace Common.ViewModels.Interfaces

open CommonViewEditors
open FsharpCommonTypes

type IViewComponent =
    abstract Label:string
    abstract UiHint:string
    
type IPanelViewModel =
    abstract GetChildren: unit->IViewComponent seq

type IPanelViewModel<'ParentType> =
    inherit IPanelViewModel 
    abstract AddChild: IViewComponent<'ParentType>->unit
    abstract GetTypedChildren: unit->IViewComponent<'ParentType> seq

    
type IDocViewModel =
    abstract GetRootView:unit->IPanelViewModel
    
type IDocViewModel<'ParentType> =
    inherit IDocViewModel 
    abstract GetDocAccessor:('ParentType->'A->'B)->('A->'B) 
    abstract UpdateDoc:IViewComponent<'ParentType>->'ParentType->unit
    abstract GetTypedRootView:unit->IPanelViewModel<'ParentType>


