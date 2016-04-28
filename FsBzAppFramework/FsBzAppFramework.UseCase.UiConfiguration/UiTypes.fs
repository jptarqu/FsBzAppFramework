namespace FsBzAppFramework.UseCase.UiConfiguration

//All possible operations on the Business Types
module UiTypes =
    open FsBzAppFramework.Ui.Contracts.UiTypes

    type CustomerPersonalUpdateUi = 
        {
            Name: IUiTextElement
            Id: IUiReadOnlyNumberElement
            Balance: IUiReadOnlyNumberElement
        }

    type BzScreen = 
        | CustomerPersonalUpdateUi of CustomerPersonalUpdateUi


