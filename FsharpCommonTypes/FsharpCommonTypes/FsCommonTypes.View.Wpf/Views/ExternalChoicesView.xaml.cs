using Common.ViewModels;
using CommonViewEditors;
using FsCommonTypes.View.Wpf;
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

namespace FsCommonTypes.Views
{
    /// <summary>
    /// Interaction logic for ExternalChoicesView.xaml
    /// </summary>
    public partial class ExternalChoicesView : UserControl
    {
        private IExternalChoicesQry<int> viewModel;

        public ExternalChoicesView()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            viewModel = (IExternalChoicesQry<int>) e.NewValue ;
            choicesBox.Provider = new ChoiceItemsFuncProvider<int>(viewModel.QueryExecutor);
        }

        private void choicesBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            //viewModel.QueryExecutor(e.Text);
        }
    }
}
