using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleModel.WpfApp.ViewModels
{
    public class SingleValueViewModel<PrimitiveType, ParentType> : BaseViewModel
    {
        private PrimitiveType _value;

        public SingleValueViewModel(Func<ParentType, PrimitiveType> documentPull, Func<PrimitiveType, ParentType, ParentType> documentUpdate)
        {
            this.DocumentPull = documentPull;
            this.DocumentUpdate = documentUpdate;
        }

        public PrimitiveType Value
        {
            get { return _value; }
            set { base.SetField(ref _value, value); }
        }

        public Func<ParentType, PrimitiveType>  DocumentPull { get; set; }
        public Func<PrimitiveType, ParentType, ParentType> DocumentUpdate { get; set; }
    }
}
