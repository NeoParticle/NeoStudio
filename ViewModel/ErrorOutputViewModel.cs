using NeoStudio.Shared;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;

namespace NeoStudio.ViewModel
{
    public class ErrorOutputViewModel : BaseViewModel
    {
        CompilerError error = new CompilerError();
        ObservableCollection<CompilerError> errorOutput = new ObservableCollection<CompilerError>();

        public CompilerError Error
        {
            get
            {
                return error;
            }
            set
            {
                error = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<CompilerError> ErrorOutput
        {
            get
            {
                return errorOutput;
            }
            set
            {
                errorOutput = value;
                RaisePropertyChanged();
            }
        }
    }
}
