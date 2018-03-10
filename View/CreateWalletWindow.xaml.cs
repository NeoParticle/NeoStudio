using NeoStudio.Shared;
using NeoStudio.ViewModel;
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
using System.Windows.Shapes;

namespace NeoStudio.View
{
    /// <summary>
    /// Interaction logic for CreateWalletWindow.xaml
    /// </summary>
    public partial class CreateWalletWindow : Window
    {
        public CreateWalletWindow()
        {
            InitializeComponent();
            DataContext = new CreateWalletViewModel();
            Messenger.Default.Register<bool>("CreateWalletWindow", close, "CloseCreateWalletWindow");
        }

        private void close(bool obj)
        {
            Messenger.Default.Unregister("CreateWalletWindow", "CloseCreateWalletWindow");
            this.Close();
        }
    }
}
