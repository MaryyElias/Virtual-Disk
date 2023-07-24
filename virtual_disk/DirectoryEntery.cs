using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace virtual_disk
{
    internal class DirectoryEntery
    {
        //total of 32 bytes
        public char[] directoryName= new char[11];
        public byte directoryAttribute;
        public byte[] directoryEmpty = new byte[12];
        public int directoryFisrtCluster;
        public int directoryFileSize;

        public DirectoryEntery(char[] directoryName, byte directoryAttribute,int directoryFisrtCluster)
        {
            this.directoryName = directoryName;
            this.directoryAttribute = directoryAttribute;
            this.directoryFisrtCluster = directoryFisrtCluster;
        }
        public DirectoryEntery()
        {
            this.directoryAttribute = 0;
            this.directoryFisrtCluster =0;
        }
    }
}
