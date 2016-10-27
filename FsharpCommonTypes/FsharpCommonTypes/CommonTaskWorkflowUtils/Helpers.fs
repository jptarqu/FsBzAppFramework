namespace CommonTaskWorkflowUtils

module Helpers =
    open System.Net.Http
    open System.Net.Http.Headers
    open Chessie.ErrorHandling
    open CommonValidations
    open CommonValidations.ApiResultHelpers
    

    let ToTask asyncOp =
            asyncOp |> Async.StartAsTask 

    let valToTask value =
            async {
                return  value
                } |> Async.StartAsTask

    let getBinary (mediaType:string) bzFunc request = 
        async {
            let! (byteContent:(byte [])) = bzFunc request
            let res = new HttpResponseMessage()
            res.Content <- new ByteArrayContent(byteContent)
            res.Content.Headers.ContentType <- new MediaTypeHeaderValue(mediaType);
            return res
               } |> ToTask
    
        


    let getValue bzFunc request = 
        let resultAsync = bzFunc request |> Async.ofAsyncResult 
        async {
            let! result = resultAsync
            match result with
            | Pass goodContent -> return SuccessApiResult goodContent
            | Fail errors -> return FailureApiResult (ErrorHelpers.MapToPropErrors errors)
            | Warn (_,errors) -> return FailureApiResult (ErrorHelpers.MapToPropErrors errors)
            
        } |> ToTask
