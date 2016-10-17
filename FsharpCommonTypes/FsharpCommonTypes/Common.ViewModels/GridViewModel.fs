namespace Common.ViewModels

open FSharp.ViewModule
open FsharpCommonTypes
open System.Collections.ObjectModel

type PivotDimensionDefinition =
    {Header:string; FieldName:string; TotalsHeader:string}

type PivotFactDefinition =
    {Header:string; FieldName:string; AggregationType: string}

type PivotGridDefinition =
    { RowDimensionDefinitions: PivotDimensionDefinition seq; ColumnDimensionDefinitions: PivotDimensionDefinition seq; FactDefinitions: PivotFactDefinition seq; }
    
type PivotGridPropDefinition<'ParentType,'RecordType> =
    { PivotSettings: PivotGridDefinition; RefreshValFromDoc:'ParentType->seq<'RecordType>; OnSelectedItem: 'RecordType->unit; PropName:string}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PivotGridDefinition =
    let CreatePivotDimension header fieldName =
        let headerTotals = header + " Totals"
        {Header = header;  FieldName =fieldName; TotalsHeader = headerTotals}
    let CreateFactDimension header fieldName =
        {Header = header;  FieldName =fieldName; AggregationType = "MAX"}
        

type IPivotGridViewModel =
    abstract PivotSettings:PivotGridDefinition
    abstract OnSelectedItem:System.Object->unit


type PivotGridViewModel<'RecordType, 'ParentType>(
                                                        refreshValFromDoc:'ParentType->seq<'RecordType>, 
                                                        onSelectedItem: 'RecordType->unit,
                                                        propName: string,
                                                        pivotSettings: PivotGridDefinition,
                                                        uiHint: string)  = 
    inherit ViewModelBase()
        
        
    let dataCollection = ObservableCollection<'RecordType>() //TODO, should we use ObServable COllections for large sets?
    let refreshObservableCollection vm =
        dataCollection.Clear()
        let rawList = refreshValFromDoc vm
        for item in rawList do
            dataCollection.Add item
            
    member self.PivotSettings with get() = pivotSettings
    member self.DataCollection with get() = dataCollection

    interface CommonViewEditors.IViewComponent<'ParentType> with
        member this.Init<'ParentType> vm = 
            refreshObservableCollection vm

        member this.OnDocUpdated<'ParentType> vm = 
            refreshObservableCollection vm //TODO, should we only explicitely listen for requests for list refresh?

    interface Interfaces.IViewComponent with 
        member this.Label = propName
        member this.UiHint = uiHint 
    interface IPivotGridViewModel with 
        member this.PivotSettings = pivotSettings
        member this.OnSelectedItem obj = onSelectedItem (obj :?> 'RecordType)



[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PivotGridViewModel =
    
    module UIHints =
        let PivotGrid = "PivotGridList"

    let AddGridViewModel uiHint 
                        (intoPanelViewModel:#Interfaces.IPanelViewModel<'ParentType>) (pivotPropDef:PivotGridPropDefinition<'ParentType, 'RecordType>)  =
        let txtInput = PivotGridViewModel(pivotPropDef.RefreshValFromDoc, pivotPropDef.OnSelectedItem, pivotPropDef.PropName, pivotPropDef.PivotSettings, uiHint )
        intoPanelViewModel.AddChild(txtInput)

    let AddPivotGridViewModel  
                        (intoPanelViewModel:#Interfaces.IPanelViewModel<'ParentType>) (pivotPropDef:PivotGridPropDefinition<'ParentType, 'RecordType>)  =
        let txtInput = PivotGridViewModel(pivotPropDef.RefreshValFromDoc, pivotPropDef.OnSelectedItem, pivotPropDef.PropName, pivotPropDef.PivotSettings, UIHints.PivotGrid )
        intoPanelViewModel.AddChild(txtInput)

