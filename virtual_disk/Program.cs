using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace virtual_disk
{
    class Program : Input
    {
        public static Directory root = new Directory(new char[] { 'H', ':','/' }, 0x10, 5, null);
        public static Directory currentDirectory = root;
        public static string currentPath;
        public static void UpdateCurrentPath()
        {
           
            currentPath = new string(currentDirectory.directoryName);
        }
        //=currentDirectory.directoryName;


        public static void Main(string[] args)
        {


            CmdFormat cmdFormat = new CmdFormat();
            cmdFormat.WriteCmdLiscence();
            VirtualDisk.Initialize("biwwnaryFile.txt");
            UpdateCurrentPath();
            //FAT.printFat();




            //FAT.printFat();
            //currentDirectory.ReadDirectory();
            //Directory dir1 = Commands.MakeDirectoryFromDirectoryEntry(currentDirectory, 2);
            //Commands.CreateFile(dir1,"f", "mary");

            //DirectoryEntery file1 = new DirectoryEntery(new char[] { 'c' }, 0x0, 0);
            //currentDirectory.AddEntry(file1);
            //int index = currentDirectory.SearchForDirsAndFiles(file1.directoryName);
            //FileType newfile = Commands.MakeFileTypeFromDicrectoryEntry(currentDirectory,index);


            int index = currentDirectory.SearchForDirsAndFiles("h.txt".ToCharArray());
            //int length = "aaaaaaaaaaaa".Length;
            //FileType newFile = new FileType(new char[] { 'h', '.', 't', 'x', 't' }, 0x0, 0, length, "aaaaaaaaaaaa", currentDirectory);
            // if (index == -1)
            //{

            Commands.CreateFile(root, "t1.txt", "test File number 1");
            Commands.CreateFile(root, "t2.txt", "test File number 2");
            Commands.CreateFile(root, "t3.txt", "test File number 3");
            //}


            //Directory destinationDir = Commands.MakeDirectoryFromDirectoryEntry(currentDirectory, 0);
            //FileType newFile = new FileType(new char[] { 'i', '.', 't', 'x', 't' }, 0x0, 0, 0, "", destinationDir);
            //DirectoryEntery dirEntryThatRepresentsTheNewFile = newFile.GetDirectoryEntry();
            //destinationDir.ReadDirectory();
            //bool canAdd = destinationDir.CheckIfAnEntryCanBeAdded(dirEntryThatRepresentsTheNewFile);
            //if (canAdd)
            //{
            //    destinationDir.AddEntry(dirEntryThatRepresentsTheNewFile);
            //    newFile.WriteFile();
            //    destinationDir.WriteDirectory();
            //}
            //else
            //{
            //    Console.WriteLine("no space available");
            //}




            //CHeck read directory


            while (true)
            {
                cmdFormat.PrintPath();
                List<string> cmdCommand = GetCmdCommand();
                Menues menues = new Menues();
                Commands commands = new Commands();
                menues.Initialize_HelpMenues();
                commands.ExecuteCommands(cmdCommand, menues);
                Console.WriteLine();
            }



            // how to read input dir name
            //1 ) search for name, return index
            //2 ) go to the cluster of that directory and read it.
            // currentDirectory.directoryFisrtCluster = currentDirectory.DirsAndFiles[1].directoryFisrtCluster;
            // currentDirectory.ReadDirectory();

         

            

        }

       
    }
}
