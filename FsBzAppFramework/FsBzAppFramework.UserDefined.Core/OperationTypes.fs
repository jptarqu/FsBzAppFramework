namespace FsBzAppFramework.UserDefined.Core

//All possible operations on the Business Types
module OperationTypes =
//Try to use domain ubiquitous language instead of CRUD language??
    type Operation =
        | OpCreateException
        | OpTransaction 

