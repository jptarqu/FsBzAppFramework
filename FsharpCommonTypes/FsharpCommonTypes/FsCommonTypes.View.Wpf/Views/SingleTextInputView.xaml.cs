using Common.ViewModels;
using FsCommonTypes.View.Wpf.Views;
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
    /// Interaction logic for SingleTextInputView.xaml
    /// </summary>
    public partial class SingleTextInputView : UserControl
    {
        public SingleTextInputView()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var input = e.NewValue as IMaskedInput;
            if (input != null)
            {
                MaskedText.SetMask(ValueTextBox, input.Mask);
                //MaskedText.SetPromptChar(ValueTextBox, '?');
            }
        }
    }
}
