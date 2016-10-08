namespace FsharpCommonTypes

module InterfaceTypes =

    type ConstraintFieldDefinition =
        | TextPropertyDefinition of TextType.TextPropertyDefinition
        | IntegerPropertyDefinition of IntegerTypes.IntegerPropertyDefinition
        | DecimalPropertyDefinition of DecimalTypes.DecimalPropertyDefinition

    type ITextEditableProperty = 
        abstract GetFieldDefinition : unit -> TextType.TextPropertyDefinition

    type IConvertibleFromString<'PropDefType> =
        abstract SetValueFromText : string -> 'PropDefType

    type IIntegerEditableProperty = 
        abstract GetFieldDefinition : unit -> IntegerTypes.IntegerPropertyDefinition

    type IDecimalEditableProperty = 
        abstract GetFieldDefinition : unit -> DecimalTypes.DecimalPropertyDefinition

    type ICanValidate = 
        abstract member GetValidationErrors : unit -> PropertyError seq 

    let CollectAllError (validatingList: PropertyError seq seq) =
        validatingList  |> Seq.collect (fun x -> x )  