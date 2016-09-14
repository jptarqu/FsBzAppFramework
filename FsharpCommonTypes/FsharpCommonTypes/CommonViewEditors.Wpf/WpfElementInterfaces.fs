namespace CommonViewEditors.Wpf

module WpfElementInterfaces =
    open CommonViewEditors
    open CommonValidations
    
    type TextChangedCallback<'FieldType> = ('FieldType->string->unit)

    type ITextField =
        abstract member SetValue:string->unit
        abstract member GetValue:unit->string
        abstract member SetChangeCallback:TextChangedCallback<ITextField>->unit
        abstract member SetErrors:string seq->unit


    type IIntegerField =
        abstract member SetValue:int->unit
        abstract member GetValue:unit->int
        abstract member ChangeCallback:TextChangedCallback<IIntegerField>
        abstract member SetErrors:string seq->unit

    type IDecimalField =
        abstract member SetValue:decimal->unit
        abstract member GetValue:unit->decimal
        abstract member ChangeCallback:TextChangedCallback<IDecimalField>
        abstract member SetErrors:string seq->unit
        
    type IChoicesField =
        abstract member SetValue:string->unit
        abstract member GetValue:unit->string
        abstract member GetTextEntered:unit->string
        abstract member SetChangeCallback:TextChangedCallback<IChoicesField>->unit
        abstract member SetErrors:string seq->unit
        
    type IBooleanField =
        abstract member SetValue:bool->unit
        abstract member GetValue:unit->bool
        abstract member SetChangeCallback:TextChangedCallback<IBooleanField>->unit
        abstract member SetErrors:string seq->unit
    
    type IElementService =
        abstract member AddTextField:string->string->int->int->ITextField
        abstract member AddIntegerField:string->TextChangedCallback<IIntegerField>->int->int->int->IIntegerField
        abstract member AddDecimalField:string->TextChangedCallback<IDecimalField>->decimal->decimal->decimal->IDecimalField
        abstract member AddChoicesField:string->IChoicesField
        abstract member AddBooleanField:string->IBooleanField

    type IDataService =
        abstract member CallExternalCommand<'CmdType> : string->'CmdType->CommandResult
