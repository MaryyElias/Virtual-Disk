using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace virtual_disk
{
    internal class CmdFormat
    {
        public void WriteCmdLiscence()
        {
            Console.WriteLine("Microsoft Windows [Version 10.0.19044.1266]");
            Console.WriteLine("(c) Microsoft Corporation.All rights reserved. ");

        }
        public void PrintPath()
        {
           
            Console.Write(Program.currentPath + ">");

        }
    }
}
