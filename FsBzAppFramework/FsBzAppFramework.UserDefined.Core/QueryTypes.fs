namespace FsBzAppFramework.UserDefined.Core

// A query is an encapsulation of the request for a query
module QueryRequestTypes =
    type CustomerQryRequest = { Id: int }

    
module QueryResultTypes =
    type CustomerQryResult = { Name: string; Id: int; Balance: decimal }

