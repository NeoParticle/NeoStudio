using NeoStudio.View;
using NeoStudio.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NeoStudio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static WalletSettings _walletSettings = new WalletSettings();

        public static WalletSettings WalletSettings
        {
            get { return _walletSettings; }
            set
            {
                _walletSettings = value;
                ((MainWindowViewModel)((NeoStudio.View.MainWindow)App.Current.MainWindow).DataContext).WalletList = new ObservableCollection<LocalWallet>(value.LocalWallets);
            }
        }

        public App()
        {
            WalletSettings.LoadWalletSettings();
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            NeoStudio.Init.RegisteBlockChain();
        }
    }
}
