using Neo;
using Neo.Core;
using Neo.SmartContract;
using Neo.VM;
using NeoStudio.Model;
using NeoStudio.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NeoStudio
{
    public static class NeoTransaction
    {
        public static InvocationTransaction GetDeployContractTransaction(ContractModel contractModel, out string scriptHash, out string scriptBytesString)
        {
            MainWindow mainWindow = (MainWindow)App.Current.MainWindow;
            scriptHash = null;
            scriptBytesString = null;

            if (mainWindow.ActiveDocument == null)
                return null;

            RIPEMD160 myRIPEMD160 = RIPEMD160Managed.Create();
            string uniqueTempHash = BitConverter.ToString(myRIPEMD160.ComputeHash(Guid.NewGuid().ToByteArray()));
            mainWindow.ActiveDocument.scintilla.Text = mainWindow.ActiveDocument.scintilla.Text.Replace("MI4m2tqy+RPxxQfKGdKhg1Hb62s=", uniqueTempHash);
            mainWindow.ActiveDocument.Save();
            if (!SmartContract.Build())
                return null;

            string contractFileName = Path.Combine(Path.GetDirectoryName(mainWindow.ActiveDocument.FilePath), Path.GetFileNameWithoutExtension(mainWindow.ActiveDocument.FilePath), ".avm");
            if (!File.Exists(contractFileName))
                return null;

            if (!contractModel.Validate())
                return null;

            byte[] script = File.ReadAllBytes(contractFileName);
            scriptHash = script.ToScriptHash().ToString();

            byte[] parameter_list = contractModel.ParameterHexValue.HexToBytes();

            ContractParameterType return_type = contractModel.ReturnTypeHexValue.HexToBytes().Select(p => (ContractParameterType?)p).FirstOrDefault() ?? ContractParameterType.Void;
            ContractPropertyState properties = ContractPropertyState.NoProperty;
            if (contractModel.IsStorageRequired) properties |= ContractPropertyState.HasStorage;
            if (contractModel.HasDynamicInvoke) properties |= ContractPropertyState.HasDynamicInvoke;

            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitSysCall("Neo.Contract.Create", script, parameter_list, return_type, properties, contractModel.Name, contractModel.Version, contractModel.Author,
                    contractModel.Email, contractModel.Description);
                return new InvocationTransaction
                {
                    Script = sb.ToArray()
                };
            }
        }
    }
}
