using Common.ViewModels;
using FsCommonTypes.View.Wpf.Builders;
using FsCommonTypes.View.Wpf.SyncFusion.Builders;
using FsCommonTypes.View.Wpf.Views;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FsCommonTypes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        private static DialogService dialogService;

        public MainWindow()
        {
            InitializeComponent();
            if(!DesignerProperties.GetIsInDesignMode(this))
            {
                ScreenManager screenManager = SetupFramework();
                SetupFirstScreen(screenManager);
            }
        }

        private static void SetupFirstScreen(ScreenManager screenManager)
        {
            dialogService = new DialogService();
            var newViewModel = new SalesListViewModel(dialogService, screenManager);
            newViewModel.StartUp();
        }

        private ScreenManager SetupFramework()
        {
            var screenManager = this.tabsManager.GetScreenManager();
            View.Wpf.Builders.ViewBuildersCollection viewBuilders = new ViewBuildersCollection();
            viewBuilders.AddDefaultViewBuilders();
            viewBuilders.AddSyncfusionViewBuilders();
            this.tabsManager.SetupManager(viewBuilders);
            return screenManager;
        }

        private async void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO add unique identifier per docModel view (dynamic id). Each of those viewws must have a 
            //  dialoghost
           //await  dialogService.PromptMessage("Test", "Test"); //exception: No loaded DialogHost instances.
        }
    }
}
