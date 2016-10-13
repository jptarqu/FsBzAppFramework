namespace Common.ViewModels

open CommonViewEditors
open FsharpCommonTypes
open FSharp.ViewModule
open System.Collections.ObjectModel

type ExternalChoicesViewModel<'PrimitiveType , 'ParentType when 'PrimitiveType: equality >(propFactory:PropFactoryMethod<'PrimitiveType>,
                                                            refreshValFromDoc:'ParentType->BzProp<'PrimitiveType>, 
                                                            refreshDocFromVal:BzProp<'PrimitiveType>->'ParentType, // allow create new doc by sending the newly BzProp<'PrimitiveType>
                                                            pushUpdatedDoc: CommonViewEditors.IViewComponent<'ParentType> ->'ParentType->unit,
                                                            queryExecutor: string -> seq<SimpleExternalChoicesQueryResult<'PrimitiveType>>,
                                                            labelLkup: 'PrimitiveType -> string,
                                                            propName: string,
                                                            defaultValue: 'PrimitiveType) as self = 
    inherit ViewModelBase()
//    let mutable txtValue = defaultValue
    let mutable currErrors:seq<PropertyError> = Seq.empty
    
    let getStrErrors = BusinessTypes.GetStrErrors propFactory

    let txtValue = self.Factory.Backing(<@ self.Value @>, defaultValue, getStrErrors)
    let txtLabel = self.Factory.Backing(<@ self.ResultLabel @>, "")


    let isValueValid = 
        currErrors |> Seq.isEmpty
    let alertParentOfDocChg newVal =
        let newDoc = refreshDocFromVal newVal
        pushUpdatedDoc self newDoc

        
    member self.Value with get() = txtValue.Value 
                        and set value = 
                            if (value <> txtValue.Value) then
                                txtValue.Value <- value
                                let newPropState = propFactory value
                                alertParentOfDocChg newPropState 

    member self.ResultLabel with get() = txtLabel.Value 
                            and set value = 
                                if (value <> txtLabel.Value) then
                                    txtLabel.Value <- value
                                   
    member self.PropName with get() = propName

    interface CommonViewEditors.IViewComponent<'ParentType> with
        member this.Init<'ParentType> vm = 
            let primitiveVal = BusinessTypes.ToPrimitive (refreshValFromDoc vm)
            self.Value <- primitiveVal
            self.ResultLabel <- labelLkup primitiveVal

        member this.OnDocUpdated<'ParentType> vm = 
            let primitiveVal = BusinessTypes.ToPrimitive (refreshValFromDoc vm)
            self.Value <- primitiveVal
            self.ResultLabel <- labelLkup primitiveVal

        member this.Label = propName
        member this.UiHint = "ExternalChoices"
    interface IExternalChoicesQry<'PrimitiveType> with
        member this.QueryExecutor filterStr = 
            queryExecutor filterStr
    
type OldExternalChoicesViewModel<'ParentType>(docPull:'ParentType->int, 
                                                        docUpdate: int->'ParentType, 
                                                        pushUpdatedDoc: CommonViewEditors.IViewComponent<'ParentType> ->'ParentType->unit,
                                                        queryExecutor: string -> seq<ExternalChoicesQueryResult<int>>,
                                                        propName: string,
                                                        defaultValue: int) as self = 
    inherit ViewModelBase()
//    let mutable txtValue = defaultValue
    let mutable currErrors:seq<PropertyError> = Seq.empty

    let txtValue = self.Factory.Backing(<@ self.Value @>, defaultValue)
    let possibleChoices = ObservableCollection<int>()


    // ...
    let validate () = 
        currErrors <- [ ] // TODO add validation for primitive not being default
    let isValueValid = 
        currErrors |> Seq.isEmpty
    let alertParentOfDocChg newVal =
        let newDoc = docUpdate newVal
        pushUpdatedDoc self newDoc

    let limitChoices newFilter =
        let newChoices = queryExecutor (newFilter.ToString())
        possibleChoices.Clear()
        newChoices |> Seq.iter (fun item -> possibleChoices.Add(item.Content))
        ()
        
    member self.Value with get() = txtValue.Value 
                        and set value = 
                            if (value <> txtValue.Value) then
                                txtValue.Value <- value
                                //limitChoices txtValue.Value
                                validate()
                                if (isValueValid) then
                                   alertParentOfDocChg txtValue.Value
                                   
    member self.PossibleChoices with get() = possibleChoices
    member self.PropName with get() = propName

    interface CommonViewEditors.IViewComponent<'ParentType> with
        member this.Init<'ParentType> vm = 
            self.Value <- (docPull vm)

        member this.OnDocUpdated<'ParentType> vm = 
            self.Value <- (docPull vm)
            ()

        member this.Label = propName
        member this.UiHint = "ExternalChoices"
    interface IIntExternalChoicesQry with
        member this.QueryExecutor filterStr = 
            queryExecutor filterStr
    

//    
//[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
//module SimpleChoicesViewModel =
//    let AddSimpleChoicesViewModel (docViewModel:#Interfaces.IDocViewModel<'ParentType>) (propDef:PropDefinition<'ParentType, 'Primitive>)  =
//        let docUpdate = docViewModel.GetDocAccessor(propDef.Setter)
//        let txtInput = SingleInputViewModel(propDef.Factory, propDef.Getter, docUpdateName,  docViewModel.UpdateDoc, propDef.Name, "")
//        
//        let qryExec = doc.AddMyDocToFunc (fun doc filterStr -> 
//                                                            let results:list<SimpleExternalChoicesQueryResult<int>> = 
//                                                                            [ {ResultId= 1; ResultLabel= "Test 1";  };
//                                                                            {ResultId= 2; ResultLabel= "Test 2";  } ;
//                                                                            {ResultId= 3; ResultLabel= "Test 3";  }  ]
//                                                            results |> Seq.filter (fun x -> x.ResultId.ToString() = filterStr) 
//                                                         ) 
//        let lblLkp = (fun index -> 
//                "DUMMY Test 2"
//                ) 
//        let simpleChoices =  [ {ResultId= 1; ResultLabel= "Test 1";  };
//                                {ResultId= 2; ResultLabel= "Test 2";  } ;
//                                {ResultId= 3; ResultLabel= "Test 3";  }  ]
//        let choicesInput = SimpleChoicesViewModel(propDef.Factory, propDef.Getter, docUpdate,  docViewModel.UpdateDoc, simpleChoices,   "SalesRegion", 0)
//       
//        doc.AddChild(choicesInput)