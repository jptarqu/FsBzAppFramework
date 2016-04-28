namespace FsBzAppFramework.UseCase.UiConfiguration

module UiConfiguration =
    open FsBzAppFramework.Ui.Contracts.UiTypes
    open FsBzAppFramework.Ui.Contracts.UiConfiguration
    open UiTypes
    open FsBzAppFramework.UserDefined.Core.QueryRequestTypes
    open FsBzAppFramework.UserDefined.Core.QueryResultTypes

    let BuildCustomerPersonalUpdateUi (uiBuilder:UiBuilder) (model:CustomerQryResult) =
        let ui:CustomerPersonalUpdateUi = { 
                    Name = uiBuilder.BuildTextInput "Name" model.Name
                    Id = uiBuilder.BuildReadOnlyNumber "Id" (decimal model.Id) "0"
                    Balance = uiBuilder.BuildReadOnlyNumber "Balance" model.Balance "C2"
                 }
        ui
 
    let RelayToServerTodo (msg:QryRequest) =
        ()
    let ReceiveUiMessage (msg:QryRequest) =
        match msg with
        | CustomerQryRequest r -> 
            RelayToServerTodo msg |> ignore //call server