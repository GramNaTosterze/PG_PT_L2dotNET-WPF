using System;
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
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualBasic;

namespace prLab2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AboutDialog();
            dialog.ShowDialog();
        }
        private void OpenDir_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog() { Description = "Select directory to open" };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string root = dlg.SelectedPath;
                ProcessDir(dirTreeView, root);
            }

        }
        public void ProcessDir(ItemsControl root, string path)
        {
            FileSystemInfo currentDir = new FileInfo(path);
            string[] files = Directory.GetFiles(path);
            string[] subDirectories = Directory.GetDirectories(path);


            var dir = AddDirToTree(currentDir.Name, currentDir.FullName, root);

            foreach (string file in files)
            {
                FileInfo f = new FileInfo(file);
                AddFileToTree(f.Name, f.FullName, dir);
            }
            foreach (string subDirectory in subDirectories)
                ProcessDir(dir, subDirectory);
        }
        void CreateFile_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem target = (TreeViewItem)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget;
            string chosenDir = (string)target.Tag;
            CreateDialog create = new CreateDialog(chosenDir);
            create.ShowDialog();
            if (!create.fileCreated)
                return;
            if (create.IsDir())
                AddDirToTree(create.FileName(), create.FullName(), target);
            else
                AddFileToTree(create.FileName(), create.FullName(), target);
                
        }
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            
            TreeViewItem clickedObject = (TreeViewItem)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget;
            string content = File.ReadAllText((string)clickedObject.Tag);
            scrollViewer.Content = new TextBlock() { Text = content };
        }
        static void DeleteDir_Click(object sender, RoutedEventArgs e)
        {
            DeleteFromTree(sender, e);
            TreeViewItem clickedObject = (TreeViewItem)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget;
            DirectoryInfo dir = new DirectoryInfo((string)clickedObject.Tag);
            dir.DeleteAll();
        }
        static void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            DeleteFromTree(sender, e);
            TreeViewItem clickedObject = (TreeViewItem)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget;
            FileInfo file = new FileInfo((string)clickedObject.Tag);
            if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                file.Attributes = FileAttributes.Normal;
            file.Delete();
        }
        static TreeViewItem DeleteFromTree(object sender, RoutedEventArgs e)
        {
            TreeViewItem clickedObject = (TreeViewItem)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget;
            TreeViewItem parentObject = (TreeViewItem)clickedObject.Parent;
            parentObject.Items.Remove(clickedObject);
            return clickedObject;
        }
        TreeViewItem AddDirToTree(string name, string path, ItemsControl root)
        {
            var dir = new TreeViewItem { Header = name, Tag = path };
            dir.ContextMenu = new ContextMenu();

            MenuItem MICreate = new MenuItem() { Header = "Create" };
            MICreate.Click += CreateFile_Click;
            MenuItem MIDelete = new MenuItem() { Header = "Delete" };
            MIDelete.Click += DeleteDir_Click;

            dir.ContextMenu.Items.Add(MICreate);
            dir.ContextMenu.Items.Add(MIDelete);
            root.Items.Add(dir);

            return dir;
        }
        void AddFileToTree(string name, string path, ItemsControl dir)
        {
            var item = new TreeViewItem { Header = name, Tag = path };
            item.ContextMenu = new ContextMenu();
            MenuItem MIOpen = new MenuItem() { Header = "Open" };
            MIOpen.Click += OpenFile_Click;
            MenuItem MIDeleteFile = new MenuItem() { Header = "Delete" };
            MIDeleteFile.Click += DeleteFile_Click;
            item.ContextMenu.Items.Add(MIOpen);
            item.ContextMenu.Items.Add(MIDeleteFile);
            dir.Items.Add(item);
        }
        private void CloseDown_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void PrintRAHS(object sender, RoutedEventArgs e)
        {
            FileInfo selectedFile = new FileInfo((string)((TreeViewItem)dirTreeView.SelectedItem).Tag);
            rashBlock.Text = selectedFile.GetRAHS();
        }

    }

    public static class ExtendIO
    {
        public static string GetRAHS(this System.IO.FileSystemInfo io)
        {
            FileAttributes atr = io.Attributes;
            string ret = "";
            bool r = (atr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
            bool a = (atr & FileAttributes.Archive) == FileAttributes.Archive;
            bool h = (atr & FileAttributes.Hidden) == FileAttributes.Hidden;
            bool s = (atr & FileAttributes.System) == FileAttributes.System;
            ret += (r ? 'r' : '-');
            ret += (a ? 'a' : '-');
            ret += (h ? 'h' : '-');
            ret += (s ? 's' : '-');
            return ret;

        }
        public static void DeleteAll(this System.IO.DirectoryInfo io)
        {
            DirectoryInfo[] subDirectories = io.GetDirectories();
            FileInfo[] files = io.GetFiles();
            foreach (FileInfo file in files)
            {
                if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    file.Attributes = FileAttributes.Normal;
                file.Delete();
            }
            foreach (DirectoryInfo subDir in subDirectories)
            {
                subDir.DeleteAll();
            }
            if ((io.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                io.Attributes = FileAttributes.Normal;
            io.Delete();
        }
    }
}