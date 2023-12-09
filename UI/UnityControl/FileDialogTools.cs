using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;

namespace UnityControl
{
    public class FileDialogTools
    {
        public FileDialogTools() { }

        /// <summary>
        /// 打开文件选择框
        /// </summary>
        public static string OpenFileDialog(string title, string filter = "所有|*.*")
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            {
                dlg.CheckPathExists = true;
                dlg.AddExtension = true;
                dlg.Title = title;
                dlg.ValidateNames = true;
                //dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dlg.Filter = filter;
                dlg.FilterIndex = 1;

                if (dlg.ShowDialog() == true)
                {
                    return dlg.FileName;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 保存文件选择框
        /// </summary>
        public static string SaveFileDialog(string title, string filter = "所有|*.*")
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            {
                dlg.CheckPathExists = true;
                dlg.CheckFileExists = true;
                dlg.AddExtension = true;
                dlg.Title = title;
                dlg.ValidateNames = true;
                //dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dlg.Filter = filter;
                dlg.FilterIndex = 1;

                if (dlg.ShowDialog() == true)
                {
                    return dlg.FileName;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 打开文件夹选择框
        /// </summary>
        public static string OpenFolderBrowserDialog(string title)
        {
            var dlg = new CommonOpenFileDialog();
            {
                dlg.IsFolderPicker = true;
                dlg.EnsurePathExists = true;
                dlg.Title = title;
                //dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dlg.RestoreDirectory = true;

                if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    return dlg.FileName;
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
