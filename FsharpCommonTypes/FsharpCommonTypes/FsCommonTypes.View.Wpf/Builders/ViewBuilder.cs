using Common.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FsCommonTypes.View.Wpf.Builders
{
    public class ViewBuilder
    {
        private readonly string _uiHint;
        private readonly Func<IViewComponent, UserControl> _viewFactory;

        public ViewBuilder(string uiHint, Func<IViewComponent, UserControl> viewFactory)
        {
            this._uiHint = uiHint;
            this._viewFactory = viewFactory;
        }

        public string UiHint
        {
            get
            {
                return _uiHint;
            }
            
        }

        public Func<IViewComponent, UserControl> ViewFactory
        {
            get
            {
                return _viewFactory;
            }
            
        }
    }
}
