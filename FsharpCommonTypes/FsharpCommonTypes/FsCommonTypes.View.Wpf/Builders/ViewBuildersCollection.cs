using Common.ViewModels;
using Common.ViewModels.Interfaces;
using FsCommonTypes.View.Wpf.Views;
using FsCommonTypes.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FsCommonTypes.View.Wpf.Builders
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
            _builders.AddLast(new ViewBuilder(ExternalChoicesViewModelModule.UIHints.ExternalChoices,
                (childView) => CreateSetDataContext(() => new ExternalChoicesView(), childView)
            ));
            _builders.AddLast(new ViewBuilder(SimpleChoicesViewModelModule.UIHints.SimpleChoices,
                (childView) => CreateSetDataContext(() => new SimpleChoicesView(), childView)
            ));
            _builders.AddLast(new ViewBuilder(SimpleChoicesViewModelModule.UIHints.SimpleChoices,
                (childView) => CreateSetDataContext(() => new SimpleChoicesView(), childView)
            ));
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

        private UserControl CreateSetDataContext<TView>(Type viewType, IViewComponent childView) where TView : UserControl
        {

            var newCtrl = Activator.CreateInstance< TView>() ;
            newCtrl.DataContext = childView;
            return  newCtrl;
        }
        private UserControl CreateSetDataContext<TView>(Func<TView> constructor, IViewComponent childView) where TView : UserControl
        {

            var newCtrl = constructor();
            newCtrl.DataContext = childView;
            return newCtrl;
        }
    }
}
