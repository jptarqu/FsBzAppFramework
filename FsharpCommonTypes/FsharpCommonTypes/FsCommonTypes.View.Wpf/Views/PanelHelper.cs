using Common.ViewModels.Interfaces;
using FsCommonTypes.View.Wpf.Builders;
using FsCommonTypes.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FsCommonTypes.View.Wpf.Views
{
    internal static class PanelHelper
    {
        public static void AddFieldViewAsCol(int currCol, IViewComponent childView, ViewBuildersCollection viewBuilders, Grid ChildrenViewsContainer)
        {
            UserControl newCtrl = null;
            newCtrl = CreateField(childView, viewBuilders, ChildrenViewsContainer);
            if (newCtrl != null)
            {
                Grid.SetColumn(newCtrl, currCol);
                Grid.SetRow(newCtrl, 1);
            }
        }
        public static void AddFieldViewAsRow(int currRow, IViewComponent childView, ViewBuildersCollection viewBuilders, Grid ChildrenViewsContainer)
        {
            UserControl newCtrl = null;
            newCtrl = CreateField(childView, viewBuilders, ChildrenViewsContainer);
            if (newCtrl != null)
            {
                Grid.SetRow(newCtrl, currRow);
                Grid.SetColumn(newCtrl, 1);
            }
        }

        private static UserControl CreateField(IViewComponent childView, ViewBuildersCollection viewBuilders, Grid ChildrenViewsContainer)
        {
            UserControl newCtrl;
            if (childView.UiHint == "RowsPanel")
            {
                newCtrl = new RowsPanelView();
                ((RowsPanelView)newCtrl).SetViewComponent((IPanelViewModel)childView, viewBuilders);
                ChildrenViewsContainer.Children.Add(newCtrl);
            }
            else if (childView.UiHint == "ColumnsPanel")
            {
                newCtrl = new ColumnsPanelView();
                ((ColumnsPanelView)newCtrl).SetViewComponent((IPanelViewModel)childView, viewBuilders);
                ChildrenViewsContainer.Children.Add(newCtrl);
            }
            else
            {
                newCtrl = viewBuilders.GetBuilder(childView.UiHint).ViewFactory(childView);
                if (newCtrl != null)
                {
                    ChildrenViewsContainer.Children.Add(newCtrl);
                }
            }

            return newCtrl;
        }
    }
}
