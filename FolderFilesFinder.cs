

using System.Diagnostics.Metrics;

namespace FolderFilesFinder
{
    class FolderFilesFinder
    {
        public static List<string> getAllSubfolders(string folderPath, int blacklistNumber)
        {
            List<string> folders = new List<string>();
            folders.Add(folderPath);
            int counter = 0;
            while (folders.Count > counter)
            { 
                folders.AddRange(getFoldersInFolder(folders[counter], blacklistNumber));
                counter++;
            }
            return folders;
        }

        static List<string> getFoldersInFolder(string folder, int blacklistNumber)
        {
            List<string> subFolders = Directory.GetDirectories(folder).ToList<string>();
            for (int i = 0; i < subFolders.Count; i++)
            {
                if (Setup.Setup.checkIfFolderInBlacklist(subFolders[i].Substring(subFolders[i].LastIndexOf("\\")+1), blacklistNumber))
                {
                    subFolders.RemoveAt(i);
                    i--;
                }
            }
            return subFolders;
        }

        public static List<List<string>> getFilesInFolders(List<string> folders)
        {
            List<List<string>> files = new List<List<string>>();
            foreach (string folder in folders)
            {
                files.Add(getFilesInFolder(folder));
            }
            return files;
        }

        public static List<string> getFilesInFolder(string folder)
        {
            List<FileInfo> fileInfos = new DirectoryInfo(folder).GetFiles().OrderBy(p => p.CreationTime).ToList();
            List<string> files = new List<string>();
            foreach (FileInfo fileInfo in fileInfos)
            {
                Console.WriteLine(fileInfo.Name.ToString() + ": " + fileInfo.CreationTime.ToString());
                files.Add(fileInfo.Name);
            }
            return files;
        }
    }
}