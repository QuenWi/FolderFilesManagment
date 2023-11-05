
using System.Collections.Generic;
using System.Text;

namespace Setup
{
    class Setup
    {
        static HashSet<string> blacklistBoth = new HashSet<string>();
        static HashSet<string> blacklistRenamer = new HashSet<string>();
        static HashSet<string> blacklistLinkNew = new HashSet<string>();

        static string setupFolderPath = "";
        static string blacklistFilePath = "";
        public static string logsFilePath = "";

        static string newFilesPath = "";

        public static bool checkSetup(string folderPath)
        {
            setupFolderPath = folderPath + "\\000FolderFilesManagment";
            blacklistFilePath = setupFolderPath + "\\Blacklist.txt";
            logsFilePath = setupFolderPath + "\\logs.txt";
            newFilesPath = folderPath + "\\000NewFiles";
            if (Directory.Exists(setupFolderPath) && File.Exists(blacklistFilePath) && File.Exists(logsFilePath))
            {
                fillBlacklist();
                return true;
            }
            else
            {
                Console.WriteLine("You would like to create a setup in that folder? [y, n]:");
                string userInput = Console.ReadLine();
                if (userInput == "y" || userInput == "yes")
                {
                    createSetup();
                    fillBlacklist();
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
        }

        static void createSetup()
        {
            try
            {
                Directory.CreateDirectory(setupFolderPath);
                File.Create(blacklistFilePath).Close();
                File.Create(logsFilePath).Close();
                if (!File.Exists(Directory.GetCurrentDirectory() + "\\sortByOption.txt"))
                {
                    using (StreamWriter sw = File.CreateText(Directory.GetCurrentDirectory() + "\\sortByOption.txt"))
                    {
                        sw.WriteLine("1");
                    }
                }
                Directory.CreateDirectory(newFilesPath);
                using (StreamWriter logsStream = new StreamWriter(blacklistFilePath))
                {
                    logsStream.Write("Blacklist Both:\n-\n-\n-\n\nBlacklist Renamer:\n-\n-\n-\n\nBlacklist LinkNew:\n-\n-\n-\n\n");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.ToString());
            }
        }

        public static void fillBlacklist()
        {
            blacklistBoth.Add("000FolderFilesManagment");
            blacklistBoth.Add("000NewFiles");
            bool read = false;
            using (StreamReader blacklistStream = new StreamReader(blacklistFilePath))
            {
                readBlacklist(blacklistStream, blacklistBoth);
                readBlacklist(blacklistStream, blacklistRenamer);
                readBlacklist(blacklistStream, blacklistLinkNew);
            }
        }

        static void readBlacklist(StreamReader blacklistStream, HashSet<String> blacklist)
        {
            bool read = false;
            while (true)
            {
                string line = blacklistStream.ReadLine();
                if (line.StartsWith("-"))
                {
                    read = true;
                    blacklist.Add(line.Substring(1));
                }
                else if (read)
                {
                    read = false;
                    break;
                }
            }
        }

        public static bool checkIfFolderInBlacklist(string folderPath, int blacklistNumber)
        {
            if(blacklistNumber == 1) //Renamer
            {
                if (blacklistBoth.Contains(folderPath) || blacklistRenamer.Contains(folderPath))
                {
                    return true;
                }
            } else if (blacklistNumber == 2) //Link New
            {
                if (blacklistBoth.Contains(folderPath) || blacklistLinkNew.Contains(folderPath))
                {
                    return true;
                }
            }
            else if (blacklistNumber == 3) //Renamer and link renamed
            {
                if (blacklistBoth.Contains(folderPath) || blacklistRenamer.Contains(folderPath))
                {
                    return true;
                }
            }
            return false;
        }

        public static void writeLog(StreamWriter logsStream, string fileOriginalName, string fileNewName, DateTime now)
        {
            logsStream.WriteLine(now.ToString() + ": " + "\"" + fileOriginalName + "\" -> \"" + fileNewName + "\"");
        }

        public static void deleteAllNewFiles()
        {
            DirectoryInfo di = new DirectoryInfo(newFilesPath);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        public static void copyOrLinkFile(string file) {
            if (new FileInfo(file).Length < 1000000 * 50)
            {
                copyFile(file);
            }
            else
            {
                linkFile(file);
            }
        }

        public static void copyFile(string file)
        {
            string copyTo = newFilesPath + file.Substring(file.LastIndexOf("\\"));
            if (!File.Exists(copyTo))
            {
                File.Copy(file, copyTo);
            }
        }

        public static void linkFile(string file)
        {
            string linkTo = newFilesPath + file.Substring(file.LastIndexOf("\\")).Split(".")[0] + ".url";
            if (!File.Exists(linkTo))
            {
                using (StreamWriter writer = new StreamWriter(linkTo))
                {
                    writer.WriteLine("[InternetShortcut]");
                    writer.WriteLine("URL=file:///" + file);
                }
            }
        }

        public static void printBlacklist()
        {
            String[] lines = File.ReadAllLines(blacklistFilePath);
            for (var i = 0; i < lines.Length; i += 1)
            {
                Console.Out.WriteLine(lines[i]);
            }
        }
    } 
}