using Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FsharpCommonTypes.View.XamarinForms.Views
{
    public class DialogService : IDialogService
    {
        public async Task PromptMessage(string screenId, string title, string message)
        {
            var lop = 1;
            var task = new Task<int>(() => lop++);
            //show the dialog
            var t = await task;
            //DialogCoordinator.Instance.ShowMessageAsync(this, title, message, MessageDialogStyle.Affirmative,metroDialogSettings);
        }

    }
}
