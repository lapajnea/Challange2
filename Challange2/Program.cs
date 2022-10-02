using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Challange2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //demo
            Console.WriteLine("Please insert folder path. (Eg. C:\\folder1\\folder2\\folder3\\...)");
            bool validInput = false;
            string rootPath = Console.ReadLine();
            Console.WriteLine();
            FileStructure folder = new FileStructure();
            folder = GetFoldersAndFiles($@"{rootPath}");

            Console.WriteLine("Do you want order folders and files by size or name?");
            string orderMode="";
            validInput = false;
            while(!validInput)
            {
                Console.WriteLine("Please insert order mode.");
                orderMode = Console.ReadLine();
                if(orderMode == "size" || orderMode == "name")
                {
                    validInput = true;
                }
                
            }
            validInput = false;
            Console.WriteLine("Do you want to have descending order?");
            var orderString = "";
            bool order= true;
            while(!validInput)
            {
                Console.WriteLine("Please insert 'yes' if you want descending order otherwise insert 'no'.");
                orderString = Console.ReadLine();
                if(orderString == "yes" || orderString == "no")
                {
                    validInput = true;
                    if (orderString == "yes")
                    {
                        order = true;
                    }
                    else { order = false; }
                }
                
            }

            if(orderMode == "size")
            {
                OrderBySizeDescending(folder, order);
            }
            else { OrderByNameDescending(folder, order); }

        }



        /// <summary>
        /// function find subfolders and files in given folder
        /// </summary>
        /// <param name="mainFolderPath"></param>
        /// <returns></returns>
        public static FileStructure GetFoldersAndFiles(string mainFolderPath)
        {
            FileStructure folderStructure = new FileStructure();
            Dictionary<string, long> FolderInfo = new Dictionary<string, long>();
            Dictionary<string, long> FileInfo = new Dictionary<string, long>();

            string rootPath = $@"{mainFolderPath}";
            if (!Directory.Exists(rootPath))
            {
                throw new Exception("The folder doesn't exist!");
            }

            folderStructure.MainFolder = mainFolderPath.Split('\\').Last();
            
            DirectoryInfo place = new DirectoryInfo(rootPath);
            FileInfo[] files = place.GetFiles(); //find files
            string[] fileNames = files.Select(x => x.Name).ToArray();
            foreach (var file in fileNames) //find sizes
            {
                //DirectoryInfo newDir = new DirectoryInfo($@"{rootPath}\{file}");
               // long size = newDir.EnumerateFiles().Sum(x => x.Length); ;
                long size = new FileInfo($@"{rootPath}\{file}").Length;
                FileInfo.Add(file.ToString(), size);
            }
            folderStructure.Files = FileInfo;
            folderStructure.SubFolders = FileInfo;


            string[] SubFolderPaths = Directory.GetDirectories(rootPath, "*", SearchOption.TopDirectoryOnly); // find subfolders paths

            List<string> SubFolderNames = new List<string>();
            foreach (var subFolder in SubFolderPaths) // add just folder name
            {
                string folder = subFolder.Split('\\').Last();
               
                SubFolderNames.Add(folder);   
            }
            foreach(var folder in SubFolderNames)
            {
                DirectoryInfo newDir = new DirectoryInfo($"{rootPath}\\{folder}");
                long size = FolderSize(newDir);
                FolderInfo.Add(folder.ToString(), size);
            }
            folderStructure.SubFolders = FolderInfo;

            return folderStructure;

        }


        /// <summary>
        /// function order folders by fist letter in name.
        /// if value is true is order descending, else ascending
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="value"></param>
        public static void OrderByNameDescending(FileStructure folder, bool value)
        {
            FileStructure fo = new FileStructure();
            Console.WriteLine($"Main Folder: {folder.MainFolder}");
            if (value)
            {
                fo.SubFolders = folder.SubFolders.OrderByDescending(x => x.Key[0]).ToDictionary(x => x.Key, y => y.Value);
                fo.Files = folder.Files.OrderByDescending(y => y.Key[0]).ToDictionary(x => x.Key, y => y.Value);
            }
            else
            {
                fo.SubFolders = folder.SubFolders.OrderBy(x => x.Key[0]).ToDictionary(x => x.Key, y => y.Value);
                fo.Files = folder.Files.OrderBy(y => y.Key[0]).ToDictionary(x => x.Key, y => y.Value);
            }

            Console.WriteLine("\nSub Folders:");
            foreach (var subFolder in fo.SubFolders)
            {
                Console.WriteLine(subFolder.Key);
            }
            Console.WriteLine("\nFiles:");
            foreach (var file in fo.Files)
            {
                Console.WriteLine(file.Key);
            }
        }


        public static void OrderBySizeDescending(FileStructure folder, bool value)
        {
            FileStructure fo = new FileStructure();
            Console.WriteLine($"Main Folder: {folder.MainFolder}");
            
            if (value)
            {
                fo.SubFolders = folder.SubFolders.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, y => y.Value);
                fo.Files = folder.Files.OrderByDescending(y => y.Value).ToDictionary(x => x.Key, y => y.Value);
            }
            else
            {
                fo.SubFolders = folder.SubFolders.OrderBy(x => x.Value).ToDictionary(x => x.Key, y => y.Value);
                fo.Files = folder.Files.OrderBy(y => y.Value).ToDictionary(x => x.Key, y => y.Value);
            }

            Console.WriteLine("\nSub Folders:");
            foreach (var subFolder in fo.SubFolders)
            {
                var size = BytesConverter(subFolder.Value);
                Console.WriteLine($"{subFolder.Key} size: {size}");
            }
            Console.WriteLine("\nFiles:");
            foreach (var file in fo.Files)
            {
                var size = BytesConverter(file.Value);
                Console.WriteLine($"{file.Key} size: {size}");
            }
        }

        /// <summary>
        /// function return size of a folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        static long FolderSize(DirectoryInfo folder)
        {
            long totalSizeOfDir = 0;
            FileInfo[] allFiles = folder.GetFiles();

            
            foreach (FileInfo file in allFiles)
            {
                totalSizeOfDir += file.Length;
            }
           
            DirectoryInfo[] subFolders = folder.GetDirectories();  // Find all subdirectories

            foreach (DirectoryInfo dir in subFolders)
            {
                totalSizeOfDir += FolderSize(dir);
            }           
            return totalSizeOfDir;
        }


       /// <summary>
       /// function convert from bytes to KB, MB or GB
       /// </summary>
       /// <param name="bytes"></param>
       /// <returns></returns>
       public static string BytesConverter(long bytes)
        {
            if(bytes > Math.Pow(10, 9))
            {
                double x = bytes / Math.Pow(2, 30);
                return $"{Math.Round(x,2)} GB";
            }
            if (bytes > Math.Pow(10, 7) && bytes < Math.Pow(10,9))
            {
                double x = bytes / Math.Pow(2, 20);
                return $"{Math.Round(x,2)} MB";
            }
            else
            {
                double x = bytes / Math.Pow(2, 10);
                return $"{Math.Round(x,0)} KB";
            }                

        }


    }
}
