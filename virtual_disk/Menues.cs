using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace virtual_disk
{
    internal class Menues
    {
        CmdFormat cmdFormat = new CmdFormat();

        Dictionary<string, string> menueInDetails =
                    new Dictionary<string, string>();

        Dictionary<string, string> helpMenu =
                new Dictionary<string, string>();
        public string key;

        public void Initialize_HelpMenues()
        {
            helpMenu.Add("cd", " - Change the current default directory to ." +
                    "\n           If the argument is not present, report the current directory." +
                    "\n           If the directory does not exist an appropriate" +
                    " error should be reported.");
            helpMenu.Add("cls", "- Clear the screen.");
            helpMenu.Add("dir", "- List the contents of directory .");
            helpMenu.Add("quit", "- Quit the shell.");
            helpMenu.Add("copy", "- Copies one or more files to another location.");
            helpMenu.Add("del", "- Deletes one or more files.");
            helpMenu.Add("help", "-Provides Help information for commands.");
            helpMenu.Add("md", "- Creates a directory.");
            helpMenu.Add("rd", "- Removes a directory.");
            helpMenu.Add("rename", "- Renames a file.");
            helpMenu.Add("type", "- Displays the contents of a text file.");
            helpMenu.Add("import", "– import text file(s) from your computer");
            helpMenu.Add("export", "– export text file(s) to your computer.");




            menueInDetails.Add("CD", "  Change the current default directory to the directory given in the argument.\nIf the argument is not present, report the current directory.\nIf the directory does not exist an appropriate error should be reported.\ncd command syntax is\ncd\nor\ncd[directory]\n[directory] can be directory name or fullpath of a directory");
            menueInDetails.Add("CLS", " Clear the screen.\ncls command syntax is\ncls.");
            menueInDetails.Add("DIR", " List the contents of directory given in the argument.\nIf the argument is not present, list the content of the current directory.\nIf the directory does not exist an appropriate error should be reported.\ndir command syntax is\ndir\nor\ndir[directory]\n[directory] can be directory name or fullpath of a directory or file name or full path of a file.");
            menueInDetails.Add("QUIT", " Quit the shell.\nquit command syntax is\nquit");
            menueInDetails.Add("COPY", " Copies one or more files to another location.\ncopy command syntax is \n copy[source] \nor \n copy[source] [destination] \n[source] can be file name (or fullpath or file) or directory name(or fullpath or directory) \n[destination] can be file name(or fullpath or file) or directory name or fullpath of a directory");
            menueInDetails.Add("DEL", " Deletes one or more files.\nNOTE: it confirms the user choice to delete the file before deleting\ndel command syntax is\ndel[dirFile] +\n+after[dirfile] represent that you can pass more than file Name(or fullpath of file) or directory name(or fullpath of directory)\n[dirfile] can be file Name(or fullpath of file) or directory name(or fullpath of directory).");
            menueInDetails.Add("MD", " Creates a directory.\nmd command syntax is\nmd[directory]\n[directory] can be a new directory name or fullpath of a new directory");
            menueInDetails.Add("RD", " Removes a directory.\nNOTE: it confirms the user choice to delete the directory before deleting\nrd command syntax is\nrd[directory] +\n[directory] can be a directory name or fullpath of a directory\n+ after[directory] represent that you can pass more than directory name(or fullpath of directory)");
            menueInDetails.Add("RENAME", " Renames a file.\nrename command syntax is\nrd[fileName][new fileName]\n[fileName] can be a file name or fullpath of a filename\n[new fileName] can be a new file name not fullpath");
            menueInDetails.Add("TYPE", " Displays the contents of a text file.\ntype command syntax is\ntype[file] +\nNOTE: it displays the filename before its content for every file\n[file] can be file Name(or fullpath of file) of text file\n+ after[file] represent that you can pass more than file Name(or fullpath of file).");
            menueInDetails.Add("IMPORT", " Import text file(s) from your computer\nimport command syntax is\nimport[source]\nor\nimport[source][destination]\n[source] can be file Name(or fullpath of file) or directory Name(or fullpath of directory) from your physical disk\n[destination] can be file Name(or fullpath of file) or directory name or fullpath of a directory");
            menueInDetails.Add("EXPORT", " Export text file(s) to your computer\nexport command syntax is\nexport[source]\nor\nexport[source][destination]\n[source] can be file Name(or fullpath of file) or directory Name(or fullpath of directory) from your virtual disk\n[destination] can be file Name(or fullpath of file) or directory name or fullpath of a directory."); ;
            menueInDetails.Add("HELP", " Provides help information for commands.\nhelp command syntax is  \n help \n or \n For more information on a specific command, type help[ Command ]\ncommand - displays help information on that command.");
        }
        public void Print_Content_Of_Help_Menue()
        {
            foreach (KeyValuePair<string, string> kvp in helpMenu)
            {
                Console.WriteLine("{0} \t {1}",
                    kvp.Key, kvp.Value);
            }
        }
        public void Print_Value_Of_Key()
        {

            string value;
            bool hasValue = menueInDetails.TryGetValue(key, out value);
            if (hasValue)
            {
                Console.WriteLine(value);
                
            }
            else
            {
                Console.WriteLine("This path " + key + " is not exists. ");
               
            }
        }

    }
}
