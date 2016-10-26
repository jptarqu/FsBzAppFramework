namespace CommonValidations
   
[<CLIMutableAttribute>]
type SummaryError =
    { ErrorCode:string; Description:string;  }

[<CLIMutableAttribute>]
type PropertyError =
    { ErrorCode:string; Description:string; PropertyName:string; }
    

type RequestError = 
    | ValidationErrors of PropertyError list
    | ValidationError of PropertyError 
    | FatalError of SummaryError
    | NoDataFound of SummaryError

module ErrorHelpers =
    let MapToPropErrors requestErrors  =
        let flattenErrors = requestErrors |> Seq.collect (fun requestError ->
                                                            match requestError with  
                                                            | ValidationErrors errs -> errs
                                                            | ValidationError err -> [ err ]  
                                                            | FatalError summaryError -> [ { ErrorCode = summaryError.ErrorCode; Description =summaryError.Description; PropertyName = ""; } ]
                                                            | NoDataFound summaryError -> [ { ErrorCode = summaryError.ErrorCode; Description =summaryError.Description; PropertyName = ""; } ]
                                                            )
        flattenErrors
        
[<CLIMutableAttribute>]
type ApiResultRendition<'a> = { Content : option<'a>; ReportableErrors: seq<PropertyError> }
module ApiResultHelpers =
    let SuccessApiResult content =
        { Content =  Some content; ReportableErrors=  Seq.empty }
    let FailureApiResult content errors =
        { Content =  None; ReportableErrors=  errors }