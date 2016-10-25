namespace FsharpCommonTypes
   
type SummaryError =
    { ErrorCode:string; Description:string;  }

type PropertyError =
    { ErrorCode:string; Description:string; PropertyName:string; }
    member this.DisplayAsPropErrorString () =
        sprintf "%s: %s"  this.Description this.PropertyName
    member this.PropOrEntityName = if this.PropertyName = "" then "Entity" else this.PropertyName 
 
type QueryResult<'QueryType> =
    { Content: 'QueryType; Errors: PropertyError seq}

type CommandResult =
    { Message:string; Errors: PropertyError seq}

type CommandDefinition<'ModelType>  =
    { CmdName:string; CmdExecuter: 'ModelType->Async<CommandResult>}

    
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module CommandDefinition =
    let AlwaysSucessCmd doc = 
        async {
            return { CommandResult.Errors = Seq.empty; CommandResult.Message = "Thanks!" }
        }
    let CancelCmdDefinition = 
        { CommandDefinition.CmdName = "Cancel"; CommandDefinition.CmdExecuter = AlwaysSucessCmd}

type ErrorMessage = string
    
type IPropValidator<'ParentType> =
    abstract member GetValidationErrors : 'ParentType->seq<PropertyError>
// with types like this, validation occurs at the moment of creation. A prop is either valid or invlid based on 
//  the construction parameters
// do we need     [<CLIMutable>] here??

type BzProp<'Primitive> = 
    | ValidProp of 'Primitive
    | InvalidProp of 'Primitive * seq<ErrorMessage>
    with 
        member x.GetValidationErrors() = match x with 
                                             | ValidProp _ -> Seq.empty
                                             | InvalidProp (badPrimitive, errors) -> errors 
        member x.ToPrimitive() = match x with 
                                             | ValidProp validPrimitive -> Some validPrimitive
                                             | InvalidProp (badPrimitive, errors) -> None  
        override x.ToString() = match x with 
                                             | ValidProp validPrimitive -> validPrimitive.ToString()
                                             | InvalidProp (badPrimitive, errors) -> ""  
        
        

type PropFactoryMethod<'InputPrimitive, 'Primitive> =
    'InputPrimitive->BzProp<'Primitive>
    
type PropDefinition<'ParentType, 'Primitive, 'InputPrimitive> = 
    {Name: string; 
    Factory: PropFactoryMethod<'InputPrimitive, 'Primitive>; 
    Setter: 'ParentType->BzProp<'Primitive>->'ParentType; 
    Getter: 'ParentType -> BzProp<'Primitive>;
    PropToInput: BzProp<'Primitive> -> 'InputPrimitive  }
    with 
        member this.GetValidationErrors doc =
                let prop = (this.Getter doc)
                prop.GetValidationErrors() 
                |> Seq.map (fun errMsg ->  
                    { PropertyError.ErrorCode = "PropValidation"; Description = errMsg; PropertyName = this.Name }) 
            
        interface IPropValidator<'ParentType> with
            member this.GetValidationErrors doc =
                this.GetValidationErrors doc