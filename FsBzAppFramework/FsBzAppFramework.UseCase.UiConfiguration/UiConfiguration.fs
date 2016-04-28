namespace FsBzAppFramework.UseCase.UiConfiguration

module UiConfiguration =
    open FsBzAppFramework.Ui.Contracts.UiTypes
    open FsBzAppFramework.Ui.Contracts.UiConfiguration
    open UiTypes
    open FsBzAppFramework.UserDefined.Core.QueryRequestTypes
    open FsBzAppFramework.UserDefined.Core.QueryResultTypes
    open FsBzAppFramework.UserDefined.Core

    //childUiBuilder already represents the builder within the new "child screen"
    let BuildCustomerPersonalUpdateUi (childUiBuilder:UiBuilder) (model:CustomerQryResult) =
        let ui:CustomerPersonalUpdateUi = { 
                    Name = childUiBuilder.BuildTextInput "Name" model.Name
                    Id = childUiBuilder.BuildReadOnlyNumber "Id" (decimal model.Id) "0"
                    Balance = childUiBuilder.BuildReadOnlyNumber "Balance" model.Balance "C2"
                 }
        ui
 

    type ServerQueryService =
        {
            CustomerPersonalUpdateQry : QueryServiceTypes.CustomerQry
        }

    // whatever calls BuildChildUiFromMessage is already calling in async mode in non ui thread??
    let BuildChildUiFromMessage (qryService:ServerQueryService) (childUiBuilder:UiBuilder) (msg:QryRequest) =
        match msg with
        | CustomerQryRequest r -> 
            let model = qryService.CustomerPersonalUpdateQry r  //call server async and build ui from model returned
            let childUi = BuildCustomerPersonalUpdateUi childUiBuilder model
            childUi


    //called from App.cs??
    type BzUiConfiguration() =
        let qryService = { CustomerPersonalUpdateQry = FsBzAppFramework.UseCase.Query.ObtainPersonalData }//The only reason we are referencing FsBzAppFramework.UseCase is becuase this is a test
        
        member this.BuildChildUiFromMessage(childUiBuilder:UiBuilder, msg:QryRequest) =
            BuildChildUiFromMessage qryService childUiBuilder msg |> ignore
        
        member this.BuildNavButtons(navBuilder:NavBuilder) =
            do 
                navBuilder.BuildBtn "Get Juan" (fun childUiBuilder -> 
                                                    this.BuildChildUiFromMessage(childUiBuilder, CustomerQryRequest { Id = 5 } ))
             