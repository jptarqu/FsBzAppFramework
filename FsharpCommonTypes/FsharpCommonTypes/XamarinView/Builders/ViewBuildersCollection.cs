using Common.ViewModels;
using Common.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FsCommonTypes.View.XamarinForms.Builders;
using Xamarin.Forms;
using FsharpCommonTypes.View.XamarinForms.Views;

namespace FsCommonTypes.View.XamarinForms.Builders
{
    public class ViewBuildersCollection
    {
        private readonly LinkedList<ViewBuilder> _builders;

        public ViewBuildersCollection()
        {
            _builders = new LinkedList<ViewBuilder>();
        }

        public void AddDefaultViewBuilders()
        {
            _builders.AddLast(new ViewBuilder(SingleInputViewModelModule.UIHints.SingleTextInput,
                (childView) => CreateSetDataContext(() => new SingleTextInputView(), childView)
            ));
            _builders.AddLast(new ViewBuilder(SingleInputViewModelModule.UIHints.DateInput,
                (childView) => CreateSetDataContext(() => new DateInputView(), childView)
            ));
            //_builders.AddLast(new ViewBuilder(ExternalChoicesViewModelModule.UIHints.ExternalChoices,
            //    (childView) => CreateSetDataContext(() => new ExternalChoicesView(), childView)
            //));
            //_builders.AddLast(new ViewBuilder(SimpleChoicesViewModelModule.UIHints.SimpleChoices,
            //    (childView) => CreateSetDataContext(() => new SimpleChoicesView(), childView)
            //));
            //_builders.AddLast(new ViewBuilder(SingleInputViewModelModule.UIHints.IntInput,
            //    (childView) => CreateSetDataContext(() => new IntInputView(), childView)
            //));
            //_builders.AddLast(new ViewBuilder(SingleInputViewModelModule.UIHints.ReadOnlyText,
            //    (childView) => CreateSetDataContext(() => new ReadOnlyTextView(), childView)
            //));

        }

        public void AddBuilderLast(ViewBuilder newBuilder)
        {
            _builders.AddLast(newBuilder);
        }
        public void AddBuilderFirst(ViewBuilder newBuilder)
        {
            _builders.AddFirst(newBuilder);
        }
        public ViewBuilder GetBuilder(string uiHint)
        {
            return _builders.FirstOrDefault(b => b.UiHint == uiHint);
        }

        private Xamarin.Forms.View CreateSetDataContext<TView>(Type viewType, IViewComponent childView) where TView : Xamarin.Forms.View
        {

            var newCtrl = Activator.CreateInstance<TView>();
            newCtrl.BindingContext = childView;
            return newCtrl;
        }
        private Xamarin.Forms.View CreateSetDataContext<TView>(Func<TView> constructor, IViewComponent childView) where TView : Xamarin.Forms.View
        {

            var newCtrl = constructor();
            newCtrl.BindingContext = childView;
            return newCtrl;
        }
    }
}
