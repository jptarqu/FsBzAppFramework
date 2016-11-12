using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.ViewModels.Interfaces;
using FsCommonTypes.View.XamarinForms.Builders;
using Xamarin.Forms;

namespace FsharpCommonTypes.View.XamarinForms.Views
{
    public partial class RowsPanelView : ContentView
    {
        public RowsPanelView()
        {

        }
        public void SetViewComponent(IPanelViewModel panelView, ViewBuildersCollection viewBuilders)
        {
            this.BindingContext = panelView;
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
                newCtrl.SetViewComponent((IPanelViewModel)childView, viewBuilders);
                Grid.SetRow(newCtrl, currRow);
                Grid.SetColumn(newCtrl, 1);
                Grid.SetColumnSpan(newCtrl, 4);
                ChildrenViewsContainer.Children.Add(newCtrl);
            }
            else
            {
                var newCtrl = viewBuilders.GetBuilder(childView.UiHint).ViewFactory(childView);
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
            var label = new Label() { Text = childView.Label };
            Grid.SetRow(label, currRow);
            Grid.SetColumn(label, 0);
            //label.VerticalAlignment = VerticalAlignment.Center;
            //label.HorizontalAlignment = HorizontalAlignment.Right;
            label.Margin = new Thickness(16, 0, 8, 0);
            ChildrenViewsContainer.Children.Add(label);
        }


        private void AddRow()
        {
            RowDefinition gridRow1 = new RowDefinition();
            gridRow1.Height = GridLength.Auto;
            ChildrenViewsContainer.RowDefinitions.Add(gridRow1);
        }
    }
}
