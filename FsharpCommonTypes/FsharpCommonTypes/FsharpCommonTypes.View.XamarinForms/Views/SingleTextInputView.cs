using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace FsharpCommonTypes.View.XamarinForms.Views
{
    public class SingleTextInputView : ContentView
    {
        public SingleTextInputView()
        {
            Content = new Label { Text = "Hello ContentView" };
        }
    }
}
