using NeoStudio.ViewModel;
using System;
using System.CodeDom.Compiler;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeoStudio.View
{
    /// <summary>
    /// Interaction logic for ErrorOutputView.xaml
    /// </summary>
    public partial class ErrorOutputView : UserControl
    {
        public ErrorOutputView()
        {
            InitializeComponent();
            DataContext = new ErrorOutputViewModel();
        }

        internal void AddError(CompilerError compilerErrorCollection)
        {
            ((ErrorOutputViewModel)DataContext).ErrorOutput.Add(compilerErrorCollection);
        }

        internal void IsNoError(bool value)
        {
            if (value)
            {
                successImage.Visibility = Visibility.Visible;
                errorList.Visibility = Visibility.Hidden;
            }
            else
            {
                successImage.Visibility = Visibility.Hidden;
                errorList.Visibility = Visibility.Visible;
            }
        }

        internal void ClearError()
        {
            ((ErrorOutputViewModel)DataContext).ErrorOutput.Clear();
        }
    }
}
