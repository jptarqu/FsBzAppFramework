using Common.ViewModels.Interfaces;
using FsCommonTypes.View.Wpf.Builders;
using FsCommonTypes.Views;
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
    /// Interaction logic for ColumnsPanelView.xaml
    /// </summary>
    public partial class ColumnsPanelView : UserControl
    {
        public ColumnsPanelView()
        {
            InitializeComponent();
        }

        public void SetViewComponent(IPanelViewModel panelView, ViewBuildersCollection viewBuilders)
        {
            this.DataContext = panelView;
            var children = panelView.GetChildren();
            int currCol = 1;
            AddPadColumn(3);
            foreach (var childView in children)
            {
                if (currCol > 1)
                {
                    AddPadColumn(1);
                }
                AddColumn();
                AddLabelView(currCol, childView);
                PanelHelper.AddFieldViewAsCol(currCol, childView, viewBuilders, ChildrenViewsContainer);
                currCol += 2;
            }
            AddPadColumn(3);
        }

            

        private void AddLabelView(int currCol, IViewComponent childView)
        {
            var label = new TextBlock() { Text = childView.Label };
            Grid.SetColumn(label, currCol);
            Grid.SetRow(label, 0);

            label.VerticalAlignment = VerticalAlignment.Center;
            label.HorizontalAlignment = HorizontalAlignment.Left;
            //label.Margin = new Thickness(16, 0, 8, 0);
            ChildrenViewsContainer.Children.Add(label);
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void AddPadColumn(int size)
        {
            var col = new ColumnDefinition()
            {
                Width = new GridLength(size, GridUnitType.Star)
            };
            ChildrenViewsContainer.ColumnDefinitions.Add(col);
        }
        private void AddColumn()
        {
            var col = new ColumnDefinition()
            {
                Width = GridLength.Auto
            };
            ChildrenViewsContainer.ColumnDefinitions.Add(col);
        }
    }
}
