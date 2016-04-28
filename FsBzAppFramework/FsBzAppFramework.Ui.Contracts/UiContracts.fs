namespace FsBzAppFramework.Ui.Contracts

open System

module UiTypes =
    type UiLabel = string
    type UiFormat = string
    type UiTextValue = string
    type UiDateValue = DateTime
    type UiDateTimeValue  = DateTime
    type UiTimeValue  = DateTime
    type UiIntValue = int
    type UiDecimalValue = decimal
    type UiElementId =string
    type UiErrMsgs = string seq
    type UiImplementation = (unit -> unit)

    
    type IUiReadOnlyTextElement =
        abstract member GetValue : unit->UiTextValue
    type IUiReadOnlyNumberElement =
        abstract member GetValue : unit->UiDecimalValue
    type IUiReadOnlyDateElement =
        abstract member GetValue : unit->UiDateTimeValue

    type IUiElement =
        abstract member SetErrors : UiErrMsgs -> unit


    type IUiTextElement =
        inherit IUiElement
        abstract member GetValue : unit->UiTextValue
    type IUiDateElement =
        inherit IUiElement
        abstract member GetValue : unit->UiDateValue
    type IUiDateTimeElement =
        inherit IUiElement
        abstract member GetValue : unit->UiDateTimeValue

    type IUiTimeElement =
        inherit IUiElement
        abstract member GetValue : unit->UiTimeValue

    type IUiIntElement =
        inherit IUiElement
        abstract member GetValue : unit->UiIntValue

    type IUiDecimalElement =
        inherit IUiElement
        abstract member GetValue : unit->UiDecimalValue
    
    type UiFieldsSet = {Fields: IUiTextElement seq}
    

    


//// from 2016-04-28 rethinking
// The builders still build, but their implementation is handled by wpf front end
//  they do returnt the element abstraction (IUiTextElement, IUiDateElement, etc)
//  whihc is then assigned to the strongly typed user defined UI type by the User Ui confiurator
module UiConfiguration =
    open UiTypes
    // an instance of UiBuilder is created by the front end and its funcs injected using the 
    //  concrete implementation
    type UiBuilder  = 
        {
            BuildReadOnlyText : ( UiLabel-> UiTextValue -> IUiReadOnlyTextElement)
            BuildReadOnlyNumber : ( UiLabel-> UiDecimalValue -> UiFormat -> IUiReadOnlyNumberElement)
            BuildReadOnlyDate : ( UiLabel-> UiDateTimeValue -> UiFormat -> IUiReadOnlyDateElement)

            BuildTextInput : ( UiLabel-> UiTextValue -> IUiTextElement)
            BuildDateInput : ( UiLabel-> UiTextValue -> IUiDateElement)
            BuildDateTimeInput : ( UiLabel-> UiTextValue -> IUiDateTimeElement)
            BuildTimeInput : ( UiLabel-> UiTextValue -> IUiTimeElement)
            BuildIntInput : ( UiLabel-> UiTextValue -> IUiIntElement)
            BuildDecimalInput : ( UiLabel-> UiTextValue -> IUiDecimalElement)

        }
    
    type MessageRaiser<'a> = unit -> 'a

    type NavBuilder =
        {
            BuildBtn : (UiLabel -> (UiBuilder -> unit) -> unit)        
        }

    let TestIdea () =
        let wpfTextInputBuilder id label initVal =  
            let newWidget = {
                new IUiTextElement with
                    member x.SetErrors(a) = 
                        ()
                    member x.GetValue() = 
                        "0"
            } //newWidget built by Wpf using the 3 params, the newWidget implements IUiTextElement
            newWidget
        wpfTextInputBuilder "b;ah" "bla" "blka"