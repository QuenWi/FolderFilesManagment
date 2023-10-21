﻿
using FolderFilesFinder;
using System.IO.Enumeration;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Menue
{
    class Menue
    {
        public static string folderPath = "";

        static void Main(string[] args)
        {
            Console.WriteLine("starting...");
            chooseFolder();
            menue();
        }

        static void menue()
        {
            Console.WriteLine("\nmenue...\n1: Rename Files\n2: Link new Files\n3: Rename Files and link renamed\n9: Change Folder");
            string choosenOption = Console.ReadLine();
            if(choosenOption == "1")
            {
                renameFiles(false);
            } else if (choosenOption == "2")
            {
                linkNewFiles();
            }
            else if (choosenOption == "3")
            {
                renameAndLinkThem();
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

        static void renameFiles(bool linkRenamedFiles)
        {
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
                if(!renameFilesInFolder(folders[i], files[i], false, linkRenamedFiles))
                {
                    Console.WriteLine("Error: Start troubleshooting.");
                    if (!renameFilesInFolder(folders[i], FolderFilesFinder.FolderFilesFinder.getFilesInFolder(folders[i]), true, false))
                    {
                        Console.WriteLine("Error in troubleshooting!");
                        Environment.Exit(0);
                    }
                    else
                    {
                        if (!renameFilesInFolder(folders[i], FolderFilesFinder.FolderFilesFinder.getFilesInFolder(folders[i]), false, false))
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
            (List<string> folders, List<List<string>> files) = getFolders(2);
            Console.WriteLine("What is the maximum age in days for files to get linked?:");
            float daysAgo = float.Parse(Console.ReadLine());
            DateTime now = DateTime.Now.AddDays(-daysAgo);
            Console.WriteLine("Start linking new files.");
            Setup.Setup.deleteAllNewFiles();
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
                        Setup.Setup.copyOrLinkFile(file);
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
            renameFiles(true);
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

        static bool renameFilesInFolder(string folder, List<string> files, bool troubleshooting, bool linkRenamed)
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
                        Setup.Setup.writeLog(file, prediction, now);
                        if (linkRenamed && !troubleshooting)
                        {
                            Setup.Setup.copyOrLinkFile(prediction);
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}