using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.ViewModels;
using Common.ViewModels.Interfaces;
using FsCommonTypes.View.XamarinForms.Builders;
using Xamarin.Forms;

namespace FsharpCommonTypes.View.XamarinForms.Views
{
    public partial class DocView : StackLayout
    {
        public DocView()
        {

        }

        public void SetDoc(IDocViewModel docModel, ViewBuildersCollection _viewBuilders, string ScreenId)
        {
            this.BindingContext = docModel;
            fieldsContainer.SetViewComponent(docModel.GetRootView(), _viewBuilders);
        }
    }
}
