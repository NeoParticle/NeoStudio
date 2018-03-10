using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace NeoStudio.View
{
    /// <summary>
    /// Interaction logic for OpenFileDialog.xaml
    /// </summary>
    public partial class OpenFileDialog : UserControl
    {
        private readonly Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();

        public OpenFileDialog()
        {
            InitializeComponent();
        }

        public bool AddExtenstion
        {
            get { return fileDialog.AddExtension; }
            set { fileDialog.AddExtension = value; }
        }
        public bool CheckFileExists
        {
            get { return fileDialog.CheckFileExists; }
            set { fileDialog.CheckFileExists = value; }
        }
        public bool CheckPathExists
        {
            get { return fileDialog.CheckPathExists; }
            set { fileDialog.CheckPathExists = value; }
        }
        public IList<Microsoft.Win32.FileDialogCustomPlace> CustomPlaces
        {
            get { return fileDialog.CustomPlaces; }
            set { fileDialog.CustomPlaces = value; }
        }
        public string DefaultExt
        {
            get { return fileDialog.DefaultExt; }
            set { fileDialog.DefaultExt = value; }
        }
        public bool DereferenceLinks
        {
            get { return fileDialog.DereferenceLinks; }
            set { fileDialog.DereferenceLinks = value; }
        }
        public string FileName
        {
            get { return fileDialog.FileName; }
            set { fileDialog.FileName = value; }
        }
        public string[] FileNames { get { return fileDialog.FileNames; } }
        public string Filter
        {
            get { return fileDialog.Filter; }
            set { fileDialog.Filter = value; }
        }
        public int FilterIndex
        {
            get { return fileDialog.FilterIndex; }
            set { fileDialog.FilterIndex = value; }
        }
        public string InitialDirectory
        {
            get { return fileDialog.InitialDirectory; }
            set { fileDialog.InitialDirectory = value; }
        }
        public bool Multiselect
        {
            get { return fileDialog.Multiselect; }
            set { fileDialog.Multiselect = value; }
        }
        public bool ReadOnlyChecked
        {
            get { return fileDialog.ReadOnlyChecked; }
            set { fileDialog.ReadOnlyChecked = value; }
        }
        public string SafeFileName { get { return fileDialog.SafeFileName; } }
        public string[] SafeFileNames { get { return fileDialog.SafeFileNames; } }
        public bool ShowReadOnly
        {
            get { return fileDialog.ShowReadOnly; }
            set { fileDialog.ShowReadOnly = value; }
        }
        new public object Tag
        {
            get { return fileDialog.Tag; }
            set { fileDialog.Tag = value; }
        }
        public string Title
        {
            get { return fileDialog.Title; }
            set { fileDialog.Title = value; }
        }
        public bool ValidateNames
        {
            get { return fileDialog.ValidateNames; }
            set { fileDialog.ValidateNames = value; }
        }

        public Stream OpenFile() { return fileDialog.OpenFile(); }
        public Stream[] OpenFiles() { return fileDialog.OpenFiles(); }
        public void Reset() { fileDialog.Reset(); }
        public bool? ShowDialog() { return fileDialog.ShowDialog(); }
        public bool? ShowDialog(Window w) { return fileDialog.ShowDialog(w); }

        public override string ToString() { return fileDialog.ToString(); }
    }
}
