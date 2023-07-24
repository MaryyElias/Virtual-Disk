using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace virtual_disk
{
    public static class VirtualDisk
    {
        static FileStream? disk;
        //intialize a new File or Opens an existing one
        
        public static void Initialize(string fileName)
        {
            
            

            if (!File.Exists(fileName))
            {
                disk = new FileStream(fileName, FileMode.Create);
                //is buffer intialized with zeroes in bytes by new???
                byte[] buffer = new byte[1024];
                WriteCluster(0, buffer);
                FAT.PrepareFAT();
                FAT.WriteFAT();
                Program.currentDirectory = Program.root;
            }
            else
            {
                disk = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite);
                FAT.ReadFAT();
                Program.root.ReadDirectory();
            }
          
        }
        public static void WriteCluster(int clusterIndex, byte[] buffer)
        {
            //go to the desired place
            disk?.Seek(clusterIndex * 1024, SeekOrigin.Begin);
            //write in fileStream "disk"
            disk?.Write(buffer, 0, buffer.Length);
            disk?.Flush();
        }
        public static byte[] ReadCluster(int clusterIndex)
        {
            //go to the desired place
            disk?.Seek(clusterIndex * 1024, SeekOrigin.Begin);
            byte[] buffer = new byte[1024];
            //read in buffer[]
            disk?.Read(buffer);
            return buffer;
        }
        public static int GetLogicalFreeSpace()
        {
            return 1024*1024 - (int) disk.Length;
        }

    }
}
