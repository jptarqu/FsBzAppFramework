using System;
using Xamarin.Forms;
using Common.ViewModels;
using Common.ViewModels.Interfaces;

namespace FsCommonTypes.View.XamarinForms.Builders
{
    public class ViewBuilder
    {
        private readonly string _uiHint;
        private readonly Func<IViewComponent, Xamarin.Forms.View> _viewFactory;

        public ViewBuilder(string uiHint, Func<IViewComponent, Xamarin.Forms.View> viewFactory)
        {
            this._uiHint = uiHint;
            this._viewFactory = viewFactory;
        }

        public string UiHint
        {
            get
            {
                return _uiHint;
            }

        }

        public Func<IViewComponent, Xamarin.Forms.View> ViewFactory
        {
            get
            {
                return _viewFactory;
            }

        }
    }
}
