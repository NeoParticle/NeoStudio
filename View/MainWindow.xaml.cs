using Neo.Core;
using NeoStudio.ViewModel;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace NeoStudio.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            lineNumbersMenuItem.IsChecked = true;
            DataContext = new MainWindowViewModel();
        }

        private int _newDocumentCount;
        public static RoutedCommand NewFileCommand = new RoutedCommand();
        public static RoutedCommand OpenFileCommand = new RoutedCommand();
        public static RoutedCommand OpenFolderCommand = new RoutedCommand();
        public static RoutedCommand SaveFileCommand = new RoutedCommand();
        public static RoutedCommand SaveAllFilesCommand = new RoutedCommand();
        public static RoutedCommand PrintFileCommand = new RoutedCommand();
        public static RoutedCommand UndoCommand = new RoutedCommand();
        public static RoutedCommand RedoCommand = new RoutedCommand();
        public static RoutedCommand CutCommand = new RoutedCommand();
        public static RoutedCommand CopyCommand = new RoutedCommand();
        public static RoutedCommand PasteCommand = new RoutedCommand();
        public static RoutedCommand SelectAllCommand = new RoutedCommand();
        public static RoutedCommand IncrementalSearchCommand = new RoutedCommand();
        public static RoutedCommand FindCommand = new RoutedCommand();
        public static RoutedCommand ReplaceCommand = new RoutedCommand();
        public static RoutedCommand GotoCommand = new RoutedCommand();
        public static RoutedCommand RunContract = new RoutedCommand();
        public static RoutedCommand StepInCode = new RoutedCommand();

        #region Properties
        public DocumentView ActiveDocument
        {
            get
            {
                return documentsRoot.Children.FirstOrDefault(c => c.Content == dockPanel.ActiveContent) as DocumentView;
            }
        }

        public IEnumerable<DocumentView> Documents
        {
            get { return documentsRoot.Children.Cast<DocumentView>(); }
        }
        #endregion

        #region Menu Events

        #region File Events
        private void newMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NewDocument();
        }

        private void openMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void openFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFolder();
        }

        private void closeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.Close();
        }

        private void saveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.Save();
        }

        private void saveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.SaveAs();
        }

        private void saveAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (DocumentView doc in Documents)
                doc.Save();
        }

        private void exitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Undo/Redo Event
        private void undoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.scintilla.Undo();
        }

        private void redoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.scintilla.Redo();
        }
        #endregion

        #region Cut/Copy/Paste Event
        private void cutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.scintilla.Cut();
        }

        private void copyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.scintilla.Copy();
        }

        private void pasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.scintilla.Paste();
        }
        #endregion

        #region Select/Select All/ClearSelection Event
        private void selectLine_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveDocument != null)
            {
                ScintillaNET.Line line = ActiveDocument.scintilla.Lines[ActiveDocument.scintilla.CurrentLine];
                ActiveDocument.scintilla.SetSelection(line.Position + line.Length, line.Position);
            }
        }

        private void selectAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.scintilla.SelectAll();
        }

        private void clearSelection_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.scintilla.SetEmptySelection(0);
        }
        #endregion

        #region Find And Replace Events TODO
        private void incrementalSearchMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void findMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void replaceMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void findInFilesMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void replaceInFilesMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void gotoMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region BookMark Events TODO
        private void toggleBookmarkMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void previousBookmarkMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void nextBookmarkMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void nextBookmarkMenuItem_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void clearBookmarksMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region UpperCase/LowerCase Event
        private void makeUpperCaseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ActiveDocument.scintilla.ExecuteCmd(Command.Uppercase);
        }

        private void makeLowerCaseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ActiveDocument.scintilla.ExecuteCmd(Command.Lowercase);
        }
        #endregion

        #region View
        private void toolbarMenuItem_Click(object sender, RoutedEventArgs e)
        {
            toolStrip.Visibility = Toggle(toolStrip.Visibility);
            toolbarMenuItem.IsChecked = toolStrip.Visibility == Visibility.Visible;
        }

        private void statusBarMenuItem_Click(object sender, RoutedEventArgs e)
        {
            statusStrip.Visibility = Toggle(statusStrip.Visibility);
            statusBarMenuItem.IsChecked = statusStrip.Visibility == Visibility.Visible;
        }

        private void solutionExplorerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!solutionExplorer.IsVisible)
            {
                solutionExplorerMenuItem.IsChecked = solutionExplorerLayout.IsVisible = true;
            }
            else
            {
                solutionExplorerMenuItem.IsChecked = solutionExplorerLayout.IsVisible = false;
            }
        }
        #endregion

        #region Control Character Visibility Event
        private void whitespaceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            whitespaceMenuItem.IsChecked = !whitespaceMenuItem.IsChecked;
            foreach (DocumentView doc in Documents)
            {
                if (whitespaceMenuItem.IsChecked)
                    doc.scintilla.ViewWhitespace = WhitespaceMode.VisibleAlways;
                else
                    doc.scintilla.ViewWhitespace = WhitespaceMode.Invisible;
            }
        }

        private void wordWrapMenuItem_Click(object sender, RoutedEventArgs e)
        {
            wordWrapMenuItem.IsChecked = !wordWrapMenuItem.IsChecked;
            foreach (DocumentView doc in Documents)
            {
                if (wordWrapMenuItem.IsChecked)
                    doc.scintilla.WrapMode = WrapMode.Word;
                else
                    doc.scintilla.WrapMode = WrapMode.None;
            }
        }

        private void endOfLineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            endOfLineMenuItem.IsChecked = !endOfLineMenuItem.IsChecked;
            foreach (DocumentView doc in Documents)
            {
                doc.scintilla.ViewEol = endOfLineMenuItem.IsChecked;
            }
        }
        #endregion

        #region Zoom Events
        private void zoomInMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _zoomLevel++;
            UpdateAllScintillaZoom();
        }

        private void zoomOutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _zoomLevel--;
            UpdateAllScintillaZoom();
        }

        private void resetZoomMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _zoomLevel = 0;
            UpdateAllScintillaZoom();
        }
        #endregion

        #region LineNUmber Margin Event
        private void lineNumbersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            lineNumbersMenuItem.IsChecked = !lineNumbersMenuItem.IsChecked;
            foreach (DocumentView docForm in Documents)
            {
                if (lineNumbersMenuItem.IsChecked)
                    docForm.scintilla.Margins[ScintillaConstants.NUMBER_MARGIN].Width = ScintillaConstants.LINE_NUMBERS_MARGIN_WIDTH;
                else
                    docForm.scintilla.Margins[ScintillaConstants.NUMBER_MARGIN].Width = 0;
            }
        }
        #endregion

        #region Fold Events
        private void foldLevelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.scintilla.Lines[ActiveDocument.scintilla.CurrentLine].FoldLine(FoldAction.Contract);
        }

        private void unfoldLevelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.scintilla.Lines[ActiveDocument.scintilla.CurrentLine].FoldLine(FoldAction.Expand);
        }

        private void foldAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.scintilla.FoldAll(FoldAction.Contract);
        }

        private void unfoldAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.scintilla.FoldAll(FoldAction.Expand);
        }
        #endregion

        private void buildContractMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CSharp.Compile();

            if (!SmartContract.Build())
                System.Windows.MessageBox.Show("Build Failed!", "Neo Studio", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);

            RefreshSoltionExplorer();
        }

        private void createWalletMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CreateWalletWindow createWalletWindow = new CreateWalletWindow();
            createWalletWindow.ShowDialog();
        }

        #region Window Event
        private void bookmarkWindowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Future!", "NEO Studo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void findResultsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Future!", "NEO Studo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void closeWindowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.Close();
        }

        private void closeAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Future!", "NEO Studo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("V1.0", "NEO Studo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #endregion

        #region ScintillaNet Event
        private void dockPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (ActiveDocument != null)
                ActiveDocument.Closing += Doc_Closing;
        }

        private void dockPanel_ActiveContentChanged(object sender, EventArgs e)
        {
            if (ActiveDocument != null)
            {
                this.Title = String.Format(CultureInfo.CurrentCulture, "{0} - {1}", ActiveDocument.Title, DocumentView.DocTitle);
                if (System.IO.Path.GetExtension(ActiveDocument.FilePath) == ".cs")
                {
                    Properties.Settings.Default.LastWalletPath = ActiveDocument.FilePath;
                    Properties.Settings.Default.Save();
                    CSharp.Compile();
                }
            }
            else
                this.Title = DocumentView.DocTitle;

        }
        #endregion

        #region ScintillaNet Methods
        private int _zoomLevel;
        private void InitScintillaDocument(DocumentView doc)
        {
            ScintillaMethods.InitBase(doc);

            if (lineNumbersMenuItem.IsChecked)
                doc.scintilla.Margins[ScintillaConstants.NUMBER_MARGIN].Width = ScintillaConstants.LINE_NUMBERS_MARGIN_WIDTH;
            else
                doc.scintilla.Margins[ScintillaConstants.NUMBER_MARGIN].Width = 0;

            // Turn on white space?
            if (whitespaceMenuItem.IsChecked)
                doc.scintilla.ViewWhitespace = WhitespaceMode.VisibleAlways;
            else
                doc.scintilla.ViewWhitespace = WhitespaceMode.Invisible;

            // Turn on word wrap?
            if (wordWrapMenuItem.IsChecked)
                doc.scintilla.WrapMode = WrapMode.Word;
            else
                doc.scintilla.WrapMode = WrapMode.None;

            // Show EOL?
            doc.scintilla.ViewEol = endOfLineMenuItem.IsChecked;

            // Set the zoom
            doc.scintilla.Zoom = _zoomLevel;
        }


        private void UpdateAllScintillaZoom()
        {
            foreach (DocumentView doc in documentsRoot.Children)
                doc.scintilla.Zoom = _zoomLevel;
        }

        private void OpenFolder()
        {
            string path = openFolderDialog.Show();
            //if (!string.IsNullOrEmpty(path))
            //    solutionExplorer.LoadFolder(path);
        }

        public void OpenFileFromOutside(string path)
        {
            bool isOpen = false;
            foreach (DocumentView documentForm in Documents)
            {
                if (path.Equals(documentForm.FilePath, StringComparison.OrdinalIgnoreCase))
                {
                    documentForm.IsActive = true;
                    isOpen = true;
                    break;
                }
            }

            if (!isOpen)
                OpenFile(path);
        }

        private void OpenFile()
        {
            bool? res = openFileDialog.ShowDialog();
            if (res == null || !(bool)res)
                return;

            foreach (string filePath in openFileDialog.FileNames)
            {
                bool isOpen = false;
                foreach (DocumentView documentForm in Documents)
                {
                    if (filePath.Equals(documentForm.FilePath, StringComparison.OrdinalIgnoreCase))
                    {
                        documentForm.IsActive = true;
                        isOpen = true;
                        break;
                    }
                }

                if (!isOpen)
                    OpenFile(filePath);
            }
        }

        public DocumentView OpenFile(string filePath)
        {
            DocumentView doc = new DocumentView();
            InitScintillaDocument(doc);
            doc.scintilla.Text = File.ReadAllText(filePath);
            doc.scintilla.EmptyUndoBuffer();
            doc.Title = System.IO.Path.GetFileName(filePath);
            doc.FilePath = filePath;
            documentsRoot.Children.Add(doc);
            doc.DockAsDocument();
            doc.IsActive = true;
            doc.ContentId = Guid.NewGuid().ToString();
            doc.Closing += Doc_Closing;
            RefreshSoltionExplorer();
            
            return doc;
        }

        private void Doc_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (documentsRoot.Children.Count == 1)
            {
                _newDocumentCount = 0;
                NewDocument();
            }
        }

        internal DocumentView NewDocument()
        {
            DocumentView doc = new DocumentView();
            InitScintillaDocument(doc);
            doc.Title = String.Format(CultureInfo.CurrentCulture, "{0}{1}", ScintillaConstants.NEW_DOCUMENT_TEXT, ++_newDocumentCount);
            documentsRoot.Children.Add(doc);
            doc.DockAsDocument();
            doc.IsActive = true;
            doc.ContentId = Guid.NewGuid().ToString();
            doc.Closing += Doc_Closing;
            doc.Title += " *";
            return doc;
        }
        #endregion

        public void RefreshSoltionExplorer()
        {
            if (ActiveDocument == null)
                return;
            string solutionPath = System.IO.Path.GetDirectoryName(ActiveDocument.FilePath);
            solutionExplorer.Clear();
            solutionExplorer.LoadFolder(solutionPath);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NeoStudio.Init.SyncBlockChain();
            NeoStudio.Init.OpenDefaultWallet();
            ((MainWindowViewModel)((NeoStudio.View.MainWindow)App.Current.MainWindow).DataContext).WalletList = new ObservableCollection<LocalWallet>(App.WalletSettings.LocalWallets);

            if (NeoStudio.NeoWallet.ActiveWallet != null)
            {
                NeoStudio.Init.CurrentWallet = NeoStudio.NeoWallet.ActiveWallet;
                NeoStudio.Init.ChangeWallet(NeoStudio.NeoWallet.ActiveWallet);
            }

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(NeoStudio.Init.timer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();

            //RefreshSoltionExplorer();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Blockchain.PersistCompleted -= NeoStudio.Init.Blockchain_PersistCompleted;
            NeoStudio.Init.ChangeWallet(null);
            System.Windows.Application.Current.Shutdown();
        }

        #region Helpers
        private static Visibility Toggle(Visibility v)
        {
            if (v == Visibility.Visible)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }



        #endregion

        private void runContract_Click(object sender, RoutedEventArgs e)
        {
            DeployContractDialog deployContractDialog = new DeployContractDialog();
            deployContractDialog.ShowDialog();
        }

        private void StepInCode_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}
