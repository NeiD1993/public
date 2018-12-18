using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypingTest.Service
{
    static class OpenFileDialogService
    {
        private static readonly bool openFileDialogCheckFileExists = true;

        private static readonly bool openFileDialogCheckPathExists = true;

        private static readonly bool openFileDialogMultiselect = true;

        private static readonly string openFileDialogFilter = "Text documents (.txt)|*.txt";

        public static string[] ShowOpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = openFileDialogCheckFileExists;
            openFileDialog.CheckPathExists = openFileDialogCheckPathExists;
            openFileDialog.Multiselect = openFileDialogMultiselect;
            openFileDialog.Filter = openFileDialogFilter;
            openFileDialog.ShowDialog();
            return openFileDialog.FileNames;
        }
    }
}
