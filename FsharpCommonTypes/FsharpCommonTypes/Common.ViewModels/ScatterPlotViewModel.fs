namespace Common.ViewModels

open FSharp.ViewModule
open FsharpCommonTypes
open System.Collections.ObjectModel

//    
//
//              
//type CharPoint =
//    {XValue: int; YValue: int}

type ScatterPlotPropDefinition<'ParentType,'RecordType> =
    {  RefreshValFromDoc:'ParentType->seq<'RecordType>; XPropName: string; YPropName: string; PropName:string}
    
type IScatterPlotViewModel =
//    abstract OnSelectedItem:System.Object->unit
//    abstract OnUnselection:unit->unit
    abstract SetDataRefreshNeededCallback:(unit->unit)->unit

type ScatterPlotViewModel<'RecordType, 'ParentType>(
                                                        refreshValFromDoc:'ParentType->seq<'RecordType>, 

                                                        pushUpdatedDoc : Common.ViewModels.Interfaces.IViewComponent<'ParentType> -> 'ParentType -> unit,
                                                        propName: string,
                                                        xPropName: string, 
                                                        yPropName: string,
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
            
//    let alertParentOfDocChg newVal = 
//        let newDoc = updateSelectedItem newVal
//        pushUpdatedDoc self newDoc

    member self.DataCollection with get() = dataCollection

    interface Common.ViewModels.Interfaces.IViewComponent<'ParentType> with
        member this.Init<'ParentType> vm = 
            refreshObservableCollection vm

        member this.OnDocUpdated<'ParentType> vm = 
            refreshObservableCollection vm //TODO, should we only explicitely listen for requests for list refresh?

    interface Interfaces.IViewComponent with 
        member this.Label = propName
        member this.UiHint = uiHint 
    interface IScatterPlotViewModel with 
        member this.SetDataRefreshNeededCallback f = 
            dataRefreshNeededCallback <- Some f
        


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ScatterPlotViewModel =
    
    module UIHints =
        let ScatterPlot = "ScatterPlotList"
        
    let AddScatterPlotViewModel  (docViewModel : #Interfaces.IDocViewModel<'ParentType>)
                        (intoPanelViewModel:#Interfaces.IPanelViewModel<'ParentType>) (scatterPlotPropDef:ScatterPlotPropDefinition<'ParentType, 'RecordType>)  =
//        let docUpdateFunc = docViewModel.GetDocAccessor(scatterPlotPropDef.SelectedItemSetter)
        let scatterPlot = ScatterPlotViewModel(scatterPlotPropDef.RefreshValFromDoc, docViewModel.UpdateDoc, scatterPlotPropDef.PropName, scatterPlotPropDef.XPropName, scatterPlotPropDef.YPropName, UIHints.ScatterPlot )
        intoPanelViewModel.AddChild(scatterPlot)