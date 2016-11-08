namespace Common.ViewModels

open FSharp.ViewModule
open FsharpCommonTypes
open System.Collections.ObjectModel

type PivotDimensionDefinition =
    {Header:string; FieldName:string; TotalsHeader:string}

type PivotFactDefinition =
    {Header:string; FieldName:string; AggregationType: string; FactType: string}

type PivotGridDefinition =
    { RowDimensionDefinitions: PivotDimensionDefinition seq; ColumnDimensionDefinitions: PivotDimensionDefinition seq; FactDefinitions: PivotFactDefinition seq; }
    
type PivotGridPropDefinition<'ParentType,'RecordType> =
    { PivotSettings: PivotGridDefinition; RefreshValFromDoc:'ParentType->seq<'RecordType>; SelectedItemSetter: 'ParentType->'RecordType option->'ParentType; PropName:string}

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PivotGridDefinition =
    module FactTypes =
        let DateFact = "Date"
        let NullableDateFact = "NullableDate"
        let NumericFact = "Numeric"
        let TextFact = "Text"

    let CreatePivotDimension header fieldName =
        let headerTotals = header + " Totals"
        {Header = header;  FieldName =fieldName; TotalsHeader = headerTotals}
    let CreateFactDimension header fieldName factType =
        {Header = header;  FieldName =fieldName; AggregationType = "MAX"; FactType = factType}
        

type IPivotGridViewModel =
    abstract PivotSettings:PivotGridDefinition
    abstract OnSelectedItem:System.Object->unit
    abstract OnUnselection:unit->unit
    abstract SetDataRefreshNeededCallback:(unit->unit)->unit
    


type PivotGridViewModel<'RecordType, 'ParentType>(
                                                        refreshValFromDoc:'ParentType->seq<'RecordType>, 
                                                        updateSelectedItem: 'RecordType option->'ParentType,
                                                        pushUpdatedDoc : Common.ViewModels.Interfaces.IViewComponent<'ParentType> -> 'ParentType -> unit,
                                                        propName: string,
                                                        pivotSettings: PivotGridDefinition,
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

    member self.PivotSettings with get() = pivotSettings
    member self.DataCollection with get() = dataCollection

    interface Common.ViewModels.Interfaces.IViewComponent<'ParentType> with
        member this.Init<'ParentType> vm = 
            refreshObservableCollection vm

        member this.OnDocUpdated<'ParentType> vm = 
            refreshObservableCollection vm //TODO, should we only explicitely listen for requests for list refresh?

    interface Interfaces.IViewComponent with 
        member this.Label = propName
        member this.UiHint = uiHint 
    interface IPivotGridViewModel with 
        member this.PivotSettings = pivotSettings
        member this.OnSelectedItem obj = alertParentOfDocChg (Some(obj :?> 'RecordType))
        member this.OnUnselection ()= alertParentOfDocChg None
        member this.SetDataRefreshNeededCallback f = 
            dataRefreshNeededCallback <- Some f
        


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module PivotGridViewModel =
    
    module UIHints =
        let PivotGrid = "PivotGridList"

//    let AddGridViewModel uiHint 
//                        (intoPanelViewModel:#Interfaces.IPanelViewModel<'ParentType>) (pivotPropDef:PivotGridPropDefinition<'ParentType, 'RecordType>)  =
//        let txtInput = PivotGridViewModel(pivotPropDef.RefreshValFromDoc, pivotPropDef.SelectedItemSetter, pivotPropDef.PropName, pivotPropDef.PivotSettings, uiHint )
//        intoPanelViewModel.AddChild(txtInput)

    let AddPivotGridViewModel  (docViewModel : #Interfaces.IDocViewModel<'ParentType>)
                        (intoPanelViewModel:#Interfaces.IPanelViewModel<'ParentType>) (pivotPropDef:PivotGridPropDefinition<'ParentType, 'RecordType>)  =
        let docUpdateFunc = docViewModel.GetDocAccessor(pivotPropDef.SelectedItemSetter)
        let txtInput = PivotGridViewModel(pivotPropDef.RefreshValFromDoc, docUpdateFunc, docViewModel.UpdateDoc, pivotPropDef.PropName, pivotPropDef.PivotSettings, UIHints.PivotGrid )
        intoPanelViewModel.AddChild(txtInput)

