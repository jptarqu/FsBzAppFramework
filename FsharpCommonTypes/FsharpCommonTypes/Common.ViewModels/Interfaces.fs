namespace Common.ViewModels.Interfaces



type SimpleExternalChoicesQueryResult<'PrimitiveType> = {ResultId: 'PrimitiveType; ResultLabel: string; }

type IExternalChoicesQry<'PrimitiveType when 'PrimitiveType: equality> =
    abstract QueryExecutor:string->seq<SimpleExternalChoicesQueryResult<'PrimitiveType>>

type IViewComponent =
    abstract Label:string
    abstract UiHint:string
    
type IViewComponent<'ViewModel> =
    abstract OnDocUpdated<'ViewModel> : 'ViewModel -> unit
    abstract Init<'ViewModel> : 'ViewModel -> unit

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


