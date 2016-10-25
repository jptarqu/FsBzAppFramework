using Common.ViewModels;
using Common.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FsCommonTypes.View.Wpf.Builders;

namespace FsCommonTypes.View.Wpf.Views
{
    /// <summary>
    /// Interaction logic for DocView.xaml
    /// </summary>
    public partial class DocView : UserControl
    {

        public DocView()
        {
            InitializeComponent();
        }
        

        //private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    var newDoc = e.NewValue as IDocViewModel;
        //    if (newDoc != null)
        //    {
        //        fieldsContainer.SetViewComponent(newDoc.GetRootView());
        //    }
        //}

        internal void SetDoc(IDocViewModel docModel, ViewBuildersCollection _viewBuilders, string ScreenId)
        {
            DialogHostContainer.Identifier = ScreenId;
            fieldsContainer.SetViewComponent(docModel.GetRootView(), _viewBuilders);
            this.DataContext = docModel;
        }
    }
}
