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
    public class MainWindowViewModel : BaseViewModel
    {
        private ObservableCollection<LocalWallet> _WalletList = new ObservableCollection<LocalWallet>();
        private LocalWallet _SelectedWallet;

        public ObservableCollection<LocalWallet> WalletList
        {
            get { return _WalletList; }
            set
            {
                _WalletList = value;
                RaisePropertyChanged("WalletList");
            }
        }

        public LocalWallet SelectedWallet
        {
            get { return _SelectedWallet; }
            set
            {
                _SelectedWallet = value;
                if (NeoWallet.OpenWallet(value.WalletName, value.Password))
                {
                    App.WalletSettings.DefaultWallet = value.WalletName;
                    App.WalletSettings.DefaultPassword = value.Password;
                    App.WalletSettings.Save(Path.GetDirectoryName(value.WalletName));
                }
                else
                {
                    NeoStudio.Init.OpenDefaultWallet();
                }

                RaisePropertyChanged("SelectedWallet");
            }
        }
    }
}
