namespace TypesPlayground


module QueryParamTypes =
    type NewCustomerApplicationQryParam = { CustomerApplicationId: string }
    type GetCustomerTypesQryParam = { CustomerTypeName: string }

module QueryTypes =
    open Region
    open FsharpCommonTypes.BusinessTypes


    type RegionQuery = 
        {Region: SalesRegion}


        

