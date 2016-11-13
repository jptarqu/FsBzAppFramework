using Xamarin.Forms;
using Common.ViewModels;
using FsCommonTypes.View.XamarinForms.Builders;
using FsharpCommonTypes.View.XamarinForms.Views;

namespace XamarinSample
{
    public partial class XamarinSamplePage : ContentPage
    {
        readonly ViewBuildersCollection _viewBuilders;

        public XamarinSamplePage()
        {
            InitializeComponent();

            _viewBuilders = new ViewBuildersCollection();
            var screenManager = new ScreenManager();
            var dialogService = new DialogService();

            var viewModel = SalesHelpers.SampleViewModel(dialogService, screenManager);
            var viewModelBase = viewModel.InitNoScreen();
            var docContainer = new DocView();
            docContainer.SetDoc(viewModelBase, _viewBuilders, "FirstScreenDummy");
        }
    }
}
