using NeoStudio.View;
using ScintillaNET;
using ScintillaNET.WPF;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NeoStudio
{
    internal static class CSharp
    {
        internal static bool Compile()
        {
            DocumentView activeDocument = ((MainWindow)App.Current.MainWindow).ActiveDocument;
            if (activeDocument == null)
                return false;
            ScintillaWPF scintilla = activeDocument.scintilla;

            if (activeDocument.FilePath != null)
                using (CodeDomProvider csc = Microsoft.CSharp.CSharpCodeProvider.CreateProvider("CSharp"))
                {
                    ((MainWindow)App.Current.MainWindow).errorView.ClearError();
                    System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
                    parameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
                    parameters.ReferencedAssemblies.Add("Neo.SmartContract.Framework.dll");
                    parameters.ReferencedAssemblies.Add("System.dll");
                    parameters.ReferencedAssemblies.Add("System.Data.DataSetExtensions.dll");
                    parameters.ReferencedAssemblies.Add("System.Data.dll");
                    parameters.ReferencedAssemblies.Add("System.Core.dll");
                    parameters.ReferencedAssemblies.Add("System.Numerics.dll");
                    parameters.ReferencedAssemblies.Add("System.Drawing.dll");
                    parameters.ReferencedAssemblies.Add("System.Runtime.Serialization.dll");
                    parameters.ReferencedAssemblies.Add("System.ComponentModel.dll");
                    parameters.ReferencedAssemblies.Add("System.Xml.dll");
                    parameters.ReferencedAssemblies.Add("System.Xml.Linq.dll");
                    parameters.GenerateExecutable = false;
                    parameters.GenerateInMemory = true;
                    parameters.OutputAssembly = activeDocument.FilePath.Remove(activeDocument.FilePath.Length - 2, 2) + "dll";
                    CompilerResults results = csc.CompileAssemblyFromSource(parameters, activeDocument.scintilla.Text);

                    scintilla.IndicatorCurrent = ScintillaConstants.ERROR_INDICATOR;
                    scintilla.IndicatorClearRange(0, scintilla.TextLength);

                    
                    if (results.Errors.Count > 0)
                    {
                        ((MainWindow)App.Current.MainWindow).errorView.IsNoError(false);
                        foreach (CompilerError CompErr in results.Errors)
                        {
                            ((MainWindow)App.Current.MainWindow).errorView.AddError(CompErr);
                            UnderLineCompiledError(CompErr, scintilla);
                        }
                    }
                    else
                    {
                        ((MainWindow)App.Current.MainWindow).errorView.IsNoError(true);
                    }
                    return true;
                }
            return false;
        }

        internal static void UnderLineCompiledError(CompilerError compilerError, ScintillaWPF scintilla)
        {
            Line line = scintilla.Lines[compilerError.Line - 1];
            scintilla.IndicatorFillRange(line.Position + compilerError.Column - 1, line.Length - compilerError.Column - 1);
        }
    }
}
