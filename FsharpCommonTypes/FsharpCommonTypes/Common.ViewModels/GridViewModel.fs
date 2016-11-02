namespace Common.ViewModels

open FSharp.ViewModule
open FsharpCommonTypes
open System.Collections.ObjectModel

    
type TablePropDefinition<'ParentType,'RecordType> =
    {  RefreshValFromDoc:'ParentType->seq<'RecordType>; SelectedItemSetter: 'ParentType->'RecordType option->'ParentType; PropName:string}

              

type ITableViewModel =
    abstract OnSelectedItem:System.Object->unit
    abstract OnUnselection:unit->unit
    abstract SetDataRefreshNeededCallback:(unit->unit)->unit
    


type TableViewModel<'RecordType, 'ParentType>(
                                                        refreshValFromDoc:'ParentType->seq<'RecordType>, 
                                                        updateSelectedItem: 'RecordType option->'ParentType,
                                                        pushUpdatedDoc : Common.ViewModels.Interfaces.IViewComponent<'ParentType> -> 'ParentType -> unit,
                                                        propName: string,
                                                        uiHint: string) as self = 
    inherit ViewModelBase()
        
        
    let dataCollection = ObservableCollection<'RecordType>() //TODO, should we use ObServable COllections for large sets?
    let mutable dataRefreshNeededCallback:((unit->unit) option) = None

    let refreshObservableCollection vm =
        dataCollection.Clear()
        let rawList = refreshValFromDoc vm
        for item in rawList do
            dataCollection.Add item
        match dataRefreshNeededCallback with
        | Some f -> f()
        | None -> ()
            
    let alertParentOfDocChg newVal = 
        let newDoc = updateSelectedItem newVal
        pushUpdatedDoc self newDoc

    member self.DataCollection with get() = dataCollection

    interface Common.ViewModels.Interfaces.IViewComponent<'ParentType> with
        member this.Init<'ParentType> vm = 
            refreshObservableCollection vm

        member this.OnDocUpdated<'ParentType> vm = 
            refreshObservableCollection vm //TODO, should we only explicitely listen for requests for list refresh?

    interface Interfaces.IViewComponent with 
        member this.Label = propName
        member this.UiHint = uiHint 
    interface ITableViewModel with 
        member this.OnSelectedItem obj = alertParentOfDocChg (Some(obj :?> 'RecordType))
        member this.OnUnselection ()= alertParentOfDocChg None
        member this.SetDataRefreshNeededCallback f = 
            dataRefreshNeededCallback <- Some f
        


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module TableViewModel =
    
    module UIHints =
        let Table = "TableList"
        
    let AddTableViewModel  (docViewModel : #Interfaces.IDocViewModel<'ParentType>)
                        (intoPanelViewModel:#Interfaces.IPanelViewModel<'ParentType>) (tablePropDef:TablePropDefinition<'ParentType, 'RecordType>)  =
        let docUpdateFunc = docViewModel.GetDocAccessor(tablePropDef.SelectedItemSetter)
        let table = TableViewModel(tablePropDef.RefreshValFromDoc, docUpdateFunc, docViewModel.UpdateDoc, tablePropDef.PropName,  UIHints.Table )
        intoPanelViewModel.AddChild(table)

