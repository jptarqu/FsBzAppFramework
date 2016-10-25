using Common.ViewModels;
using FsCommonTypes.View.Wpf.Builders;
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
        private ViewBuildersCollection _viewBuilders;

        public ScreenManagerView()
        {
            viewModel = new ScreenManager();
            DataContext = viewModel;
            viewModel.CurrentScreens.CollectionChanged += CurrentScreens_CollectionChanged;
            InitializeComponent();
        }

        private void CurrentScreens_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var screensToOpen = e.NewItems;
                foreach (IScreen newScreen in screensToOpen)
                {
                    var newTab = new TabItem();
                    newTab.Tag = newScreen.ScreenId;
                    newTab.Header = newScreen.DisplayName;

                    var docContainer = new DocView();
                    docContainer.SetDoc(newScreen.DocModel, _viewBuilders, newScreen.ScreenId);

                    newTab.Content = docContainer;
                    tabsManager.Items.Add(newTab);
                    tabsManager.SelectedItem=(newTab);
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {

                var screensToClose = e.OldItems;

                foreach (IScreen screenToClose in screensToClose)
                {
                    var foundIdx = FindTabToClose(tabsManager.Items, screenToClose);
                    if (foundIdx >= 0)
                    {
                        tabsManager.Items.RemoveAt(foundIdx);
                    }
                }
            }
        }
        private int FindTabToClose(ItemCollection items, IScreen screenToClose)
        { 

            var itemIdx = -1;
            for (itemIdx = 0; itemIdx < items.Count; itemIdx++)
            {
                TabItem item = items[itemIdx] as TabItem;
                if (item != null && (((string) item.Tag) == screenToClose.ScreenId))
                {
                    break;
                }
            }
            return itemIdx;
        }
        public void SetupManager(ViewBuildersCollection viewBuilders)
        {
            _viewBuilders = viewBuilders;
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
