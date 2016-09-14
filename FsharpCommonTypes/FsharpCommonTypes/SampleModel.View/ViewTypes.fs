namespace TypesPlayground

module ViewTypes =
    open QueryTypes
    open CommonViewEditors 
    open Region
    open QueryParamTypes

    type ICustomerApplicationDataService =
        abstract GetBasicCustomerInfo: NewCustomerApplicationQryParam->BasicCustomerInfo
        abstract GetCustomerTypes: GetCustomerTypesQryParam->CustomerType seq

    type ScreenManager =
        {ViewBuilder:ISceenBuilderService; DataService: ICustomerApplicationDataService; ViewManager: IScreenManager } // TODO maybe make ICustomerApplicationDataService more dynamically generated
        
        with 
        member private this.CustomerTypesToExternalChoices txtTypedSofar =
            this.DataService.GetCustomerTypes { CustomerTypeName = txtTypedSofar } |> Seq.map (fun l -> 
                
                {
                    ResultId = l.CustomerTypeId.TextValue
                    ResultLabel = l.Name.Value 
                    Content = FsharpCommonTypes.BusinessTypes.NewIdWrapper l l.CustomerTypeId
                })

        member this.BuildEditBasicCustomerInfoScreen (newCustomerApplicationQryParam:NewCustomerApplicationQryParam) = 
            let qryResult = this.DataService.GetBasicCustomerInfo(newCustomerApplicationQryParam)
            let saveCmd = this.ViewBuilder.CreateExternalCommand {CmdName = "Save"; CmdTransformer= (fun qry -> qry); CmdAddress = "/api/BasicCustomerInfo";}
            let viewTree = 
                [
                    this.ViewBuilder.CreateInputWrapperChoices ( {
                                                                    ExternalChoicesInputDefinition.DocumentPull =  (fun x -> x.CustomerType.Id.TextValue) 
                                                                    PropDisplayName = "Is Customer Secured"
                                                                    DocumentUpdate = (fun x doc  -> {doc with CustomerType = x.Content}) 
                                                                    ExternalChoicesQueryExecutor = (fun x txtTypedSofar -> 
                                                                        this.CustomerTypesToExternalChoices txtTypedSofar 
//                                                                            |> Seq.map (fun r -> r.)
                                                                        )   
                                                                        // TODO find a way to allow search on server via text entered into some other prop? should the prop be just part of the ViewModel of the QueryModel and just not be bound to the command?? or should the InputField have support for that?? (whihc is what we did here)
                                            //                                                                        ExternalChoicesQueryMapper = (fun result -> {Id = result.CustomerTypeId; Entity= result}  )  
                                                                    } )
                    this.ViewBuilder.CreateInputWrapperBool ( {
                        BooleanInputDefinition.DocumentPull = (fun x -> x.IsCustomerSecured) ; 
                        PropDisplayName = "Is Customer Secured"
                        DocumentUpdate =  (fun  x doc -> {doc with IsCustomerSecured = x}
                        ) } )
                    
                ]
            {
                SceenDefinition.InitialDocument = qryResult; 
                ChidrenViews = viewTree;   
                CmdTransformers = [saveCmd]; 
            }
            
        member this.ShowEditBasicCustomerInfoScreen (newCustomerApplicationQryParam:NewCustomerApplicationQryParam) = 
            let view = this.BuildEditBasicCustomerInfoScreen newCustomerApplicationQryParam
            this.ViewManager.DisplayScreen view

        // TODO add drop down query binding that have a callback to generate the URL and to transform results back to field