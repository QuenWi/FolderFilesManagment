
namespace Menue
{
    class Menue
    {
        public static string folderPath = "";

        static void Main(string[] args)
        {
            Console.WriteLine("starting...");
            Console.WriteLine("Run on own risk. If you don't want to lose the files, than it can be recommanded to create a backup.");
            chooseFolder();
            menue();
        }

        static void menue()
        {
            Console.WriteLine("\nmenue...\n1: Rename Files\n2: Link new Files\n3: Rename Files and link renamed" +
                "\n7: Print Blacklist\n8: Change Sorting By Time\n9: Change Folder");
            string choosenOption = Console.ReadLine();
            if(choosenOption == "1")
            {
                renameFiles(false, 0);
            } else if (choosenOption == "2")
            {
                linkNewFiles();
            }
            else if (choosenOption == "3")
            {
                renameAndLinkThem();
            }
            else if (choosenOption == "7")
            {
                Setup.Setup.printBlacklist();
            }
            else if (choosenOption == "8")
            {
                changeSortBy();
            }
            else if (choosenOption == "9")
            {
                chooseFolder();
            }
            menue();
        }

        static void chooseFolder()
        {
            Console.WriteLine("What folder you want to run the FolderFileManagment?:");
            folderPath = Console.ReadLine();
            if (Directory.Exists(folderPath))
            {
                Console.WriteLine("Folder found.");
                if (Setup.Setup.checkSetup(folderPath))
                {
                    Console.WriteLine("Setup found.");
                }
                else
                {
                    Console.WriteLine("Setup not found.");
                    chooseFolder();
                }
            }
            else
            {
                Console.WriteLine("Folder not found.");
                chooseFolder();
            }
        }

        static void renameFiles(bool linkRenamedFiles, int option)
        {
            Setup.Setup.fillBlacklist();
            List<string> folders = null;
            List<List<string>> files = null;
            if (!linkRenamedFiles)
            {
                (folders, files) = getFolders(1);
            }
            else
            {
                (folders, files) = getFolders(3);
            }
            Console.WriteLine("Start renaming Files.");
            for (int i = 0; i < folders.Count; i++)
            {
                if(!renameFilesInFolder(folders[i], files[i], false, linkRenamedFiles, option))
                {
                    Console.WriteLine("Error: Start troubleshooting.");
                    if (!renameFilesInFolder(folders[i], FolderFilesFinder.FolderFilesFinder.getFilesInFolder(folders[i]), true, false, option))
                    {
                        Console.WriteLine("Error in troubleshooting!");
                        Environment.Exit(0);
                    }
                    else
                    {
                        if (!renameFilesInFolder(folders[i], FolderFilesFinder.FolderFilesFinder.getFilesInFolder(folders[i]), false, false, option))
                        {
                            Console.WriteLine("Error in troubleshooting!");
                            Environment.Exit(0);
                        }
                    }
                }
            }
        }

        static void linkNewFiles()
        {
            Setup.Setup.fillBlacklist();
            (List<string> folders, List<List<string>> files) = getFolders(2);
            Console.WriteLine("What is the maximum age in days for files to get linked?:");
            float daysAgo = float.Parse(Console.ReadLine());
            DateTime now = DateTime.Now.AddDays(-daysAgo);
            Console.WriteLine("Should the 000NewFiles folder be cleared before? [y]: ");
            string input = Console.ReadLine();
            if (input == "y" || input == "yes")
            {
                Setup.Setup.deleteAllNewFiles();
            }
            Console.WriteLine("What option for copy and linking you want to use?\n1: If file smaller than 50MB than copy else link." +
                "\n2: Link all\n3: Copy all");
            int option = int.Parse(Console.ReadLine());
            if (input == "y" || input == "yes")
            {
                Setup.Setup.deleteAllNewFiles();
            }
            Console.WriteLine("Start linking new files.");
            for (int i = 0; i < folders.Count; i++)
            {
                string folder = folders[i] + "\\";
                for (int fileNumber = 1; fileNumber <= files[i].Count; fileNumber++)
                {
                    string file = folder + files[i][fileNumber - 1];
                    DateTime creationDate = File.GetCreationTime(file);
                    if (now.CompareTo(creationDate) < 0)
                    {
                        //Console.WriteLine(filePath + ", " + Console.WriteLine(new FileInfo(filePath).Length) + ": " + creationDate.ToString() + " > " + now.ToString());
                        Setup.Setup.copyOrLinkFile(file, option);
                    }
                }
            }
        }

