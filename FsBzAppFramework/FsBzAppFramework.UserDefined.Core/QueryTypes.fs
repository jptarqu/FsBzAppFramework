namespace FsBzAppFramework.UserDefined.Core

// A query is an encapsulation of the request for a query
module QueryRequestTypes =
    type CustomerQryRequest = { Id: int }

    type QryRequest =
        | CustomerQryRequest of CustomerQryRequest

    
module QueryResultTypes =
    type CustomerQryResult = { Name: string; Id: int; Balance: decimal }

module QueryServiceTypes =
    open QueryRequestTypes
    open QueryResultTypes

    type CustomerQry = CustomerQryRequest -> CustomerQryResult
