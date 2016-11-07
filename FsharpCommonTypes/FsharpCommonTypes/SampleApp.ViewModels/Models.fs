namespace Common.ViewModels

open FsharpCommonTypes
open Common.ViewModels.Interfaces

[<CLIMutable>]
type SaleModel = 
    {
        Name: BusinessTypes.LongNameType; 
        SalesRegion: BusinessTypes.IdNumberType; 
        SalesDate : BusinessTypes.PastDateTimeType;
        QuantityId : BusinessTypes.IdNumberType;
    } 
    with
        static member DefinitionName = 
            { PropDefinition.Name ="Name"; Factory = BusinessTypes.LongName; Setter = (fun d v -> {d with SaleModel.Name = v }); Getter = (fun d -> d.Name ); 
            PropToInput = BusinessTypes.ToPrimitive  }
        static member DefinitionSalesRegion = 
            { PropDefinition.Name ="Sales Region"; Factory = BusinessTypes.IdNumber; Setter = (fun d v -> {d with SaleModel.SalesRegion = v }); Getter = (fun d -> d.SalesRegion );
            PropToInput = BusinessTypes.ToPrimitive }  
        static member DefinitionSalesDate = 
            { PropDefinition.Name ="Sales Date"; Factory = BusinessTypes.PastDateTime; Setter = (fun d v -> {d with SaleModel.SalesDate = v }); Getter = (fun d -> d.SalesDate );
            PropToInput = BusinessTypes.ToPrimitive 
            }
        static member DefinitionQuantityId = 
            { PropDefinition.Name ="Quantity Id"; Factory = BusinessTypes.IdNumberFromStr; Setter = (fun d v -> {d with SaleModel.QuantityId = v }); Getter = (fun d -> d.QuantityId );
            PropToInput = BusinessTypes.ToPrimitiveStr }  
        interface InterfaceTypes.ICanValidate with 
            member this.GetValidationErrors () = 
                let isObjValid () = 
                    if this.Name = BusinessTypes.LongName "Looper" then  
                        seq [ { PropertyError.ErrorCode ="YY"; Description ="TestError"; PropertyName=""; } ] 
                    else 
                        Seq.empty
                [ SaleModel.DefinitionName.GetValidationErrors(this) ; 
                   SaleModel.DefinitionSalesRegion.GetValidationErrors(this);
                   SaleModel.DefinitionSalesDate.GetValidationErrors(this);
                   SaleModel.DefinitionQuantityId.GetValidationErrors(this);
                   isObjValid(); ] 
                |> Seq.collect id
     
[<CLIMutable>]
type SaleModelReadOnly = 
    {Name: string; SalesRegion: BusinessTypes.IdNumberType; SalesDate : string} 

[<CLIMutable>]
type SaleListModel = SelectableList<SaleModelReadOnly>
//    {Docs: SaleModelReadOnly seq; SelectedItem: Option<SaleModelReadOnly>} 
//    with
//        interface InterfaceTypes.ICanValidate with 
//            member this.GetValidationErrors () = Seq.empty
                       
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module SaleModel = 
    let ifNull (x:System.Nullable<'a>) v =
        match x.HasValue with
        | false -> v
        | true -> x.Value

    let Empty =
        let now = System.DateTime.Now
        let model ={SaleModel.Name= BusinessTypes.LongName "Alabama" ; SalesRegion = BusinessTypes.IdNumber 1; SalesDate =  BusinessTypes.PastDateTime now; QuantityId = BusinessTypes.IdNumber -1} 
        model
    let ConvertFromReadOnly (readOnly:SaleModelReadOnly) =
        //let dt = defaultArg readOnly.SalesDate (System.DateTime.Today.AddDays(float 1))
//        let dt = ifNull readOnly.SalesDate (System.DateTime.Today.AddDays(float 1))
        let dt = ConversionHelpers.tryParseDateWithDefault readOnly.SalesDate  (System.DateTime.Today.AddDays(float 1))
        {SaleModel.Name= BusinessTypes.LongName readOnly.Name ; 
        SalesRegion =  readOnly.SalesRegion; 
        SalesDate =  BusinessTypes.PastDateTime dt; 
        QuantityId = readOnly.SalesRegion} 

