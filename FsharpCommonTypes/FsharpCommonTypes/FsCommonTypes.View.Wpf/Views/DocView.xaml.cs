using Common.ViewModels;
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

        public void SetDoc<ParentType>(DocViewModel<ParentType>  newDoc ) where ParentType : FsharpCommonTypes.InterfaceTypes.ICanValidate
        {
            this.DataContext = newDoc;
            fieldsContainer.SetViewComponent(newDoc.GetRootView());
        }
    }
}
