using NeoStudio.Shared;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace NeoStudio.Model
{
    public class TreeViewItem : BaseViewModel
    {
        static string[] TEXT_EXTENSIONS = { ".CS" };
        FileInfo fileInfo;
        public FileInfo Info
        {
            get
            {
                return fileInfo;
            }
        }

        public bool IsText
        {
            get
            {
                return TEXT_EXTENSIONS.Contains(fileInfo.Extension.ToUpperInvariant());
            }
        }

        public bool IsOther
        {
            get
            {
                return !IsText;

            }
        }

        public String FileName
        {
            get
            {
                return String.IsNullOrWhiteSpace(fileInfo.Name) ? fileInfo.FullName : fileInfo.Name;
            }
        }

        public bool IsDirectory
        {
            get
            {
                return (fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
            }
        }

        public System.Windows.Media.ImageSource Icon
        {
            get
            {
                return GetImageSource();
            }
        }

        private System.Windows.Media.ImageSource GetImageSource()
        {
            string imageName = "Unknown.png";
            if (IsText)
            {
                imageName = "CSharp.png";
            }
            else if (IsDirectory)
            {
                imageName = "Directory.png";
            }
            return new System.Windows.Media.ImageSourceConverter().ConvertFromString("pack://application:,,,/Images/TreeView/" + imageName) as System.Windows.Media.ImageSource;
        }

        private ObservableCollection<TreeViewItem> children = new ObservableCollection<TreeViewItem>();
        public ObservableCollection<TreeViewItem> Children
        {
            get
            {
                return children;
            }
            set
            {
                children = value;
            }
        }
        public TreeViewItem() { }
        public TreeViewItem(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
        }

        //private bool isExpanded = false;
        //public bool IsExpanded
        //{
        //    get { return isExpanded; }
        //    set
        //    {
        //        if (isExpanded != value)
        //        {
        //            isExpanded = value;
        //            RaisePropertyChanged();
        //        }
        //    }
        //}

        //private bool isSelected = false;
        //public bool IsSelected
        //{
        //    get { return isSelected; }
        //    set
        //    {
        //        if (isSelected != value)
        //        {
        //            isSelected = value;
        //            RaisePropertyChanged();
        //        }
        //    }
        //}
    }
}