        static void renameAndLinkThem()
        {
            Console.WriteLine("Should the 000NewFiles folder be cleared before? [y]: ");
            string input = Console.ReadLine();
            if(input == "y" || input == "yes")
            {
                Setup.Setup.deleteAllNewFiles();
            }
            Console.WriteLine("What option for copy and linking you want to use?\n1: If file smaller than 50MB than copy else link." +
                "\n2: Link all\n3: Copy all");
            int option = int.Parse(Console.ReadLine());
            renameFiles(true, option);
        }

        static (List<string>, List<List<string>>) getFolders(int blacklistNumber)
        {
            List<string> folders = FolderFilesFinder.FolderFilesFinder.getAllSubfolders(folderPath, blacklistNumber);
            List<List<string>> files = FolderFilesFinder.FolderFilesFinder.getFilesInFolders(folders);
            return (folders, files);
        }

        static string fileNumberToString(int fileNumber)
        {
            string fileName = fileNumber.ToString();
            if (fileNumber / 10000 == 0)
            {
                if (fileNumber / 1000 != 0)
                {
                    fileName = "0" + fileName;
                }
                else if (fileNumber / 100 != 0)
                {
                    fileName = "00" + fileName;
                }
                else if (fileNumber / 10 != 0)
                {
                    fileName = "000" + fileName;
                }
                else
                {
                    fileName = "0000" + fileName;
                }
            }
            return fileName;
        }

        static bool renameFilesInFolder(string folder, List<string> files, bool troubleshooting, bool linkRenamed, int option)
        {
            string fileNames = "";
            DateTime now = DateTime.Now;
            if (!troubleshooting)
            {
                fileNames = folder + "\\" + folder.Substring(folder.LastIndexOf("\\")+1) + "_";
            }
            else
            {
                fileNames = folder + "\\Troubleshooting_";
            }
            using (StreamWriter logsStream = new StreamWriter(Setup.Setup.logsFilePath, append: true))
            {
                for (int fileNumber = 1; fileNumber <= files.Count; fileNumber++)
                {
                    string file = folder + "\\" + files[fileNumber - 1];
                    string prediction = fileNames + fileNumberToString(fileNumber) + file.Substring(file.LastIndexOf("."));
                    if (!prediction.Equals(file))
                    {
                        if (!File.Exists(prediction))
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            DateTime creationDate = fileInfo.CreationTime;
                            fileInfo.MoveTo(prediction);
                            File.SetCreationTime(prediction, creationDate); //to avoid the file system tunnelling. (wrong creationTime)
                            Setup.Setup.writeLog(logsStream, file, prediction, now);
                            if (linkRenamed && !troubleshooting)
                            {
                                Setup.Setup.copyOrLinkFile(prediction, option);
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        static void changeSortBy()
        {
            FolderFilesFinder.FolderFilesFinder.printSortByOption();
            Console.WriteLine("You would like to change after what to sort by? [y, n]:");
            string userInput = Console.ReadLine();
            if (userInput == "y" || userInput == "yes")
            {
                Console.WriteLine("1: LastWriteTime\n2: CreationTime");
                int sortByOption = int.Parse(Console.ReadLine());
                FolderFilesFinder.FolderFilesFinder.changeSortByOption(sortByOption);
            }
            Console.WriteLine("");
        }
    }
}