using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoStudio.Model
{
    public class ContractModel
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string ParameterHexValue { get; set; }
        public string ReturnTypeHexValue { get; set; }
        public bool IsStorageRequired { get; set; } = true;
        public bool HasDynamicInvoke { get; set; } = false;

        public bool Validate()
        {
            if (!string.IsNullOrEmpty(Name) &&
                !string.IsNullOrEmpty(Version) &&
                !string.IsNullOrEmpty(Author) &&
                !string.IsNullOrEmpty(Email) &&
                !string.IsNullOrEmpty(Description) &&
                !string.IsNullOrEmpty(ParameterHexValue))
                return true;
            return false;
        }
    }
}
