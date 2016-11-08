
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControls;
using Microsoft.FSharp.Core;
using Common.ViewModels.Interfaces;

namespace FsCommonTypes.View.Wpf
{
    class ChoiceItemsFuncProvider<ResultType> : ISuggestionProvider
    {
        private readonly Func<string, IEnumerable<SimpleExternalChoicesQueryResult<ResultType>>> _listProvider;

        public ChoiceItemsFuncProvider(Func<string, IEnumerable<SimpleExternalChoicesQueryResult<ResultType>>> listProvider)
        {
            _listProvider = listProvider;
        }

        public IEnumerable GetSuggestions(string filter)
        {
            var results= _listProvider.Invoke(filter);
            return results;
        }
    }
}
