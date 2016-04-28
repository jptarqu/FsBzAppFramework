namespace FsBzAppFramework.UseCase
// Funcs encapsulating the query logic to pull data to fill view

module Query =
    open FsBzAppFramework.UserDefined.Core.QueryRequestTypes
    open FsBzAppFramework.UserDefined.Core.QueryResultTypes

    //What call this? an actor? front end? backend?
    let ObtainPersonalData (rqst:CustomerQryRequest) =
        //declare what op will be perfomed on what resource here?? (for authorization)
        let model:CustomerQryResult = { Name = "Juan"; Id = rqst.Id; Balance = 5230.0m }
        model