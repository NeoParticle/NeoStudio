using NeoStudio.Model;
using NeoStudio.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NeoStudio.ViewModel
{
    public class DeployContractViewModel : BaseViewModel
    {
        private ContractModel contract = new ContractModel()
        {
            Name = Guid.NewGuid().ToString(),
            Version = "V1.0",
            Author = "Author",
            Email = "abc@vad.com",
            Description = "Testing a Contract",
            IsStorageRequired = true,
            HasDynamicInvoke = false
        };

        public ContractModel Contract
        {
            get { return contract; }
            set
            {
                contract = value;
                RaisePropertyChanged();
            }
        }

        public ICommand DeployCommand { get { return new RelayCommand<object>(DeployMethod); } }

        private void DeployMethod(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
