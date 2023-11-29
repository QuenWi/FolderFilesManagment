
namespace FolderFilesFinder
{
    class FolderFilesFinder
    {
        static int sortByOption = -1;

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
            setSortByOption();
            List<List<string>> files = new List<List<string>>();
            foreach (string folder in folders)
            {
                files.Add(getFilesInFolder(folder));
            }
            return files;
        }

        public static List<string> getFilesInFolder(string folder)
        {
            setSortByOption();
            List<FileInfo> fileInfos = null;
            if(sortByOption == 2)
            {
                fileInfos = new DirectoryInfo(folder).GetFiles().OrderBy(p => p.CreationTime).ToList();
            } else
            {
                fileInfos = new DirectoryInfo(folder).GetFiles().OrderBy(p => p.LastWriteTime).ToList();
            }
            List<string> files = new List<string>();
            foreach (FileInfo fileInfo in fileInfos)
            {
                files.Add(fileInfo.Name);
            }
            return files;
        }

        public static void printSortByOption()
        {
            string sortByFilePath = Directory.GetCurrentDirectory() + "\\sortByOption.txt";
            string sortByOptionString = File.ReadAllText(sortByFilePath);
            if (sortByOptionString == "2")
            {
                Console.Out.WriteLine("CreationTime");
            } else 
            {
                Console.Out.WriteLine("LastWriteTime");
            }
        }

        static void setSortByOption()
        {
            string sortByFilePath = Directory.GetCurrentDirectory() + "\\sortByOption.txt";
            string sortByOptionString = File.ReadAllText(sortByFilePath);
            try
            {
                sortByOption = int.Parse(sortByOptionString);
            } catch
            {
                Console.Out.WriteLine("Mistake with SortByOption. Please run menue option 8 and change the option!");
                string userInput = Console.ReadLine();
                Environment.Exit(1);
            }
            
        }

        public static void changeSortByOption(int option)
        {
            string sortByFilePath = Directory.GetCurrentDirectory() + "\\sortByOption.txt";
            if(!(option == 1 || option == 2))
            {
                option = 1;
            }
            using (StreamWriter logsStream = new StreamWriter(sortByFilePath, append: false))
            {
                logsStream.Write(option.ToString());
            }
        }
    }
}