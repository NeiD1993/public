using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TypingTest.Model.TextModel;
using TypingTest.Resource.Class;

namespace TypingTest.Service
{
    static class DirectoryService
    {
        private static readonly string emptyTextsDirectoryMessage = "There is no one text with content in folder.";

        private static readonly string existentTextsDirectoryFileMessage = " already exists in directory.";

        private static readonly string fileDownloadingErrorMessage = " can't be downloaded.";

        private static readonly string nonexistentTextsDirectoryFileMessage = "File is not found.";

        private static readonly string nonexistentTextsDirectoryMessage = "Directory doesn't exist.";

        private static readonly string succesfullyDownloadedFileMessage = " is successfully downloaded.";

        private static readonly string textsDirectoryPath = "Texts";

        private static readonly string textsSearchPattern = "*" + textsExtension;

        private static readonly string wrongFileNameOrFileDirectoryNameFormat = "File names of file directory names have wrong format.";

        public static readonly string textsExtension = ".txt";

        private static List<FileInfo> GetTextsDirectoryFiles(string textsSearchPattern)
        {
            return new List<FileInfo>(new DirectoryInfo(textsDirectoryPath).GetFiles(textsSearchPattern, SearchOption.AllDirectories)
                                                                           .Where(textsDirectoryFile => ((textsDirectoryFile.Length > 0) && 
                                                                                                         (RegularExpressionsService.HasInputStringPatternStringFormat(MainTextModel.textNameValuePattern,
                                                                                                                                                                      textsDirectoryFile.Name)) && 
                                                                                                         (RegularExpressionsService.HasInputStringPatternStringFormat(MainTextModel.textDirectoryNameValuePattern,
                                                                                                                                                                      textsDirectoryFile.DirectoryName)))));
        }

        public static bool AreFilesSuccessfullyDownloadedToTextsDirectory(string[] fileNames)
        {
            bool areFilesSuccessfullyDownloadedToTextsDirectory = false;
            if (fileNames != null && fileNames.Length > 0)
            {
                bool hasInputFileNameRightFormat = false;
                string shortFileName, textsDirectoryFileName;
                foreach (string fileName in fileNames)
                {
                    try
                    {
                        shortFileName = Path.GetFileName(fileName);
                        hasInputFileNameRightFormat = (RegularExpressionsService.HasInputStringPatternStringFormat(MainTextModel.textNameValuePattern, shortFileName) &&
                                                       RegularExpressionsService.HasInputStringPatternStringFormat(MainTextModel.textDirectoryNameValuePattern, Path.GetDirectoryName(fileName)));
                    }
                    catch { return areFilesSuccessfullyDownloadedToTextsDirectory; }
                    if (hasInputFileNameRightFormat)
                    {
                        {
                            textsDirectoryFileName = Path.Combine(textsDirectoryPath, shortFileName);
                            shortFileName = "\"" + shortFileName + "\"";
                            if (!File.Exists(textsDirectoryFileName))
                            {
                                try { File.Copy(fileName, textsDirectoryFileName); }
                                catch
                                {
                                    MessageBox.Show(shortFileName + fileDownloadingErrorMessage);
                                    return areFilesSuccessfullyDownloadedToTextsDirectory;
                                }
                                MessageBox.Show(shortFileName + succesfullyDownloadedFileMessage);
                                areFilesSuccessfullyDownloadedToTextsDirectory = true;
                            }
                            else MessageBox.Show(shortFileName + existentTextsDirectoryFileMessage);
                        }
                    }
                    else MessageBox.Show(wrongFileNameOrFileDirectoryNameFormat);
                }
            }
            return areFilesSuccessfullyDownloadedToTextsDirectory;
        }

        public static string ReturnTextsDirectoryFileContent(MainTextModel mainTextModel)
        {
            string textDirectoryFileContent = String.Empty;
            if (mainTextModel != null)
            {
                List<FileInfo> textsDirectoryFiles = GetTextsDirectoryFiles(mainTextModel.TextName);
                if (textsDirectoryFiles.Exists(file => file.EqualsMainTextModel(mainTextModel)))
                {
                    using (StreamReader mainTextModelStreamReader = new StreamReader(mainTextModel.TextDirectoryName + "\\" + mainTextModel.TextName, Encoding.Default))
                        textDirectoryFileContent = mainTextModelStreamReader.ReadToEnd();
                }
            }
            if (textDirectoryFileContent == String.Empty)
                MessageBox.Show(nonexistentTextsDirectoryFileMessage);
            return textDirectoryFileContent;
        }

        public static List<FileInfo> GetTextsDirectoryFiles()
        {
            List<FileInfo> textsDirectoryFiles = null;
            if (Directory.Exists(textsDirectoryPath))
            {
                textsDirectoryFiles = GetTextsDirectoryFiles(textsSearchPattern);
                if (textsDirectoryFiles.Count == 0)
                    MessageBox.Show(emptyTextsDirectoryMessage);
            }
            else MessageBox.Show(nonexistentTextsDirectoryMessage);
            return textsDirectoryFiles;
        }
    }
}
