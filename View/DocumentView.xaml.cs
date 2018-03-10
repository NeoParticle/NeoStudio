using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;

namespace NeoStudio.View
{
    /// <summary>
    /// Interaction logic for DocumentView.xaml
    /// </summary>
    public partial class DocumentView : LayoutDocument
    {
        public DocumentView()
        {
            InitializeComponent();
            this.Title = "";
            this.Closing += new EventHandler<CancelEventArgs>(DocumentForm_Closing);
            scintilla.SavePointLeft += Scintilla_SavePointLeft;
        }

        private void Scintilla_SavePointLeft(object sender, EventArgs e)
        {
            AddOrRemoveAsteric();
        }

        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public bool SaveAs()
        {
            bool? res = saveFileDialog.ShowDialog();
            if (res != null && (bool)res)
            {
                _filePath = saveFileDialog.FileName;
                return Save(_filePath);
            }

            return false;
        }

        public bool Save(string filePath)
        {
            using (FileStream fs = File.Create(filePath))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                    if (scintilla.Text != null && scintilla.Text.Trim().Length != 0)
                        bw.Write(scintilla.Text.ToCharArray(), 0, scintilla.Text.Length);
            }
            this.Title = System.IO.Path.GetFileName(filePath);
            scintilla.SetSavePoint();
            return true;
        }

        public bool Save()
        {
            if (String.IsNullOrEmpty(_filePath))
                return SaveAs();

            return Save(_filePath);
        }

        private void DocumentForm_Closing(object sender, CancelEventArgs e)
        {
            if (scintilla.Modified)
            {
                string message = String.Format(CultureInfo.CurrentCulture, "The _text in the {0} file has changed.{1}{2}Do you want to save the changes?", Title.TrimEnd(' ', '*'), Environment.NewLine, Environment.NewLine);

                MessageBoxResult dr = MessageBox.Show(message, DocTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                if (dr == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                else if (dr == MessageBoxResult.Yes)
                {
                    e.Cancel = !Save();
                    return;
                }
            }
        }

        public static string DocTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (!String.IsNullOrEmpty(titleAttribute.Title))
                        return titleAttribute.Title;
                }

                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public void AddOrRemoveAsteric()
        {
            if (scintilla.Modified)
            {
                if (!Title.EndsWith(" *", StringComparison.InvariantCulture))
                    Title += " *";
            }
            else
            {
                if (Title.EndsWith(" *", StringComparison.InvariantCulture))
                    Title = Title.Substring(0, Title.Length - 2);
            }
        }

    }
}
