using NeoStudio.Shared;
using NeoStudio.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoStudio.ViewModel
{
    public class SolutionExplorerViewModel : BaseViewModel
    {
        ObservableCollection<Model.TreeViewItem> _root = new ObservableCollection<Model.TreeViewItem>();


        public ObservableCollection<Model.TreeViewItem> Root
        {
            get
            {
                return _root;
            }
        }
    }
}
