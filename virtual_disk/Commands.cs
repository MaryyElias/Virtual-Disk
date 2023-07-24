using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace virtual_disk
{
    internal class Commands
    {
        CmdFormat cmdFormat = new CmdFormat();
        string spareCurrentPath = new string(Program.root.directoryName);

        public void ExecuteCommands(List<string> cmdCommand, Menues menues)
        {

            bool isPath = CheckIfArgumentIsPath(cmdCommand);
            bool isThereAnArgument = cmdCommand.Count > 1 ? true : false;
            Directory dirMovedTo;
            //  Program.currentDirectory.ReadDirectory();
            string pathInstring = GetPathInString(cmdCommand);
            if (cmdCommand.Count > 0)
            {

                switch (cmdCommand[0].ToUpper())
                {
                    case "CLS":
                        if (cmdCommand.Count > 1)
                        {
                            Console.WriteLine("Syntax of command is incorrect");
                            menues.key = cmdCommand[0].ToUpper();
                            menues.Print_Value_Of_Key();
                        }
                        else
                            Console.Clear();


                        break;
                    case "QUIT":
                        if (cmdCommand.Count > 1)
                        {
                            Console.WriteLine("Syntax of command is incorrect");
                            menues.key = cmdCommand[0].ToUpper();
                            menues.Print_Value_Of_Key();
                        }
                        else
                            Environment.Exit(-1);
                        break;
                    case "RD":


                        if (cmdCommand.Count > 1)
                        {
                            Directory directorySpecified1 = Program.currentDirectory;
                            string dirNameToRemove = "";


                            for (int indexOfCmd = 1; indexOfCmd < cmdCommand.Count; indexOfCmd ++)
                            {
                                dirNameToRemove = cmdCommand[indexOfCmd];
                                List<string> p1 = new List<string>();
                                p1.Add(cmdCommand[0]);
                                p1.Add(cmdCommand[indexOfCmd]);
                                isPath = CheckIfArgumentIsPath(p1);
                                if (isPath)
                                {

                                    string[] directories = new string[10];
                                    List<string> path = new List<string>();
                                    SplitPathToDirectories(p1, directories, path);
                                    dirNameToRemove = path[path.Count - 1];
                                    string pathToNewFile = "";
                                    for (int i = 0; i < path.Count - 1; i++)
                                    {
                                        pathToNewFile += path[i];
                                        if (i != 0 && i != path.Count - 2) pathToNewFile += '/';

                                    }
                                    pathToNewFile = "H:/";
                                    List<string> p = new List<string>();
                                    p.Add("spare");
                                    p.Add(pathToNewFile);

                                    directorySpecified1 = MoveToDir(p);
                                }

                                int indexOfDir = directorySpecified1.SearchForDirsAndFiles(dirNameToRemove.ToCharArray());
                                if (indexOfDir == -1)
                                    Console.WriteLine($"The system cannot find the directory specified \"{cmdCommand[indexOfCmd]}\".");
                                else
                                {
                                    Directory directoryToRemove = MakeDirectoryFromDirectoryEntry(directorySpecified1, indexOfDir);
                                    bool isDirEmpty = false;
                                    if (directoryToRemove.directoryFisrtCluster == 0)
                                        isDirEmpty = true;
                                    else
                                        isDirEmpty = false;

                                    if (isDirEmpty)
                                    {
                                        Console.WriteLine($"Are You sure you want to remove \"{dirNameToRemove}\" ,(Y/N)?");
                                        string input = Console.ReadLine();
                                        if (input.ToUpper() == "Y")
                                        {
                                            directoryToRemove.DeleteDirectory();
                                        }
                                        else break;
                                    }
                                    else
                                        Console.WriteLine("The directory is not empty.");
                                }
                            }


                        }
                        else
                        {
                            Console.WriteLine("Syntax of command is incorrect");
                            menues.key = cmdCommand[0].ToUpper();
                            menues.Print_Value_Of_Key();
                        }

                        break;
                    case "DIR":

                        bool isDir = true;
                        bool isRoot = true;
                        int numberOfDirs = 0;
                        Directory dirToDisplayContent = Program.currentDirectory;


                        if (isThereAnArgument)
                        {
                            if (isPath)
                            {
                                dirToDisplayContent = MoveToDir(cmdCommand);
                                if (dirToDisplayContent == null)
                                {
                                    Console.WriteLine($"no Such File Or Directory \"{pathInstring}\" ");
                                    return;
                                }
                                dirToDisplayContent.ReadDirectory();
                                isDir = dirToDisplayContent.directoryAttribute == 0x10 ? true : false;

                            }
                            else

                            {
                                int indexOfArgument = Program.currentDirectory.SearchForDirsAndFiles(cmdCommand[1].ToCharArray());
                                if (indexOfArgument != -1)
                                {
                                    dirToDisplayContent = MoveToDir(cmdCommand);

                                    if (dirToDisplayContent == null)
                                    {

                                        Console.WriteLine($"no Such File Or Directory \"{pathInstring}\" ");
                                        return;


                                    }
                                    dirToDisplayContent.ReadDirectory();
                                    isDir = dirToDisplayContent.directoryAttribute == 0x10 ? true : false;

                                }
                                else if (cmdCommand[1] == ".")
                                {
                                    dirToDisplayContent = Program.currentDirectory;
                                }
                                else if (cmdCommand[1] == "..")
                                {
                                    if (Program.currentDirectory != Program.root)
                                        dirToDisplayContent = Program.currentDirectory.parent;
                                }
                                else
                                {
                                    Console.WriteLine("no such file or directory");
                                    break;
                                }
                            }
                        }
                        else
                        {
                            dirToDisplayContent = Program.currentDirectory;
                            isDir = dirToDisplayContent.directoryAttribute == 0x10 ? true : false;
                        }

                        if (!isDir)
                        {
                            //print file info
                            DirectoryEntery dir = dirToDisplayContent.GetDirectoryEntry();
                            Console.WriteLine(dir.directoryFileSize.ToString() + '\t' + new string(dirToDisplayContent.directoryName));
                            //print dir info
                            dirToDisplayContent = Program.currentDirectory;
                            dirToDisplayContent.ReadDirectory();
                            int numberOfFiles = 0;
                            int sumOfFileSize = 0;

                            int sumOfDirSize = 0;

                            for (int i = 0; i < dirToDisplayContent.DirsAndFiles.Count; i++)
                            {
                                Directory dirToShowDetails = MakeDirectoryFromDirectoryEntry(dirToDisplayContent, i);
                                char[] dirName = new char[11];
                                char[] dirNameWithoutNullChar = new char[11];
                                dirName = dirToShowDetails.directoryName;
                                //to print dirName without control character
                                for (int j = 0; j < dirName.Length; j++)
                                {
                                    if (dirName[j] == '\u0010')
                                        break;
                                    dirNameWithoutNullChar[j] = dirName[j];


                                }

                                int fileSize = dirToShowDetails.GetMySizeonDisk() * 1024;

                                sumOfFileSize += fileSize;
                                numberOfFiles = 1;

                            }
                            int freeSpace = Program.currentDirectory.GetLogicalFreeSpace();
                            Console.WriteLine("{0} File(s) \t {1} bytes", numberOfFiles.ToString(), sumOfFileSize);
                            Console.WriteLine("{0} Dir(s) \t {1} bytes free ", numberOfDirs.ToString(), freeSpace);
                        }
                        else
                        {
                            if (new string(dirToDisplayContent.directoryName) != "H:/")
                            {
                                Console.WriteLine("<DIR>\t . ");
                                Console.WriteLine("<DIR>\t .. ");
                                numberOfDirs += 2;
                            }
                            dirToDisplayContent.ReadDirectory();
                            int numberOfFiles = 0;
                            int sumOfFileSize = 0;
                            int sumOfDirSize = 0;


                            for (int i = 0; i < dirToDisplayContent.DirsAndFiles.Count; i++)
                            {
                                Directory dirToShowDetails = MakeDirectoryFromDirectoryEntry(dirToDisplayContent, i);
                                char[] dirName = new char[11];
                                char[] dirNameWithoutNullChar = new char[11];
                                dirName = dirToShowDetails.directoryName;
                                //to print dirName without control character
                                for (int j = 0; j < dirName.Length; j++)
                                {
                                    if (dirName[j] == '\u0010')
                                        break;
                                    dirNameWithoutNullChar[j] = dirName[j];


                                }

                                int fileSize = dirToShowDetails.GetMySizeonDisk() * 1024;

                                sumOfFileSize += fileSize;
                                sumOfDirSize += fileSize;
                                if (dirToShowDetails.directoryAttribute == 0x10)
                                {
                                    Console.WriteLine("<DIR>\t {0} ", new string(dirNameWithoutNullChar));
                                    numberOfDirs++;


                                }
                                else
                                {
                                    Console.WriteLine("{0} \t {1}", fileSize.ToString(), new string(dirNameWithoutNullChar));
                                    numberOfFiles++;
                                }

                            }
                            int freeSpace = Program.currentDirectory.GetLogicalFreeSpace();
                            Console.WriteLine("{0} File(s) \t {1} bytes", numberOfFiles.ToString(), sumOfFileSize);
                            Console.WriteLine("{0} Dir(s) \t {1}  bytes free ", numberOfDirs.ToString(), freeSpace);
                        }
                        break;
                    case "MD":



                        if (cmdCommand.Count > 1)
                        {
                            Directory directorySpecified = Program.currentDirectory;
                            string newDirName = cmdCommand[1];
                            if (isPath)
                            {
                                //TODO: TEST
                                string[] directories = new string[10];
                                List<string> path = new List<string>();
                                SplitPathToDirectories(cmdCommand, directories, path);
                                newDirName = path[path.Count - 1];
                                string pathToNewDir = "";
                                for (int i = 0; i < path.Count - 1; i++)
                                {
                                    pathToNewDir += path[i];
                                    if (i != 0 && i != path.Count - 2) pathToNewDir += '/';

                                }
                                cmdCommand[1] = pathToNewDir;


                                directorySpecified = MoveToDir(cmdCommand);
                            }
                            DirectoryEntery newDir = new DirectoryEntery(newDirName.ToCharArray(), 0x10, 0);
                        

                            bool canAdd = true;
                            canAdd = directorySpecified.CheckIfAnEntryCanBeAdded(newDir);
                            int indexOfDir2 = directorySpecified.SearchForDirsAndFiles(newDirName.ToCharArray());
                            if (indexOfDir2 != -1) canAdd = false;

                            if (canAdd == false)
                                Console.WriteLine($"A subdirectory with the same name \"{newDirName}\" already exists.");
                            else

                            {
                                directorySpecified.AddEntry(newDir);

                                directorySpecified.WriteDirectory();
                            }

                        }

                        else
                        {
                            Console.WriteLine("Syntax of command is incorrect");
                            menues.key = cmdCommand[0].ToUpper();
                            menues.Print_Value_Of_Key();
                        }
                        break;
                    case "HELP":
                        if (cmdCommand.Count > 1)
                        {

                            menues.key = cmdCommand[1].ToUpper().Trim();
                            menues.Print_Value_Of_Key();
                        }
                        else

                            menues.Print_Content_Of_Help_Menue();
                        break;
                    case "RENAME":
                        if (cmdCommand.Count == 3)
                        {
                            Directory directorySpecified2 = Program.currentDirectory;
                            string fileNameToChange = cmdCommand[1];
                            if (isPath)
                            {

                                string[] directories = new string[10];
                                List<string> path = new List<string>();
                                SplitPathToDirectories(cmdCommand, directories, path);
                                fileNameToChange = path[path.Count - 1];
                                string pathToNewFile = "";
                                for (int i = 0; i < path.Count - 1; i++)
                                {
                                    pathToNewFile += path[i];
                                    if (i != 0 && i != path.Count - 2) pathToNewFile += '/';

                                }
                                cmdCommand[1] = pathToNewFile;


                                directorySpecified2 = MoveToDir(cmdCommand);
                            }
                            bool isNewNameAPath = false;
                            for (int i = 0; i < cmdCommand[2].Length; i++)
                            {
                                if (cmdCommand[2][i] == '/')
                                    isNewNameAPath = true;
                            }
                            if (isNewNameAPath)
                            {
                                Console.WriteLine("New name can't be a path");
                                break;
                            }

                            int indexOfFileToRename = directorySpecified2.SearchForDirsAndFiles(fileNameToChange.ToCharArray());

                            if (indexOfFileToRename != -1)
                            {
                                bool isFile = (Program.currentDirectory.DirsAndFiles[indexOfFileToRename].directoryAttribute == 0x10) ? false : true;
                                DirectoryEntery dirWithOldName = Program.currentDirectory.DirsAndFiles[indexOfFileToRename];
                                string oldName = new string(dirWithOldName.directoryName);

                                bool isThereAFileWithTheSameNewName =
                                    Program.currentDirectory.SearchForDirsAndFiles(cmdCommand[2].ToCharArray()) == -1 ? false : true;
                                if (isThereAFileWithTheSameNewName)
                                {
                                    Console.WriteLine("a file with the same name already exists");
                                }
                                else
                                {
                                    DirectoryEntery dirWithNewName = new DirectoryEntery(cmdCommand[2].ToCharArray(), dirWithOldName.directoryAttribute,
                                        dirWithOldName.directoryFisrtCluster);
                                    Program.currentDirectory.UpdateInfo(dirWithOldName, dirWithNewName);
                                    Program.currentDirectory.WriteDirectory();
                                }
                            }
                            else
                            {
                                Console.WriteLine("The system cannot find the file specified.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Syntax of command is incorrect");
                            menues.key = cmdCommand[0].ToUpper();
                            menues.Print_Value_Of_Key();
                        }

                        break;

                    case "COPY":
                        if (cmdCommand.Count > 1)
                        {
                            bool isThereADestination = cmdCommand.Count > 2 ? true : false;
                            //search for th source
                            int indexOfSource = Program.currentDirectory.SearchForDirsAndFiles(cmdCommand[1].ToCharArray());
                            int indexOfDestination = -1;
                            if (isThereADestination)
                            {
                                indexOfDestination = Program.currentDirectory.SearchForDirsAndFiles(cmdCommand[2].ToCharArray());

                            }

                            if (indexOfSource != -1)
                            {
                                if (!isThereADestination)
                                {
                                    bool isFile = (Program.currentDirectory.DirsAndFiles[indexOfSource].directoryAttribute == 0x10) ? false : true;
                                    //check if th source is file
                                    if (isFile)
                                    {
                                        FileType fileToCopy = MakeFileTypeFromDicrectoryEntry(Program.currentDirectory, indexOfSource);


                                        fileToCopy.ReadFile();
                                        Directory destination = Program.currentDirectory;
                                        int indexOfFileToCopyInDestination = destination.SearchForDirsAndFiles(fileToCopy.directoryName);
                                        if (indexOfFileToCopyInDestination != -1)
                                        {
                                            //Console.WriteLine("A fIle eith the same name already exists, Do you want to overwrite th file (Y/N)?");
                                            //string answer = Console.ReadLine();
                                            //if (answer == "Y")
                                            //{
                                            //    OverwriteFile(destination, cmdCommand[1], fileToCopy.fileContent);
                                            //}
                                            //else
                                            //    break;

                                            Console.WriteLine("A file cannot be copied onto itself");
                                        }
                                        else
                                        {
                                            CreateFile(destination, cmdCommand[1], fileToCopy.fileContent);
                                        }






                                    }
                                    else if (!isFile)
                                    {
                                        Directory dirToCopy = MakeDirectoryFromDirectoryEntry(Program.currentDirectory, indexOfSource);
                                        dirToCopy.ReadDirectory();
                                        for (int i = 0; i < dirToCopy.DirsAndFiles.Count; i++)
                                        {
                                            if (dirToCopy.DirsAndFiles[i].directoryAttribute == 0x10)
                                            {
                                                FileType fileToCopy = MakeFileTypeFromDicrectoryEntry(dirToCopy, i);
                                                fileToCopy.ReadFile();
                                                Directory destination = MakeDirectoryFromDirectoryEntry(Program.currentDirectory, indexOfDestination);
                                                int indexOfFileToCopyInDestination = destination.SearchForDirsAndFiles(fileToCopy.directoryName);
                                                if (indexOfFileToCopyInDestination != -1)
                                                {
                                                    Console.WriteLine("A fIle eith the same name already exists, Do you want to overwrite th file (Y/N)?");
                                                    string answer = Console.ReadLine();
                                                    if (answer == "Y")
                                                    {
                                                        OverwriteFile(destination, cmdCommand[1], fileToCopy.fileContent);
                                                    }
                                                    else
                                                        break;


                                                }
                                                else
                                                {
                                                    CreateFile(destination, cmdCommand[1], fileToCopy.fileContent);
                                                }


                                            }

                                        }



                                    }
                                }
                                else if (isThereADestination)
                                {
                                    bool isSourceAFile = (Program.currentDirectory.DirsAndFiles[indexOfSource].directoryAttribute == 0x10) ? false : true;
                                    if (!isSourceAFile)
                                    {
                                        Directory dirToCopy = MakeDirectoryFromDirectoryEntry(Program.currentDirectory, indexOfSource);
                                        dirToCopy.ReadDirectory();
                                        if (indexOfDestination != -1)
                                        {
                                            bool isDestinationADir = (Program.currentDirectory.DirsAndFiles[indexOfDestination].directoryAttribute == 0x10) ? true : false;
                                            if (isDestinationADir)
                                            {
                                                Directory destination = MakeDirectoryFromDirectoryEntry(Program.currentDirectory, indexOfDestination);
                                                destination.ReadDirectory();

                                                for (int i = 0; i < dirToCopy.DirsAndFiles.Count; i++)
                                                {
                                                    if (dirToCopy.DirsAndFiles[i].directoryAttribute == 0x10)
                                                    {
                                                        FileType fileToCopy = MakeFileTypeFromDicrectoryEntry(dirToCopy, i);
                                                        fileToCopy.ReadFile();

                                                        int indexOfFileToCopyInDestination = destination.SearchForDirsAndFiles(fileToCopy.directoryName);
                                                        if (indexOfFileToCopyInDestination != -1)
                                                        {
                                                            Console.WriteLine("A file eith the same name already exists, Do you want to overwrite th file (Y/N)?");
                                                            string answer = Console.ReadLine();
                                                            if (answer == "Y")
                                                            {
                                                                OverwriteFile(destination, cmdCommand[1], fileToCopy.fileContent);
                                                            }
                                                            else
                                                                break;


                                                        }
                                                        else
                                                        {
                                                            CreateFile(destination, cmdCommand[1], fileToCopy.fileContent);
                                                        }


                                                    }
                                                }
                                            }
                                            else if (!isDestinationADir)
                                            {

                                                string contents = "";
                                                for (int i = 0; i < dirToCopy.DirsAndFiles.Count; i++)
                                                {
                                                    if (dirToCopy.DirsAndFiles[i].directoryAttribute == 0x10)
                                                    {
                                                        FileType fileToCopy = MakeFileTypeFromDicrectoryEntry(dirToCopy, i);
                                                        fileToCopy.ReadFile();

                                                        contents += fileToCopy.fileContent;
                                                    }
                                                }
                                                if (indexOfDestination != -1)
                                                {
                                                    Console.WriteLine("A file eith the same name already exists, Do you want to overwrite th file (Y/N)?");
                                                    string answer = Console.ReadLine();
                                                    if (answer == "Y")
                                                    {
                                                        OverwriteFile(Program.currentDirectory, cmdCommand[1], contents);
                                                    }
                                                    else
                                                        break;
                                                }
                                                else
                                                {
                                                    //Probably broken
                                                    //FileType newFile = new FileType(cmdCommand[2].ToCharArray(), 0x0, 0,contents.Length, contents, Program.currentDirectory);
                                                    //Program.currentDirectory.AddEntry(newFile);
                                                    CreateFile(Program.currentDirectory, cmdCommand[1], contents);

                                                }


                                            }

                                        }
                                        else
                                        {
                                            Console.WriteLine("Destination Not Found.");
                                        }

                                    }
                                    else if (isSourceAFile)
                                    {
                                        FileType fileToCopy = MakeFileTypeFromDicrectoryEntry(Program.currentDirectory, indexOfSource);
                                        fileToCopy.ReadFile();
                                        if (indexOfDestination != -1)
                                        {
                                            bool isDestinationADir = (Program.currentDirectory.DirsAndFiles[indexOfDestination].directoryAttribute == 0x10) ? true : false;
                                            if (isDestinationADir)
                                            {
                                                Directory destination = MakeDirectoryFromDirectoryEntry(Program.currentDirectory, indexOfDestination);
                                                destination.ReadDirectory();





                                                int indexOfFileToCopyInDestination = destination.SearchForDirsAndFiles(fileToCopy.directoryName);
                                                if (indexOfFileToCopyInDestination != -1)
                                                {
                                                    Console.WriteLine("A file with the same name already exists, Do you want to overwrite th file (Y/N)?");
                                                    string answer = Console.ReadLine();
                                                    if (answer == "Y")
                                                    {
                                                        OverwriteFile(destination, cmdCommand[1], fileToCopy.fileContent);
                                                    }
                                                    else
                                                        break;


                                                }
                                                else
                                                {
                                                    CreateFile(destination, cmdCommand[1], fileToCopy.fileContent);
                                                    int index= destination.parent.SearchForDirsAndFiles(destination.directoryName);
                                                    DirectoryEntery old = destination.parent.DirsAndFiles[index];
                                                    DirectoryEntery newD = destination.GetDirectoryEntry();
                                                    destination.parent.UpdateInfo(old, newD);


                                                }




                                            }
                                            else if (!isDestinationADir)
                                            {

                                                if (indexOfDestination != -1)
                                                {
                                                    Console.WriteLine("A file eith the same name already exists, Do you want to overwrite th file (Y/N)?");
                                                    string answer = Console.ReadLine();
                                                    if (answer == "Y")
                                                    {
                                                        OverwriteFile(Program.currentDirectory, cmdCommand[1], fileToCopy.fileContent);
                                                    }
                                                    else
                                                        break;
                                                }
                                                else
                                                {
                                                    //FileType newFile = new FileType(cmdCommand[2].ToCharArray(), 0x0, 0, fileToCopy.fileContent.Length, fileToCopy.fileContent, Program.currentDirectory);
                                                    //Program.currentDirectory.AddEntry(newFile);
                                                    CreateFile(Program.currentDirectory, cmdCommand[1], fileToCopy.fileContent);

                                                }


                                            }

                                        }
                                        else
                                        {
                                            Console.WriteLine("destination is not found");
                                        }


                                    }
                                }


                            }
                            else
                            {
                                Console.WriteLine("no such file or directory");
                            }

                        }
                        else
                        {
                            Console.WriteLine("Syntax of command is incorrect");
                            menues.key = cmdCommand[0].ToUpper();
                            menues.Print_Value_Of_Key();
                        }
                        break;
                    case "IMPORT":
                        if (cmdCommand.Count > 1)
                        {
                            
                            string fileToImport = cmdCommand[1];
                            string fileContentToImport = "";
                            int isPathInSystem = fileToImport.IndexOf("\\");
                            if (isPathInSystem == -1)
                            {
                                int index = Program.currentDirectory.SearchForDirsAndFiles(fileToImport.ToCharArray());
                                if (index != -1)
                                {
                                    Console.WriteLine($"A file with the same name \"{fileToImport }\" already exists");
                                    break;
                                }
                            }
                            try
                            {
                                fileContentToImport = File.ReadAllText(fileToImport);
                               

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"\"{fileToImport }\" is not found on your system");
                                break;

                            }

                            Directory destonationDirectory = Program.currentDirectory;
                            if (isPath == true)
                            {
                                List<string> pathToGo = new List<string>();
                                pathToGo.Add(cmdCommand[0]);
                                pathToGo.Add(cmdCommand[2]);
                                destonationDirectory = MoveToDir(pathToGo);

                            }
                            int indexOfFileToCopyInDestination = destonationDirectory.SearchForDirsAndFiles(fileToImport.ToArray());

                            if (indexOfFileToCopyInDestination != -1)
                            {

                                Console.WriteLine("A file eith the same name already exists, Do you want to overwrite th file (Y/N)?");
                                string answer = Console.ReadLine();
                                if (answer == "Y")
                                {
                                    OverwriteFile(destonationDirectory, fileToImport, fileContentToImport);
                                }
                                else
                                    break;
                            }
                            else
                            {
                                CreateFile(destonationDirectory, fileToImport, fileContentToImport);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Syntax of command is incorrect");
                            menues.key = cmdCommand[0].ToUpper();
                            menues.Print_Value_Of_Key();
                        }
                        break;


                    case "EXPORT":
                        if (cmdCommand.Count > 1)
                        {
                            Directory destinationSpecipied = Program.currentDirectory;
                            int index = destinationSpecipied.SearchForDirsAndFiles(cmdCommand[1].ToCharArray());
                            if (index != -1)
                            {
                                FileType fileToExport = MakeFileTypeFromDicrectoryEntry(destinationSpecipied, index);
                                fileToExport.ReadFile();
                                string content = fileToExport.fileContent;
                                string fileName = cmdCommand[1];
                                using StreamWriter file = new(fileName);
                                file.Write(content);


                            }
                            else
                            {
                                Console.WriteLine($"\" {cmdCommand[1]} \" is not found");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Syntax of command is incorrect");
                            menues.key = cmdCommand[0].ToUpper();
                            menues.Print_Value_Of_Key();
                        }
                        break;

                    case "DEL":

                        if (cmdCommand.Count > 1)
                        {
                            Directory directorySpecified2 = Program.currentDirectory;
                            string fileNameToDelete = "";
                            for (int indexOfCmd = 1; indexOfCmd < cmdCommand.Count; indexOfCmd++)
                            {
                                fileNameToDelete = cmdCommand[indexOfCmd];
                                List<string> p1 = new List<string>();
                                p1.Add(cmdCommand[0]);
                                p1.Add(cmdCommand[indexOfCmd]);
                                isPath = CheckIfArgumentIsPath(p1);
                                if (isPath)
                                {

                                    string[] directories = new string[10];
                                    List<string> path = new List<string>();
                                    SplitPathToDirectories(p1, directories, path);
                                    fileNameToDelete = path[path.Count - 1];
                                    string pathToNewFile = "";
                                    for (int i = 0; i < path.Count - 1; i++)
                                    {
                                        pathToNewFile += path[i];
                                        if (i != 0 && i != path.Count - 2) pathToNewFile += '/';

                                    }
                                    pathToNewFile = "H:/";
                                    List<string> p = new List<string>();
                                    p.Add("spare");
                                    p.Add(pathToNewFile);

                                    directorySpecified2 = MoveToDir(p);
                                }
                                int index = Program.currentDirectory.SearchForDirsAndFiles(fileNameToDelete.ToCharArray());


                                if (index != -1)
                                {

                                    bool isFile = (directorySpecified2.DirsAndFiles[index].directoryAttribute == 0x10) ? false : true;
                                    if (isFile)
                                    {

                                        FileType FileToDelete = MakeFileTypeFromDicrectoryEntry(directorySpecified2, index);
                                        Console.WriteLine("Are you sure (Y/N)?");
                                        string answer = Console.ReadLine();
                                        if (answer.ToUpper() == "Y")
                                        {
                                            FileToDelete.DeleteFile();
                                        }
                                        else if (answer.ToUpper() == "N")
                                            break;
                                        else
                                        {
                                            Console.WriteLine($"Could Not Find{answer}");
                                        }
                                    }
                                    else
                                    {
                                        Directory dirTodelete = MakeDirectoryFromDirectoryEntry(directorySpecified2, index);
                                        dirTodelete.ReadDirectory();
                                        Console.WriteLine("Are you sure (Y/N)?");
                                        string answer = Console.ReadLine();
                                        if (answer.ToUpper() == "Y")
                                        {
                                            for (int i = 0; i < dirTodelete.DirsAndFiles.Count; i++)
                                            {
                                                if (dirTodelete.DirsAndFiles[i].directoryAttribute == 0x0)
                                                {

                                                    FileType FileToDelete = MakeFileTypeFromDicrectoryEntry(dirTodelete, i);
                                                    FileToDelete.DeleteFile();
                                                }
                                            }
                                        }
                                        else if (answer.ToUpper() == "N")
                                            break;
                                        else
                                        {
                                            Console.WriteLine($"Could Not Find{answer}");
                                        }

                                    }
                                }
                                else
                                {
                                    if (cmdCommand[indexOfCmd] == "H:/")
                                    {
                                        Directory dirTodelete = Program.root;
                                        dirTodelete.ReadDirectory();
                                        Console.WriteLine("Are you sure (Y/N)?");
                                        string answer = Console.ReadLine();
                                        if (answer.ToUpper() == "Y")
                                        {
                                            for (int i = 0; i < dirTodelete.DirsAndFiles.Count; i++)
                                            {
                                                if (dirTodelete.DirsAndFiles[i].directoryAttribute == 0x0)
                                                {

                                                    FileType FileToDelete = MakeFileTypeFromDicrectoryEntry(dirTodelete, i);
                                                    FileToDelete.DeleteFile();
                                                }
                                            }
                                        }
                                        else if (answer.ToUpper() == "N")
                                            break;
                                        else
                                        {
                                            Console.WriteLine($"Could Not Find{answer}");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("no such file or directory");
                                    }

                                }
                            }

                        }
                        else
                        {
                            Console.WriteLine("Syntax of command is incorrect");
                            menues.key = cmdCommand[0].ToUpper();
                            menues.Print_Value_Of_Key();
                        }
                        break;
                    case "TYPE":
                        if (cmdCommand.Count > 1)
                        {
                            for (int i = 1; i < cmdCommand.Count; i++)
                            {

                                Directory directorySpecified1 = Program.currentDirectory;
                                string fileNameToDisplay = "";
                                fileNameToDisplay = cmdCommand[i];
                                List<string> p1 = new List<string>();
                                p1.Add(cmdCommand[0]);
                                p1.Add(cmdCommand[i]);
                                isPath = CheckIfArgumentIsPath(p1);

                                if (isPath)
                                {

                                    List<string> sparelist = new List<string>();
                                    sparelist.Add(cmdCommand[0]);
                                    sparelist.Add(cmdCommand[i]);

                                    string[] directories = new string[10];
                                    List<string> path = new List<string>();
                                    SplitPathToDirectories(sparelist, directories, path);

                                    fileNameToDisplay = path[path.Count - 1];
                                    string pathToNewDir = "";

                                    for (int j = 0; j < path.Count - 1; j++)
                                    {
                                        if (path[j] == fileNameToDisplay)
                                            break;
                                        pathToNewDir += path[j];
                                        if (j != 0 && j != path.Count - 2) pathToNewDir += '/';

                                    }
                                    sparelist[1] = pathToNewDir;




                                    directorySpecified1 = MoveToDir(sparelist);
                                }
                                
                                    int index = Program.currentDirectory.SearchForDirsAndFiles(fileNameToDisplay.ToCharArray());

                                    if (index != -1)
                                    {
                                        FileType fileToDisplayContent = MakeFileTypeFromDicrectoryEntry(directorySpecified1, index);
                                        fileToDisplayContent.ReadFile();
                                        fileToDisplayContent.PrintContent();
                                    }
                                    else
                                    {
                                        Console.WriteLine($"The system cannot find file specified \" {directorySpecified1}\".");
                                    }
                                
                            }



                        }
                        else
                        {
                            Console.WriteLine("Syntax of command is incorrect");
                            menues.key = cmdCommand[0].ToUpper();
                            menues.Print_Value_Of_Key();
                        }
                        break;
                    case "CD":
                        if (isPath)
                        {
                            Program.currentDirectory = MoveToDir(cmdCommand);
                            Program.currentPath = spareCurrentPath;

                        }
                        else
                        {
                            if (cmdCommand.Count > 1)
                            {
                                if (cmdCommand[1] == "H:/")
                                {
                                    spareCurrentPath = "H:/";
                                    Program.currentPath = spareCurrentPath;
                                    Program.currentDirectory = Program.root;
                                }
                                else if (cmdCommand[1] == ".")
                                {

                                }
                                else if (cmdCommand[1] == "..")
                                {
                                    //TEST AND HANDEL
                                    if (Program.currentDirectory != Program.root)
                                    {
                                        Program.currentDirectory = Program.currentDirectory.parent;
                                        Directory sparedir = Program.currentDirectory;
                                        List<string> path = new List<string>();
                                        while (sparedir != null)
                                        {
                                            path.Add(new string(sparedir.directoryName));
                                            sparedir = sparedir.parent;

                                        }
                                        Program.currentPath = "";
                                        for (int i = path.Count - 1; i >= 0; i--)
                                        {
                                            if (path[i] != "H:/")
                                            { Program.currentPath += '/'; }
                                            Program.currentPath += path[i];
                                        }

                                    }
                                }
                                else
                                {
                                    //check if the argument is full path or dir
                                    //  bool isPath = CheckIfArgumentIsPath(cmdCommand);
                                    if (isPath)
                                    {
                                        Directory dirToGo = MoveToDir(cmdCommand);
                                        if (dirToGo != null)
                                        {
                                            Program.currentDirectory = dirToGo;
                                            Program.currentPath = spareCurrentPath;
                                        }
                                        else
                                        {

                                        }
                                    }
                                    else
                                    {
                                        int index = Program.currentDirectory.SearchForDirsAndFiles(cmdCommand[1].ToCharArray());
                                        if (index != -1)
                                        {

                                            //handel current path
                                            Directory dirToGo = MoveToDir(cmdCommand);
                                            if (dirToGo != null)
                                            {
                                                Program.currentPath = spareCurrentPath;
                                            }
                                            Program.currentDirectory = MakeDirectoryFromDirectoryEntry(Program.currentDirectory, index);
                                        }
                                        else
                                        {
                                            Console.WriteLine($"The system cannot find the path \"{pathInstring}\" specified.");
                                        }
                                    }


                                }
                            }
                            else
                            {
                                Console.WriteLine(Program.currentPath);
                            }
                        }

                        break;

                    default:
                        Console.WriteLine($"'{string.Concat(cmdCommand)}' is not recognized as an internal or external command, " +
                         "operable program or batch file.");



                        break;
                }
            }

            else
            {
                Console.WriteLine($"'{string.Concat(cmdCommand)}' is not recognized as an internal or external command, " +
                        "operable program or batch file.");
            }



        }

        private static string GetPathInString(List<string> cmdCommand)
        {
            string pathInString = "";

            for (int i = 1; i < cmdCommand.Count; i++)
            {
                pathInString += cmdCommand[i];
                if (cmdCommand[i] != "H:/" && i != cmdCommand.Count - 1)
                    pathInString += "/";
            }
            return pathInString;
        }

        public static FileType MakeFileTypeFromDicrectoryEntry(Directory dir, int index)
        {
            FileType fileToDisplayContent = new FileType(dir.DirsAndFiles[index].directoryName,
                dir.DirsAndFiles[index].directoryAttribute,
                dir.DirsAndFiles[index].directoryFisrtCluster, 0, "",
                dir);
            return fileToDisplayContent;

        }

        public static bool CheckIfArgumentIsPath(List<string> cmdCommand)
        {
            bool isPath = false;
            if (cmdCommand.Count > 1)
            {
                for (int i = 0; i < cmdCommand[1].Length; i++)
                {
                    if (cmdCommand[1][i] == '/')
                    {
                        isPath = true;
                    }

                }
                return isPath;
            }
            else
                return isPath;

        }
        public static bool CheckIfArgumentsAreParents(List<string> path)
        {
            bool isParent = false;
            if (path.Count > 1)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    if (path[i] == "..")
                        isParent = true;

                }
                return isParent;
            }
            else
                return isParent;

        }

        public Directory MoveToDir(List<string> cmdCommand)
        {



            Directory pathRequired;
            Directory dirFound = null;



            string[] directories = new string[10];
            List<string> path = new List<string>();
            //cmdCommand[1] =  dir1/dir2/dir3
            //path = dir1 dir2 dir3 ..
            SplitPathToDirectories(cmdCommand, directories, path);
            string pathInstring = GetPathInString(cmdCommand);
            bool isParent = CheckIfArgumentsAreParents(path);
            int indx = -1;
            try
            {
                indx = Program.currentDirectory.SearchForDirsAndFiles(path[0].ToCharArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (path[0] != "H:/" && indx == -1 && !isParent)
            {
                Console.WriteLine($"The system cannot find the path \"{pathInstring}\" specified.");
                return null;
            }
            else
            {

                bool isPath = CheckIfArgumentIsPath(cmdCommand);
                if (isPath)
                {
                    pathRequired = Program.root;


                    for (int i = 1; i < path.Count; i++)
                    {

                        int indexInDirsAndFiles
                             = pathRequired.SearchForDirsAndFiles(path[i].ToCharArray());
                        if (indexInDirsAndFiles == -1)
                        {
                            if (path[i] == "..")
                            {
                                if (Program.currentDirectory != Program.root)
                                {
                                    Program.currentDirectory = Program.currentDirectory.parent;
                                    Directory sparedir = Program.currentDirectory;
                                    List<string> path1 = new List<string>();
                                    while (sparedir != null)
                                    {
                                        path1.Add(new string(sparedir.directoryName));
                                        sparedir = sparedir.parent;

                                    }
                                    Program.currentPath = "";
                                    for (int j = path1.Count - 1; j >= 0; j--)
                                    {
                                        if (path1[j] != "H:/")
                                        { Program.currentPath += '/'; }
                                        Program.currentPath += path1[j];
                                    }

                                }
                            }
                            else
                            {
                                Console.WriteLine($"The system cannot find the path \"{pathInstring}\" specified.");
                                return null;
                            }


                        }
                        else
                        {

                            dirFound = MakeDirectoryFromDirectoryEntry(pathRequired, indexInDirsAndFiles);

                            //parentOfDirFound = dirFound;
                            pathRequired = dirFound;
                            if (Program.currentDirectory == Program.root)
                            {
                                spareCurrentPath += new string(pathRequired.directoryName);
                            }
                            else
                            {
                                //FIXXX

                                spareCurrentPath += '/';
                                spareCurrentPath += new string(pathRequired.directoryName);
                            }

                        }
                    }

                }
                else
                {
                    pathRequired = Program.currentDirectory;
                    for (int i = 0; i < path.Count; i++)
                    {

                        int indexInDirsAndFiles
                             = pathRequired.SearchForDirsAndFiles(path[i].ToCharArray());
                        if (indexInDirsAndFiles == -1)
                        {

                            Console.WriteLine($"The system cannot find the path \"{pathInstring}\"specified.");
                            return null;


                        }
                        else
                        {

                            dirFound = MakeDirectoryFromDirectoryEntry(pathRequired, indexInDirsAndFiles);

                            //parentOfDirFound = dirFound;
                            pathRequired = dirFound;
                            if (Program.currentDirectory == Program.root)
                            {
                                spareCurrentPath += new string(pathRequired.directoryName);
                            }
                            else
                            {
                                //FIXXX
                                spareCurrentPath = Program.currentPath;
                                spareCurrentPath += '/';
                                spareCurrentPath += new string(pathRequired.directoryName);
                            }

                        }
                    }
                }
            }

            return pathRequired;
        }

        private static void SplitPathToDirectories(List<string> cmdCommand, string[] directories, List<string> path)
        {
            int index = 0;
            int k = 0;
            while (k < cmdCommand[1].Length)
            {

                while (cmdCommand[1][k] != '/')
                {
                    directories[index] += cmdCommand[1][k];
                    k++;
                    if (k == cmdCommand[1].Length) break;


                }
                if (directories[index] == "H:")
                {
                    directories[index] += "/";
                }
                k++;
                path.Add(directories[index]);
                index++;

            }
        }

        public Directory MoveToFile(List<string> cmdCommand)
        {



            Directory pathRequired;
            Directory dirFound = null;

            string[] directories = new string[10];
            List<string> path = new List<string>();
            //cmdCommand[1] =  dir1/dir2/dir3
            //path = dir1 dir2 dir3 ..

            int index = 0;
            int k = 0;
            while (k < cmdCommand[1].Length)
            {

                while (cmdCommand[1][k] != '/')
                {
                    directories[index] += cmdCommand[1][k];
                    k++;
                    if (k == cmdCommand[1].Length) break;


                }
                if (directories[index] == "H:")
                {
                    directories[index] += "/";
                }
                k++;
                path.Add(directories[index]);
                index++;

            }

            //file name is stored at the end of path
            string fileName = path[path.Count - 1];
            //make path contains only the path without fileName
            path = path.SkipLast(1).ToList();

            string pathInstring = GetPathInString(cmdCommand);
            if (path[0] != "H:/" && Program.currentDirectory.SearchForDirsAndFiles(path[0].ToCharArray()) == -1)
            {
                Console.WriteLine($"The system cannot find the path\"{pathInstring}\" specified.");
                return null;
            }
            else
            {
                pathRequired = Program.root;
                for (int i = 0; i < path.Count; i++)
                {
                    int indexInDirsAndFiles
                         = pathRequired.SearchForDirsAndFiles(path[i].ToCharArray());
                    if (indexInDirsAndFiles == -1)
                    {
                        Console.WriteLine($"The system cannot find the path \"{pathInstring}\" specified.");
                        return null;

                    }
                    else
                    {
                        dirFound = MakeDirectoryFromDirectoryEntry(pathRequired, indexInDirsAndFiles);
                        //parentOfDirFound = dirFound;
                        pathRequired = dirFound;
                        if (Program.currentDirectory == Program.root)
                        {
                            spareCurrentPath += new string(pathRequired.directoryName);
                        }
                        else
                        {
                            //FIXXX
                            spareCurrentPath += '/';
                            spareCurrentPath += new string(pathRequired.directoryName);
                        }

                    }
                }
            }

            pathRequired.ReadDirectory();
            if (pathRequired.SearchForDirsAndFiles(fileName.ToCharArray()) != -1)
            {
                int indexOfdir = pathRequired.SearchForDirsAndFiles(fileName.ToCharArray());
                FileType fileFound = MakeFileTypeFromDicrectoryEntry(pathRequired, indexOfdir);
            }
            else
            {
                Console.WriteLine($"\n{fileName}\n is not found in specified path");
            }
            return pathRequired;
        }

        public static Directory MakeDirectoryFromDirectoryEntry
            (Directory directoryEntry, int indexInDirsAndFiles)
        {
            return new Directory(directoryEntry.DirsAndFiles[indexInDirsAndFiles].directoryName,
                                    directoryEntry.DirsAndFiles[indexInDirsAndFiles].directoryAttribute,
                                    directoryEntry.DirsAndFiles[indexInDirsAndFiles].directoryFisrtCluster,
                                   directoryEntry);
        }
        public static void CreateFile(Directory destinationDir, string fileName, string fileContent)
        {
            FileType newFile = new FileType(fileName.ToCharArray(), 0x0, 0, fileContent.Length, fileContent, destinationDir);
            DirectoryEntery dirEntryThatRepresentsTheNewFile = newFile.GetDirectoryEntry();
            bool canAdd = destinationDir.CheckIfAnEntryCanBeAdded(dirEntryThatRepresentsTheNewFile);
            if (canAdd)
            {

                destinationDir.AddEntry(dirEntryThatRepresentsTheNewFile);
                newFile.WriteFile();
                destinationDir.WriteDirectory();
            }
            else
            {
                Console.WriteLine("no space available");
            }

        }
        private static void OverwriteFile(Directory destinationDir, string fileName, string fileContent)
        {


            DirectoryEntery dirEntryThatRepresentsTheExistingFile;
            int index = destinationDir.SearchForDirsAndFiles(fileName.ToCharArray());
            dirEntryThatRepresentsTheExistingFile = destinationDir.DirsAndFiles[index];


            FileType oldFile = new FileType(dirEntryThatRepresentsTheExistingFile.directoryName,
                dirEntryThatRepresentsTheExistingFile.directoryAttribute, dirEntryThatRepresentsTheExistingFile.directoryFisrtCluster
                , fileContent.Length, fileContent, destinationDir);

            oldFile.ReadFile();
            oldFile.DeleteFile();
            FileType newFile = new FileType(fileName.ToCharArray(), 0x0, 0, fileContent.Length, fileContent, destinationDir);
            DirectoryEntery dirEntryThatRepresentsTheNewFile = newFile.GetDirectoryEntry();
            bool canAdd = destinationDir.CheckIfAnEntryCanBeAdded(dirEntryThatRepresentsTheNewFile);
            if (canAdd)
            {
                destinationDir.AddEntry(dirEntryThatRepresentsTheNewFile);
                newFile.WriteFile();



            }
            else
            {
                Console.WriteLine("no space available");
                DirectoryEntery dirEntryThatRepresentsTheOldFile = oldFile.GetDirectoryEntry();
                destinationDir.AddEntry(dirEntryThatRepresentsTheOldFile);
                oldFile.WriteFile();
            }


        }
    }
}
