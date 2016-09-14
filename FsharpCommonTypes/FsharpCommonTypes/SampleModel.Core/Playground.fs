namespace TypesPlayground


module Region = 
    open FsharpCommonTypes.BusinessTypes
    open FsharpCommonTypes.InterfaceTypes

    [<CLIMutable>]
    type SalesRegion = 
        {Name: ShortName; SalesRegionId: IdNumber } 
        with
            interface ICanValidate with 
                member this.GetValidationErrors () = 
                   [ this.Name.GetValidationErrors(); this.SalesRegionId.GetValidationErrors() ] |> CollectAllError
                   
    [<CLIMutable>]
    type CustomerType = { CustomerTypeId: IdNumber; Name: ShortName }with
            interface ICanValidate with 
                member this.GetValidationErrors () = 
                   [ this.Name.GetValidationErrors(); this.Name.GetValidationErrors() ] |> CollectAllError
    
    [<CLIMutable>]
    type JobType = { JobTypeId:IdNumber; Name:ShortName } with
            interface ICanValidate with 
                member this.GetValidationErrors () = 
                   [ this.JobTypeId.GetValidationErrors(); this.Name.GetValidationErrors() ] |> CollectAllError
    
    [<CLIMutable>]
    type BasicCustomerInfo = 
        {CustomerApplicationId: ShortName; CustomerType: IdWrapper<CustomerType>; IsCustomerSecured: bool; AmtRequested: PositiveAmount; JobType:IdWrapper<JobType>  }with
            interface ICanValidate with 
                member this.GetValidationErrors () = 
                   [    this.CustomerApplicationId.GetValidationErrors(); 
                        this.CustomerType.GetValidationErrors(); 
                        this.AmtRequested.GetValidationErrors(); 
                        this.JobType.GetValidationErrors(); 
                   ] |> CollectAllError
