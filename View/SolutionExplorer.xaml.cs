using NeoStudio.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace NeoStudio.View
{
    /// <summary>
    /// Interaction logic for SolutionExplorer.xaml
    /// </summary>
    public partial class SolutionExplorer : UserControl
    {
        SolutionExplorerViewModel solutionExplorerViewModel;

        public SolutionExplorer()
        {
            InitializeComponent();
            solutionExplorerViewModel = new SolutionExplorerViewModel();
            DataContext = solutionExplorerViewModel;
        }

        public void Clear()
        {
            solutionExplorerViewModel.Root.Clear();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Model.TreeViewItem treeViewItem = e.NewValue as Model.TreeViewItem;
            if (treeViewItem != null && parent != treeViewItem)
                if (treeViewItem.IsDirectory)
                {
                    LoadFolder(treeViewItem.Children, treeViewItem.Info.FullName);
                }
                else
                {
                    if (treeViewItem.IsText)
                    {
                        ShowText(treeViewItem.Info);
                    }
                }
        }

        Model.TreeViewItem parent = new Model.TreeViewItem();

        public void LoadFolder(string projectPath)
        {
            parent = new Model.TreeViewItem(new System.IO.FileInfo(projectPath));
            solutionExplorerViewModel.Root.Add(parent);
            LoadFolder(parent.Children, projectPath);
        }

        void LoadFolder(ObservableCollection<Model.TreeViewItem> parent, string folder)
        {
            IEnumerable<string> folderList = Directory.EnumerateDirectories(folder);
            IEnumerable<string> fileList = Directory.EnumerateFiles(folder);

            try
            {
                foreach (string directory in folderList)
                {
                    parent.Add(new Model.TreeViewItem(new FileInfo(directory)));
                }
            }
            catch { }
            try
            {

                foreach (string file in fileList)
                {
                    parent.Add(new Model.TreeViewItem(new FileInfo(file)));
                }
            }
            catch { }

        }

        public void ShowText(FileInfo info)
        {
            var mainWindow = (MainWindow)App.Current.MainWindow;
            mainWindow.OpenFileFromOutside(info.FullName);
        }


    }
}
