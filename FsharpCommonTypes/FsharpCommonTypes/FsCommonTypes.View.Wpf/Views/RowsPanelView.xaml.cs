using Common.ViewModels;
using Common.ViewModels.Interfaces;
using FsCommonTypes.View.Wpf.Builders;
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
    /// Interaction logic for RowsPanelView.xaml
    /// </summary>
    public partial class RowsPanelView : UserControl
    {
        public RowsPanelView()
        {
            InitializeComponent();
        }

        public void SetViewComponent(IPanelViewModel panelView, ViewBuildersCollection viewBuilders)
        {
            this.DataContext = panelView;
            var children = panelView.GetChildren();
            int currRow = 0;
            foreach (var childView in children)
            {
                AddRow();
                AddRow();
                AddLabelView(currRow, childView);
                AddFieldView(currRow, childView, viewBuilders);
                currRow += 2;
            }
        }
        private void AddFieldView(int currRow, IViewComponent childView, ViewBuildersCollection viewBuilders)
        {
            
            if (childView.UiHint == "RowsPanel")
            {
                var newCtrl = new RowsPanelView();
                newCtrl.SetViewComponent((IPanelViewModel) childView, viewBuilders);
                Grid.SetRow(newCtrl, currRow);
                Grid.SetColumn(newCtrl, 1);
                Grid.SetColumnSpan(newCtrl, 4);
                ChildrenViewsContainer.Children.Add(newCtrl);
            }
            else
            {
                UserControl newCtrl = viewBuilders.GetBuilder(childView.UiHint).ViewFactory(childView);
                if (newCtrl != null)
                {
                    Grid.SetRow(newCtrl, currRow);
                    Grid.SetColumn(newCtrl, 1);
                    if (childView.UiHint == "PivotGridList")
                    {
                        Grid.SetColumnSpan(newCtrl, 4);
                    }
                    ChildrenViewsContainer.Children.Add(newCtrl);
                }
            }

        }

        private void AddLabelView(int currRow, IViewComponent childView)
        {
            var label = new TextBlock() { Text = childView.Label };
            Grid.SetRow(label, currRow);
            Grid.SetColumn(label, 0);
            label.VerticalAlignment = VerticalAlignment.Center;
            label.HorizontalAlignment = HorizontalAlignment.Right;
            label.Margin = new Thickness(16, 0, 8, 0);
            ChildrenViewsContainer.Children.Add(label);
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void AddRow()
        {
            RowDefinition gridRow1 = new RowDefinition();
            gridRow1.Height = GridLength.Auto;
            ChildrenViewsContainer.RowDefinitions.Add(gridRow1);
        }
    }
}
