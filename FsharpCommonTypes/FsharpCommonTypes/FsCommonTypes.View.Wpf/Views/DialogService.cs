using Common.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FsCommonTypes.View.Wpf.Views
{
    public class DialogService : IDialogService
    {
        private ResourceDictionary DialogDictionary = new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Dialogs.xaml") };

        public void PromptMessage(string title, string message)
        {
            var metroDialogSettings = new MetroDialogSettings
            {
                CustomResourceDictionary = DialogDictionary,
                NegativeButtonText = "CANCEL",
                SuppressDefaultResources = true
            };

            DialogCoordinator.Instance.ShowMessageAsync(this, title, message, MessageDialogStyle.Affirmative,metroDialogSettings);
        }
    }
}
