namespace FsharpCommonTypes.Business

module Queries =
    open Chessie.ErrorHandling

    let GetFromDataPull (dataPull:unit->Async<'RowType list>) () =
         asyncTrial {
                let! rows = dataPull()
                return rows |> List.toSeq
            }

    let GetFromDataPullAndApply (dataPull:unit->Async<'RowType list>) (funcsToApply:('RowType list->'RowType list) seq) () =
         asyncTrial {
                let! rows = dataPull()
                let rows = funcsToApply |> Seq.fold (fun prevRows currFunc -> currFunc prevRows)  rows 
                return rows |> List.toSeq
            }

module Commands =
    open Chessie.ErrorHandling
    open CommonValidations

//    let RunValidateFuncs (input:'A) (funcs:('A->PropertyError) seq) =
//        let result = input
//                        |> validateAcctNumber 
//                        |> bind validateAcctType
//                        |> bind validateTransactionID
//                        |> failOnWarnings 
//        match result with 
//        | Fail errs -> fail (ValidationErrors errs)
//        | Pass _ -> pass input

//    let UpdateAsync otherProcessing (dataSave:'Command->Async<int option>) (record:'Command) =
//        asyncTrial {
//                
//            let! validatedCmd = RunValidateFuncs record
//            let! idReturned = dataSave validatedCmd
//            otherProcessing()
//            return finalId
//            }
