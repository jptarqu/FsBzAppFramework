namespace CommonViewEditors.Js

module JsElementInterfaces =
    open CommonViewEditors
    open CommonValidations
    
    type TextChangedCallback<'FieldType> = ('FieldType->string->unit)

    type IHtmlTextField =
        abstract member SetValue:string->unit
        abstract member GetValue:unit->string
        abstract member ChangeCallback:TextChangedCallback<IHtmlTextField>
        abstract member SetErrors:string seq->unit


    type IHtmlIntegerField =
        abstract member SetValue:int->unit
        abstract member GetValue:unit->int
        abstract member ChangeCallback:TextChangedCallback<IHtmlIntegerField>
        abstract member SetErrors:string seq->unit

    type IHtmlDecimalField =
        abstract member SetValue:decimal->unit
        abstract member GetValue:unit->decimal
        abstract member ChangeCallback:TextChangedCallback<IHtmlDecimalField>
        abstract member SetErrors:string seq->unit
        
    type IHtmlChoicesField =
        abstract member SetValue:string->unit
        abstract member GetValue:unit->string
        abstract member GetTextEntered:unit->string
        abstract member SetChangeCallback:TextChangedCallback<IHtmlChoicesField>->unit
        abstract member SetErrors:string seq->unit
        
    type IHtmlBooleanField =
        abstract member SetValue:bool->unit
        abstract member GetValue:unit->bool
        abstract member SetChangeCallback:TextChangedCallback<IHtmlBooleanField>->unit
        abstract member SetErrors:string seq->unit
    
    type IHtmlElementService =
        abstract member AddTextField:string->TextChangedCallback<IHtmlTextField>->string->int->int->IHtmlTextField
        abstract member AddIntegerField:string->TextChangedCallback<IHtmlIntegerField>->int->int->int->IHtmlIntegerField
        abstract member AddDecimalField:string->TextChangedCallback<IHtmlDecimalField>->decimal->decimal->decimal->IHtmlDecimalField
        abstract member AddChoicesField:string->IHtmlChoicesField
        abstract member AddBooleanField:string->IHtmlBooleanField

    type IJsDataService =
        abstract member CallExternalCommand<'CmdType> : string->'CmdType->CommandResult