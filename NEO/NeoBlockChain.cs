using Neo;
using Neo.Core;
using Neo.Implementations.Blockchains.LevelDB;
using Neo.Implementations.Wallets.EntityFramework;
using Neo.IO;
using Neo.Network;
using Neo.VM;
using Neo.Wallets;
using NeoStudio.Properties;
using NeoStudio.View;
using NeoStudio.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace NeoStudio
{
    public static class Init
    {
        public static LocalNode LocalNode;
        public static Wallet CurrentWallet;
        private static DateTime persistence_time = DateTime.MinValue;
        private static bool check_nep5_balance = false;
        private static readonly UInt160 RecycleScriptHash = new[] { (byte)OpCode.PUSHT }.ToScriptHash();
        private static bool balance_changed = false;

        public static void RegisteBlockChain()
        {
            FileStream fss = null;
            const string PeerStatePath = "peers.dat";
            if (File.Exists(PeerStatePath))
            {
                using (fss = new FileStream(PeerStatePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    LocalNode.LoadState(fss);
                }
            }

            using (Blockchain.RegisterBlockchain(new LevelDBBlockchain(Settings.Default.Paths.Chain)))
            {
                using (LocalNode = new LocalNode())
                {
                    LocalNode.UpnpEnabled = true;
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.ShowDialog();
                    App.Current.MainWindow = mainWindow;
                }
            }
            using (FileStream fs = new FileStream(PeerStatePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                LocalNode.SaveState(fs);
            }
        }

        public static bool OpenDefaultWallet()
        {
            LocalWallet localWallet;
            try
            {
                string walletPath = "DefaultWallet.json";
                string walletPassword = null;

                if (App.WalletSettings.LocalWallets.Count() > 0 && App.WalletSettings.DefaultWallet != null && App.WalletSettings.DefaultPassword != null)
                {
                    walletPath = App.WalletSettings.DefaultWallet;
                    walletPassword = App.WalletSettings.DefaultPassword;
                }
                if (!File.Exists(walletPath))
                {
                    walletPath = "DefaultWallet.json";
                    if (NeoStudio.NeoWallet.Create(walletPath, out localWallet))
                    {
                        if (App.WalletSettings.LocalWallets.Count(w => w.WalletName == localWallet.WalletName) == 0)
                        {
                            App.WalletSettings.LocalWallets.Add(localWallet);
                            ((MainWindowViewModel)((NeoStudio.View.MainWindow)App.Current.MainWindow).DataContext).WalletList.Add(localWallet);
                            App.WalletSettings.DefaultWallet = localWallet.WalletName;
                            App.WalletSettings.DefaultPassword = localWallet.Password;
                            App.WalletSettings.Save(Path.GetDirectoryName(localWallet.WalletName));
                        }
                        ((MainWindowViewModel)((NeoStudio.View.MainWindow)App.Current.MainWindow).DataContext).SelectedWallet = localWallet;
                        return true;
                    }
                    else
                        return false;
                }

                if (walletPassword == null)
                    if (NeoStudio.NeoWallet.OpenWallet(walletPath))
                    {

                        App.WalletSettings.DefaultWallet = walletPath;
                        if (App.WalletSettings.LocalWallets.Count(w => w.WalletName == walletPath) == 0)
                        {
                            localWallet = new LocalWallet();
                            localWallet.WalletName = walletPath;
                            App.WalletSettings.LocalWallets.Add(localWallet);
                            App.WalletSettings.Save(Path.GetDirectoryName(localWallet.WalletName));
                            ((MainWindowViewModel)((NeoStudio.View.MainWindow)App.Current.MainWindow).DataContext).WalletList.Add(localWallet);
                        }
                        ((MainWindowViewModel)((NeoStudio.View.MainWindow)App.Current.MainWindow).DataContext).SelectedWallet = App.WalletSettings.LocalWallets.FirstOrDefault(w => w.WalletName == walletPath);
                        return true;
                    }
                    else
                        return false;

                if (walletPassword != null)
                    if (NeoStudio.NeoWallet.OpenWallet(walletPath, walletPassword))
                    {
                        App.WalletSettings.DefaultWallet = walletPath;
                        App.WalletSettings.DefaultPassword = walletPassword;
                        App.WalletSettings.Save(Path.GetDirectoryName(walletPath));
                        ((MainWindowViewModel)((NeoStudio.View.MainWindow)App.Current.MainWindow).DataContext).SelectedWallet = App.WalletSettings.LocalWallets.FirstOrDefault(w => w.WalletName == walletPath);
                        return true;
                    }
                    else
                        return false;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static void SyncBlockChain()
        {
            Task.Run(() =>
            {
                const string acc_path = "chain.acc";
                const string acc_zip_path = acc_path + ".zip";
                if (File.Exists(acc_path))
                {
                    using (FileStream fs = new FileStream(acc_path, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        ImportBlocks(fs);
                    }
                    File.Delete(acc_path);
                }
                else if (File.Exists(acc_zip_path))
                {
                    using (FileStream fs = new FileStream(acc_zip_path, FileMode.Open, FileAccess.Read, FileShare.None))
                    using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Read))
                    using (Stream zs = zip.GetEntry(acc_path).Open())
                    {
                        ImportBlocks(zs);
                    }
                    File.Delete(acc_zip_path);
                }
                Blockchain.PersistCompleted += Blockchain_PersistCompleted;
                LocalNode.Start(Properties.Settings.Default.P2P.Port, Properties.Settings.Default.P2P.WsPort);
            });
        }

        public static void Blockchain_PersistCompleted(object sender, Block e)
        {
            persistence_time = DateTime.UtcNow;
            if (CurrentWallet != null)
            {
                check_nep5_balance = true;
                if (CurrentWallet.GetCoins().Any(p => !p.State.HasFlag(CoinState.Spent) && p.Output.AssetId.Equals(Blockchain.GoverningToken.Hash)) == true)
                    balance_changed = true;
                if (CurrentWallet.GetCoins().Any(p => !p.State.HasFlag(CoinState.Spent) && p.Output.AssetId.Equals(Blockchain.UtilityToken.Hash)) == true)
                    balance_changed = true;
            }
        }

        //public static void Blockchain_PersistCompleted(object sender, Block block)
        //{
        //    persistence_time = DateTime.UtcNow;
        //    if (CurrentWallet != null)
        //    {
        //        check_nep5_balance = true;
        //        if (CurrentWallet.GetCoins().Any(p => !p.State.HasFlag(CoinState.Spent) && p.Output.AssetId.Equals(Blockchain.GoverningToken.Hash)) == true)
        //            balance_changed = true;
        //    }
        //}

        private static void ImportBlocks(Stream stream)
        {
            LevelDBBlockchain blockchain = (LevelDBBlockchain)Blockchain.Default;
            blockchain.VerifyBlocks = false;
            using (BinaryReader r = new BinaryReader(stream))
            {
                uint count = r.ReadUInt32();
                for (int height = 0; height < count; height++)
                {
                    byte[] array = r.ReadBytes(r.ReadInt32());
                    if (height > Blockchain.Default.Height)
                    {
                        Block block = array.AsSerializable<Block>();
                        Blockchain.Default.AddBlock(block);
                    }
                }
            }
            blockchain.VerifyBlocks = true;
        }

        public static void timer_Tick(object sender, EventArgs e)
        {
            uint walletHeight = 0;

            if (CurrentWallet != null)
            {
                walletHeight = (CurrentWallet.WalletHeight > 0) ? CurrentWallet.WalletHeight - 1 : 0;
            }

            var height = $"{walletHeight}/{Blockchain.Default.Height}/{Blockchain.Default.HeaderHeight}";
            ((MainWindow)App.Current.MainWindow).heightLabel.Content = height;

            var nodeCount = LocalNode.RemoteNodeCount.ToString();
            ((MainWindow)App.Current.MainWindow).connectedNodes.Content = nodeCount;
            TimeSpan persistence_span = DateTime.UtcNow - persistence_time;
            if (persistence_span < TimeSpan.Zero) persistence_span = TimeSpan.Zero;

            if (CurrentWallet != null)
            {
                if (CurrentWallet.WalletHeight <= Blockchain.Default.Height + 1)
                {
                    if (balance_changed)
                    {
                        IEnumerable<Coin> coins = CurrentWallet?.GetCoins().Where(p => !p.State.HasFlag(CoinState.Spent)) ?? Enumerable.Empty<Coin>();
                        Fixed8 bonus_available = Blockchain.CalculateBonus(CurrentWallet.GetUnclaimedCoins().Select(p => p.Reference));
                        Fixed8 bonus_unavailable = Blockchain.CalculateBonus(coins.Where(p => p.State.HasFlag(CoinState.Confirmed) &&
                                p.Output.AssetId.Equals(Blockchain.GoverningToken.Hash)).Select(p => p.Reference), Blockchain.Default.Height + 1);
                        Fixed8 bonus = bonus_available + bonus_unavailable;
                        var assets = coins.GroupBy(p => p.Output.AssetId, (k, g) => new
                        {
                            Asset = Blockchain.Default.GetAssetState(k),
                            Value = g.Sum(p => p.Output.Value),
                            Claim = k.Equals(Blockchain.UtilityToken.Hash) ? bonus : Fixed8.Zero
                        }).ToDictionary(p => p.Asset.AssetId);
                        if (bonus != Fixed8.Zero && !assets.ContainsKey(Blockchain.UtilityToken.Hash))
                        {
                            assets[Blockchain.UtilityToken.Hash] = new
                            {
                                Asset = Blockchain.Default.GetAssetState(Blockchain.UtilityToken.Hash),
                                Value = Fixed8.Zero,
                                Claim = bonus
                            };
                        }
                        var balance_ans = coins.Where(p => p.Output.AssetId.Equals(Blockchain.GoverningToken.Hash)).GroupBy(p => p.Output.ScriptHash).ToDictionary(p => p.Key, p => p.Sum(i => i.Output.Value));
                        var balance_anc = coins.Where(p => p.Output.AssetId.Equals(Blockchain.UtilityToken.Hash)).GroupBy(p => p.Output.ScriptHash).ToDictionary(p => p.Key, p => p.Sum(i => i.Output.Value));

                        if (balance_ans.Count == 0)
                            ((MainWindow)App.Current.MainWindow).neoValue.Content = 0;
                        else
                            ((MainWindow)App.Current.MainWindow).neoValue.Content = balance_ans.First().Value;

                        if (balance_anc.Count == 0)
                            ((MainWindow)App.Current.MainWindow).gasValue.Content = 0;
                        else
                            ((MainWindow)App.Current.MainWindow).gasValue.Content = balance_anc.First().Value;

                        balance_changed = false;
                    }

                }
                if (check_nep5_balance && persistence_span > TimeSpan.FromSeconds(2))
                {
                    UInt160[] addresses = CurrentWallet.GetAccounts().Select(p => p.ScriptHash).ToArray();
                    foreach (string s in Properties.Settings.Default.NEP5Watched)
                    {
                        UInt160 script_hash = UInt160.Parse(s);
                        byte[] script;
                        using (ScriptBuilder sb = new ScriptBuilder())
                        {
                            foreach (UInt160 address in addresses)
                                sb.EmitAppCall(script_hash, "balanceOf", address);
                            sb.Emit(Neo.VM.OpCode.DEPTH, OpCode.PACK);
                            sb.EmitAppCall(script_hash, "decimals");
                            sb.EmitAppCall(script_hash, "name");
                            script = sb.ToArray();
                        }
                        Neo.SmartContract.ApplicationEngine engine = Neo.SmartContract.ApplicationEngine.Run(script);
                        if (engine.State.HasFlag(VMState.FAULT)) continue;
                        string name = engine.EvaluationStack.Pop().GetString();
                        byte decimals = (byte)engine.EvaluationStack.Pop().GetBigInteger();
                        //BigInteger amount = engine.EvaluationStack.Pop().GetArray().Aggregate(BigInteger.Zero, (x, y) => x + y.GetBigInteger());

                        //BigDecimal balance = new BigDecimal(amount, decimals);
                        //string value_text = balance.ToString();

                    }
                    check_nep5_balance = false;
                }
            }
        }

        private static void AddAccount(WalletAccount account, bool selected = false)
        {

        }

        private static void AddTransaction(Transaction tx, uint? height, uint time)
        {

        }

        private static void CurrentWallet_BalanceChanged(object sender, BalanceEventArgs e)
        {
            balance_changed = true;
            AddTransaction(e.Transaction, e.Height, e.Time);
        }

        public static void ChangeWallet(Wallet wallet)
        {
            if (CurrentWallet != null)
            {
                CurrentWallet.BalanceChanged -= CurrentWallet_BalanceChanged;
                if (CurrentWallet is IDisposable disposable)
                    disposable.Dispose();
            }
            CurrentWallet = wallet;
            if (CurrentWallet != null)
            {
                foreach (var i in CurrentWallet.GetTransactions().Select(p => new
                {
                    Transaction = Blockchain.Default.GetTransaction(p, out int height),
                    Height = (uint)height
                }).Where(p => p.Transaction != null).Select(p => new
                {
                    p.Transaction,
                    p.Height,
                    Time = Blockchain.Default.GetHeader(p.Height).Timestamp
                }).OrderBy(p => p.Time))
                {
                    AddTransaction(i.Transaction, i.Height, i.Time);
                }
                CurrentWallet.BalanceChanged += CurrentWallet_BalanceChanged;
            }
            bool isUserWallet = CurrentWallet is UserWallet;
            if (CurrentWallet != null)
            {
                foreach (WalletAccount account in CurrentWallet.GetAccounts().ToArray())
                {
                    AddAccount(account);
                }
            }
            balance_changed = true;
            check_nep5_balance = true;
        }
    }


}
