namespace FsBzAppFramework.Ui.Contracts

open System

type IUiTextElement =
    abstract member OnChange : 
        callback:(string->unit) -> unit
        
type IUiDateElement =
    abstract member OnChange : 
        callback:(DateTime->unit) -> unit
        
type IUiDateTimeElement =
    abstract member OnChange : 
        callback:(DateTime->unit) -> unit
        
type IUiTimeElement =
    abstract member OnChange : 
        callback:(DateTime->unit) -> unit
        
type IUiIntElement =
    abstract member OnChange : 
        callback:(int->unit) -> unit
        
type IUiDecimalElement =
    abstract member OnChange : 
        callback:(decimal->unit) -> unit


type UiElement =
    | IUiTextElement of IUiTextElement
    | IUiDateElement of IUiDateElement
    | IUiTimeElement of IUiTimeElement
    | IUiDateTimeElement of IUiDateTimeElement
    | IUiIntElement of IUiIntElement
    | IUiDecimalElement of IUiDecimalElement
    
type UiFieldsSet = {Fields: UiElement seq}

type IUiBuilder =
    abstract member FieldsSet: 
        label:string -> fields:(UiElement seq) -> UiFieldsSet

    abstract member TextInput: 
        label:string -> initValue:string -> errMsg : string -> IUiTextElement 
        
    abstract member DateInput: 
        label:string -> initValue:DateTime -> errMsg : string -> IUiDateElement 
        
    abstract member TimeInput: 
        label:string -> initValue:DateTime -> errMsg : string -> IUiDateElement 
        
    abstract member DateTimeInput: 
        label:string -> initValue:DateTime -> errMsg : string -> IUiDateElement 
        
    abstract member IntInput: 
        label:string -> initValue:int -> errMsg : string -> IUiIntElement 

    abstract member DecimalInput: 
        label:string -> initValue:decimal -> errMsg : string -> IUiDecimalElement 
    
