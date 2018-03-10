using Neo;
using Neo.Implementations.Wallets.NEP6;
using Neo.Wallets;
using NeoStudio.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoStudio
{
    public static class NeoWallet
    {
        private static NEP6Wallet _ActiveWallet = null;
        public static NEP6Wallet ActiveWallet
        {
            get { return _ActiveWallet; }
            set { _ActiveWallet = value; }
        }
        private static string _password = "##hhsdhwOILL()*76";

        public static bool Create(string walletPath, out WalletAccount walletAccount)
        {
            walletAccount = null;
            try
            {
                NEP6Wallet wallet = new NEP6Wallet(walletPath);
                wallet.Unlock(_password);
                walletAccount = wallet.CreateAccount();
                wallet.Save();
                if (ActiveWallet != null)
                    ActiveWallet.Dispose();
                ActiveWallet = wallet;
                NeoStudio.Init.ChangeWallet(wallet);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Create(string walletPath, out LocalWallet localWallet)
        {
            localWallet = new LocalWallet();
            WalletAccount walletAccount;
            try
            {
                if (!Create(walletPath, out walletAccount))
                    return false;

                localWallet.WalletName = walletPath;
                localWallet.Address = walletAccount.Address;
                localWallet.PublicKey = walletAccount.GetKey().PublicKey.EncodePoint(true).ToHexString();
                localWallet.Password = _password;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool OpenWallet(string walletPath)
        {
            if (!OpenWallet(walletPath, _password))
                return false;
            return true;
        }

        public static bool OpenWallet(string walletPath, string password)
        {
            if (string.IsNullOrEmpty(password))
                password = _password;
            NEP6Wallet nep6wallet = new NEP6Wallet(walletPath);
            try
            {
                nep6wallet.Unlock(password);
                if (ActiveWallet != null)
                    ActiveWallet.Dispose();
                ActiveWallet = nep6wallet;
                NeoStudio.Init.ChangeWallet(nep6wallet);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool OpenWallet(LocalWallet localWallet)
        {
            return OpenWallet(localWallet.WalletName, localWallet.Password);
        }
    }

    public class WalletSettings
    {
        public List<LocalWallet> LocalWallets { get; set; } = new List<LocalWallet>();
        public string DefaultWallet { get; set; }
        public string DefaultPassword { get; set; }
        public bool Save(string dirPath)
        {
            try
            {
                File.WriteAllText(Path.Combine(dirPath, "Settings.Wallet.json"), JsonConvert.SerializeObject(this));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool LoadWalletSettings()
        {
            WalletSettings walletSettings = null;
            try
            {
                string lastContractPath = Properties.Settings.Default.LastWalletPath;
                string walletSettingsFilePath = Path.Combine(Path.GetDirectoryName(lastContractPath), "Settings.Wallet.json");
                if (File.Exists(walletSettingsFilePath))
                {
                    walletSettings = JsonConvert.DeserializeObject<WalletSettings>(File.ReadAllText(walletSettingsFilePath));
                    NeoWallet.OpenWallet(walletSettings.DefaultWallet, walletSettings.DefaultPassword);
                    this.LocalWallets = walletSettings.LocalWallets;
                    this.DefaultWallet = walletSettings.DefaultWallet;
                    this.DefaultPassword = walletSettings.DefaultPassword;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class LocalWallet
    {
        public string WalletName { get; set; }
        public string PublicKey { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string DisplayName
        {
            get
            {
                if (Path.GetFileName(WalletName) == string.Empty)
                    return WalletName;
                else
                    return Path.GetFileName(WalletName);
            }
        }
    }
}
