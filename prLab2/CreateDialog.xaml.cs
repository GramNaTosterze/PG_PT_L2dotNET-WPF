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
using System.Windows.Shapes;
using System.IO;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

namespace prLab2
{
    /// <summary>
    /// Logika interakcji dla klasy CreateDialog.xaml
    /// </summary>
    public partial class CreateDialog : Window
    {
        public bool fileCreated = false;
        private static bool isDir = true;
        private string destDir;
        private string fullPath;
        public CreateDialog(string chosenDir)
        {
            destDir = chosenDir;
            InitializeComponent();
        }

        private void CreateFile_Click(object sender, RoutedEventArgs e)
        {
            fullPath = destDir + "\\" + fileName.Text;
            if(isDir)
            {
                Directory.CreateDirectory(fullPath);
                SetSelectedAttributes(fullPath);
                fileCreated = true;
            }
            else if(Regex.IsMatch(fileName.Text, "\\w{8,}.(txt|php|html)"))
            {
                File.Create(fullPath);
                SetSelectedAttributes(fullPath);
                fileCreated = true;
            }

            if(fileCreated)
                this.Close();
        }
        private void SetSelectedAttributes(string path)
        {
            FileAttributes attributes = FileAttributes.Normal;
            if ((bool)readOnly.IsChecked)
                attributes |= FileAttributes.ReadOnly;
            if ((bool)archive.IsChecked)
                attributes |= FileAttributes.Archive;
            if ((bool)hidden.IsChecked)
                attributes |= FileAttributes.Hidden;
            if ((bool)system.IsChecked)
                attributes |= FileAttributes.System;
            File.SetAttributes(path, attributes);
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SetToDir(object sender, RoutedEventArgs e) { isDir = true; }
        private void SetToFile(object sender, RoutedEventArgs e) { isDir = false; }
        public string FullName() { return fullPath; }
        public string FileName() { return fileName.Text; }
        public bool IsDir() { return isDir; }
    }
}
