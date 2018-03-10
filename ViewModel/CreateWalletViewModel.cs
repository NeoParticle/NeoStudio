using NeoStudio.Shared;
using NeoStudio.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NeoStudio.ViewModel
{
    public class CreateWalletViewModel : BaseViewModel
    {
        string walletName = string.Empty;
        public string WalletName
        {
            get { return walletName; }
            set
            {
                walletName = value;
                RaisePropertyChanged();
            }
        }

        public ICommand CreateWalletCommand { get { return new RelayCommand<object>(CreateWalletMethod); } }

        void CreateWalletMethod(object obj)
        {
            if (((MainWindow)App.Current.MainWindow).ActiveDocument == null)
            {
                MessageBox.Show("Please create or open a Smart Contract.", "Neo Studio", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (((MainWindow)App.Current.MainWindow).ActiveDocument.FilePath == null)
            {
                MessageBox.Show("Please save your Smart Contract.", "Neo Studio", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrEmpty(WalletName))
            {
                if (App.WalletSettings.LocalWallets.Where(w => w.WalletName == Path.GetFileNameWithoutExtension(WalletName) + ".json").Count() > 0)
                {
                    MessageBox.Show("Wallet Name Already exists....");
                    Messenger.Default.Send<bool>(true, "CloseCreateWalletWindow");
                }

                string walletPath = Path.Combine(Path.GetDirectoryName(((MainWindow)App.Current.MainWindow).ActiveDocument.FilePath), Path.GetFileNameWithoutExtension(WalletName) + ".json"); ;
                LocalWallet localWallet;
                if (!NeoWallet.Create(walletPath, out localWallet))
                {
                    MessageBox.Show("Failed to create wallet", "Create Wallet", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                App.WalletSettings.LocalWallets.Add(localWallet);
                App.WalletSettings.DefaultWallet = walletPath;
                App.WalletSettings.DefaultPassword = localWallet.Password;
                App.WalletSettings.Save(Path.GetDirectoryName(walletPath));
                ((MainWindowViewModel)((NeoStudio.View.MainWindow)App.Current.MainWindow).DataContext).WalletList.Add(localWallet);
                ((MainWindowViewModel)((NeoStudio.View.MainWindow)App.Current.MainWindow).DataContext).SelectedWallet = localWallet;

                ((MainWindow)App.Current.MainWindow).RefreshSoltionExplorer();
                MessageBox.Show("Success!!!", "Create Wallet", MessageBoxButton.OK, MessageBoxImage.Information);
                Messenger.Default.Send<bool>(true, "CloseCreateWalletWindow");
            }
            else
                MessageBox.Show("Please Enter a wallet Name.");

            Messenger.Default.Send<bool>(true, "CloseCreateWalletWindow");
        }
    }
}
