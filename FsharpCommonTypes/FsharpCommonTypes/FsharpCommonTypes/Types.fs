namespace CommonValidations
   
type SummaryError =
    { ErrorCode:string; Description:string;  }

type PropertyError =
    { ErrorCode:string; Description:string; PropertyName:string; }
    member this.DisplayAsPropErrorString () =
        sprintf "%s: %s"  this.Description this.PropertyName
 
type QueryResult<'QueryType> =
    { Content: 'QueryType; Errors: PropertyError seq}

type CommandResult =
    { Message:string; Errors: PropertyError seq}