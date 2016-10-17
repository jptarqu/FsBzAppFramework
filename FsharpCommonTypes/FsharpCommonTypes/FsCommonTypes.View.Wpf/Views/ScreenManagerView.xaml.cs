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
    /// Interaction logic for ScreenManagerView.xaml
    /// </summary>
    public partial class ScreenManagerView : UserControl
    {
        private ScreenManager viewModel; 
        public ScreenManagerView()
        {
            viewModel = new ScreenManager();
            DataContext = viewModel;
            InitializeComponent();
        }

        public ScreenManager GetScreenManager()
        {
            return this.viewModel;
        }

        public void SetManager()
        {

        }
    }
}
