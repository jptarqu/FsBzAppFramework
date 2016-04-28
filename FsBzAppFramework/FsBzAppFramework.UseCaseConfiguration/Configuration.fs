namespace FsBzAppFramework.UseCaseConfiguration

module BzConfiguration =
    open FsBzAppFramework.UserDefined.Core.QueryRequestTypes
    open FsBzAppFramework.UseCase

    let ReceiveMessage (msg:QryRequest) =
        match msg with
        | CustomerQryRequest r -> Query.ObtainPersonalData r // maybe this configuration code is running in server??
