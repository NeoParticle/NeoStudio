using NeoStudio.ViewModel;
using System.Windows;

namespace NeoStudio.View
{
    /// <summary>
    /// Interaction logic for DeployContractDialog.xaml
    /// </summary>
    public partial class DeployContractDialog : Window
    {
        public DeployContractDialog()
        {
            InitializeComponent();
            DataContext = new DeployContractViewModel();
        }
    }
}
