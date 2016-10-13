namespace Common.ViewModels

open FSharp.ViewModule
open FsharpCommonTypes


type SingleInputViewModel<'PrimitiveType, 'ParentType when 'PrimitiveType: equality>(propFactory:PropFactoryMethod<'PrimitiveType>,
                                                        refreshValFromDoc:'ParentType->BzProp<'PrimitiveType>, 
                                                        refreshDocFromVal:BzProp<'PrimitiveType>->'ParentType, // allow create new doc by sending the newly BzProp<'PrimitiveType>
                                                        pushUpdatedDoc: CommonViewEditors.IViewComponent<'ParentType> ->'ParentType->unit,
                                                        propName: string,
                                                        defaultValue: 'PrimitiveType) as self = 
    inherit ViewModelBase()
    
//    let mutable txtValue = defaultValue
    let mutable currErrors:seq<ErrorMessage> = Seq.empty
    
    //let validate () = 
    //    currErrors <- propConstraint.GetPropertyValidationErrors propName self.Value
    
    let getStrErrors = BusinessTypes.GetStrErrors propFactory

    let txtValue = self.Factory.Backing(<@ self.Value @>, defaultValue, getStrErrors)

    // ...
    
    let alertParentOfDocChg newVal =
        let newDoc = refreshDocFromVal newVal
        pushUpdatedDoc self newDoc

    member self.Value with get() = txtValue.Value 
                        and set value = 
                            if (value <> txtValue.Value) then
                                txtValue.Value <- value
                                let newPropState = propFactory value
                                alertParentOfDocChg newPropState // always send to doc, even if invalid state?

    member self.PropName with get() = propName

    interface CommonViewEditors.IViewComponent<'ParentType> with
        member this.Init<'ParentType> vm = 
            self.Value <- BusinessTypes.ToPrimitive (refreshValFromDoc vm)

        member this.OnDocUpdated<'ParentType> vm = 
            self.Value <- BusinessTypes.ToPrimitive (refreshValFromDoc vm)
            ()

        member this.Label = propName
        member this.UiHint = "SingleInput"

type OldSingleInputViewModel<'PrimitiveType, 'ParentType>(docPull:'ParentType->'PrimitiveType, 
                                                        docUpdate:string->'ParentType, 
                                                        pushUpdatedDoc: CommonViewEditors.IViewComponent<'ParentType> ->'ParentType->unit,
                                                        propConstraint: TextType.TextPropertyDefinition,
                                                        propName: string) as self = 
    inherit ViewModelBase()
    let defaultValue = ""
//    let mutable txtValue = defaultValue
    let mutable currErrors:seq<PropertyError> = Seq.empty
    
    let validate () = 
        currErrors <- propConstraint.GetPropertyValidationErrors propName self.Value

    let getValErrors newVal =
        validate()
        let strErrors = currErrors |> Seq.map (fun e -> e.Description) |> Seq.toList
        strErrors

    let txtValue = self.Factory.Backing(<@ self.Value @>, "", getValErrors)

    // ...
    let isValueValid = 
        currErrors |> Seq.isEmpty
    let alertParentOfDocChg newVal =
        let newDoc = docUpdate newVal
        pushUpdatedDoc self newDoc

    member self.Value with get() = txtValue.Value 
                        and set value = 
                            if (value <> txtValue.Value) then
                                txtValue.Value <- value
                                validate()
                                if (isValueValid) then
                                   alertParentOfDocChg txtValue.Value

    member self.PropName with get() = propName

    interface CommonViewEditors.IViewComponent<'ParentType> with
        member this.Init<'ParentType> vm = 
            self.Value <- (docPull vm).ToString()

        member this.OnDocUpdated<'ParentType> vm = 
            self.Value <- (docPull vm).ToString()
            ()

        member this.Label = propName
        member this.UiHint = "SingleInput"

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module SingleInputViewModel =
    let AddSingleInputViewModel (docViewModel:#Interfaces.IDocViewModel<'ParentType>) (propDef:PropDefinition<'ParentType, 'Primitive>)  =
        let docUpdateName = docViewModel.GetDocAccessor(propDef.Setter)
        let txtInput = SingleInputViewModel(propDef.Factory, propDef.Getter, docUpdateName,  docViewModel.UpdateDoc, propDef.Name, "")
        docViewModel.AddChild(txtInput)