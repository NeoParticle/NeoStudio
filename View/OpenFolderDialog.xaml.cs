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

namespace NeoStudio.View
{
    /// <summary>
    /// Interaction logic for OpenFolderDialog.xaml
    /// </summary>
    public partial class OpenFolderDialog : UserControl
    {
        private readonly System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

        public OpenFolderDialog()
        {
            InitializeComponent();
        }

        public string Show()
        {
            var result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                return folderBrowserDialog.SelectedPath;
            return string.Empty;
        }
    }
}
