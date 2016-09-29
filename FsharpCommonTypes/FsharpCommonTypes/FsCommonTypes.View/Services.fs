namespace CommonViewEditors

open FsharpCommonTypes.InterfaceTypes
open CommonValidations
open FsharpCommonTypes

type DocumentUpdate< 'PrimitiveType, 'ParentType> = ('ParentType -> 'PrimitiveType ->   'ParentType)

type DocumentPull<'ParentType,'PrimitiveType> = ('ParentType  -> 'PrimitiveType)

type ScopePull< 'ScopeType, 'ParentType> = ('ParentType  -> 'ScopeType)



type ExternalChoicesQueryResult<'Result> = {ResultId: string; ResultLabel: string; Content:'Result  }

type ExternalChoicesQueryExecutor<'ParentType, 'Result > = 'ParentType -> string -> seq<ExternalChoicesQueryResult<'Result>>

type IIntExternalChoicesQry =
    abstract QueryExecutor:string->seq<ExternalChoicesQueryResult<int>> //unit //seq<ExternalChoicesQueryResult<int>>

type IntExternalChoicesInputDefinition< 'ViewModel > = 
    {
        PropDisplayName: string; 
        DocumentPull: DocumentPull<'ViewModel, int > ; 
        DocumentUpdate: DocumentUpdate< int, 'ViewModel> ; 
        ExternalChoicesQueryExecutor: ExternalChoicesQueryExecutor< 'ViewModel, int > ; 
    }

type ExternalChoicesInputDefinition< 'ViewModel, 'Result > = 
    {
        PropDisplayName: string; 
        DocumentPull: DocumentPull<'ViewModel, string > ; 
        DocumentUpdate: DocumentUpdate< ExternalChoicesQueryResult<'Result>, 'ViewModel> ; 
        ExternalChoicesQueryExecutor: ExternalChoicesQueryExecutor< 'ViewModel, 'Result > ; 
    }
    
type ConstraintedInputDefinition< 'PropType , 'ParentType> = 
    {PropDisplayName: string; DocumentPull: DocumentPull<'ParentType,'PropType> ; DocumentUpdate: DocumentUpdate< 'PropType, 'ParentType>; Constraint: ConstraintFieldDefinition }
    
type TextInputDefinition<  'ParentType> = 
    {PropDisplayName: string; DocumentPull: DocumentPull<'ParentType,string> ; DocumentUpdate: DocumentUpdate< string, 'ParentType>; Constraint: TextType.TextPropertyDefinition }
    
type BooleanInputDefinition<  'ParentType> = 
    {PropDisplayName: string; DocumentPull: DocumentPull<'ParentType,bool> ; DocumentUpdate: DocumentUpdate< bool, 'ParentType> }


type IViewComponent<'ViewModel> =
    abstract OnDocUpdated<'ViewModel> : 'ViewModel -> unit
    abstract IsValid:unit->bool
    abstract Init<'ViewModel> : 'ViewModel -> unit
    abstract Label:string
    abstract UiHint:string

type IScreen<'ViewModel> =
    abstract UpdateDoc<'ViewModel> : 'ViewModel -> unit
    
type ExternalViewCommandDefinition<'ViewModel, 'CmdType> = 
    {CmdName: string; CmdTransformer : 'ViewModel->'CmdType; CmdAddress: string;}

type IViewCommand<'ViewModel> = 
    abstract Name:string
    abstract Execute:'ViewModel->CommandResult
    abstract CanExecute:'ViewModel->bool
        
type InputDefinition< 'PropType , 'ParentType, 'Result> =
    | ConstraintedInputDefinition of ConstraintedInputDefinition< 'PropType , 'ParentType>

type ViewContainerDefinition<'ViewModel> =
    {Name: string; ChidrenViews: IViewComponent<'ViewModel>;}

type ISceenBuilderService  =
    abstract CreateExternalCommand<'ViewModel, 'CmdType> : ExternalViewCommandDefinition<'ViewModel, 'CmdType>->IViewCommand<'ViewModel>
    abstract CreateInputWrapperBool< 'ViewModel> : BooleanInputDefinition< 'ViewModel>->IViewComponent<'ViewModel>
    abstract CreateInputWrapperText< 'ViewModel> : TextInputDefinition< 'ViewModel>->IViewComponent<'ViewModel>
    abstract CreateInputWrapperChoices< 'ViewModel, 'Result> : ExternalChoicesInputDefinition<'ViewModel , 'Result>->IViewComponent<'ViewModel>
    abstract CreateContainerWrapper< 'ViewModel> : ViewContainerDefinition<'ViewModel>->IViewComponent<'ViewModel>

type SceenDefinition<'ViewModel>  =
    {InitialDocument: 'ViewModel; ChidrenViews: IViewComponent<'ViewModel> seq;   CmdTransformers: IViewCommand<'ViewModel> seq; }
    
type IScreenManager =
    abstract DisplayScreen< 'ViewModel> :   (SceenDefinition<'ViewModel>) -> unit

   