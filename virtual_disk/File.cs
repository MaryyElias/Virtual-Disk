using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace virtual_disk
{
    internal class FileType : DirectoryEntery
    {
        public string fileContent;
        public int fileSize;
        Directory parent;
 
        public FileType(char[] directoryName, byte directoryAttribute, int directoryFisrtCluster, 
            int fileSize,string fileContent, Directory parent)
            : base(directoryName, directoryAttribute, directoryFisrtCluster)
        {
            this.fileContent = fileContent;
            this.parent = parent;      
            this.fileSize= fileSize;
            this.directoryFileSize = fileSize;
        }
    
        public DirectoryEntery GetDirectoryEntry()
        {
            DirectoryEntery myDir = new DirectoryEntery(this.directoryName, this.directoryAttribute, this.directoryFisrtCluster);
            myDir.directoryFileSize = this.fileSize;
            myDir.directoryEmpty = this.directoryEmpty;
            myDir.directoryFisrtCluster = this.directoryFisrtCluster;
            return myDir;

        }
        public int GetMySizeonDesk()
        {
            int size = 0;
            if (this.directoryFisrtCluster != 0)
            {
                int cluster = this.directoryFisrtCluster;
                int nextCLuster = FAT.GetClusterPointer(cluster);
                do
                {
                    size++;
                    cluster = nextCLuster;
                    if (cluster != -1)
                        nextCLuster = FAT.GetClusterPointer(cluster);
                } while (cluster != -1);

            }
            return size;
        }
        public void EmptyMyClusters()
        {

            if (this.directoryFisrtCluster != 0)
            {
                int cluster = this.directoryFisrtCluster;
                int nextCluster = FAT.GetClusterPointer(cluster);
                if (cluster == 5 && nextCluster == 0)
                    return; //root
                do
                {
                    FAT.SetClusterPointer(cluster, 0);
                    cluster = nextCluster;
                    if (cluster != -1)
                        nextCluster = FAT.GetClusterPointer(cluster);
                }
                while (cluster != -1);
            }
        }
        
        //how to implement update info?
        public void UpdateInfo(DirectoryEntery oldInfo, DirectoryEntery newInfo)
        {
            ReadFile();
            int index = parent.SearchForDirsAndFiles(oldInfo.directoryName);
            if (index != -1)
            {
                parent.DirsAndFiles.RemoveAt(index);
                parent.DirsAndFiles.Add(newInfo);
            }
        }
        public void WriteFile()
        {
            DirectoryEntery oldDirectory = GetDirectoryEntry();

            List<byte[]> clustersToWrite = new List<byte[]>();
            var buffer = new byte[fileContent.Length];
            int clusterIndex = 0;
            int lastCLuster = -1;
            int bufferIndex = 0;
            PutFileContentInBuffer(buffer, ref bufferIndex);
            DivideFileContentToClusters(buffer);
            //Do i need dirFirstClustrer?
            //yes, to know exactly where the file content is stored in FAT array.
            //no i don't need it because dirFirst cluster is already set by the parent
            SetDirFirstClusterIndex(ref clusterIndex);
            WriteBuffer(ref clusterIndex, ref lastCLuster);


            void PutFileContentInBuffer(byte[] buffer, ref int bufferIndex)
            {
                byte[] ConvertFileContentToByteArray()
                {
                    byte[] fileContentInBytes = Encoding.ASCII.GetBytes(this.fileContent);
                    return fileContentInBytes;
                }
                byte[] fileContent = ConvertFileContentToByteArray();
                for (int j = 0; j < fileContent.Length; j++)
                {
                    buffer[bufferIndex] = fileContent[j];
                    bufferIndex++;
                }
            }
            void DivideFileContentToClusters(byte[] buffer)
            {
                int numberOfClustersNeededToWriteFileContent;

                if (buffer.Length > 1024)
                {
                    numberOfClustersNeededToWriteFileContent =
                    ((buffer.Length % 1024) > 0) ? (buffer.Length / 1024) + 1 : (buffer.Length / 1024);
                    

                }
                else
                {
                    numberOfClustersNeededToWriteFileContent = 1;
                    
                }
                int bufferIndex2 = 0;
                int maxBufferIndex2 = buffer.Length - 1;
                for (int i = 0; i < numberOfClustersNeededToWriteFileContent; i++)
                {
                    byte[] splittedArrayOfBytes = new byte[1024];
                    for (int j = 0; j < 1024; j++)
                    {
                        
                        splittedArrayOfBytes[j] = buffer[bufferIndex2];
                        bufferIndex2++;
                        if (bufferIndex2 > maxBufferIndex2)
                        {
                            break;
                        }
                    }
                    clustersToWrite.Add(splittedArrayOfBytes);
                }
            }
            void SetDirFirstClusterIndex(ref int clusterIndex)
            {
                //when will it be equal to 0 ?
                //??????????
                if (directoryFisrtCluster == 0)
                {
                    clusterIndex = FAT.GetEmptyClusterIndex();
                    directoryFisrtCluster = clusterIndex;
                }
                //why setting pointers to 0?
                else
                {   //cluster =5  6  -1
                    //pnter  = 0  0 
                    //next = 6  -1
                    int nextClusterIndex = FAT.GetClusterPointer(clusterIndex);
                    do
                    {
                        FAT.SetClusterPointer(clusterIndex, 0);
                        clusterIndex = nextClusterIndex;
                        if (clusterIndex != -1)
                            nextClusterIndex = FAT.GetClusterPointer(clusterIndex);

                    }
                    while (clusterIndex != -1);

                    clusterIndex = FAT.GetEmptyClusterIndex();
                    directoryFisrtCluster = clusterIndex;

                }
            }
            //cluster Index was given a value according to function above
            void WriteBuffer(ref int clusterIndex, ref int lastCLuster)
            {
                for (int i = 0; i < clustersToWrite.Count; i++)
                {
                    VirtualDisk.WriteCluster(clusterIndex, clustersToWrite[i]);
                    FAT.SetClusterPointer(clusterIndex, -1);
                    if (lastCLuster != -1)
                    {
                        FAT.SetClusterPointer(lastCLuster, clusterIndex);
                    }
                    lastCLuster = clusterIndex;
                    clusterIndex = FAT.GetEmptyClusterIndex();
                }

            }

            DirectoryEntery newDirectory = GetDirectoryEntry();
            UpdateInfo(oldDirectory, newDirectory);
        }
        public void ReadFile()

        {
            if (directoryFisrtCluster != 0)
            {
                fileContent = null;
                int clusterIndexOfFileContentToRead = directoryFisrtCluster;
                int nextClusterIndexOfFilesContentToRead
                    = FAT.GetClusterPointer(clusterIndexOfFileContentToRead);
                do
                {

                    byte[] clusterToRead = VirtualDisk.ReadCluster(clusterIndexOfFileContentToRead);
                    int clusterToReadIndex = 0;
                    string s = null;
                    //each function here converts from byte to the required type before putting
                    //TODO:
                    // Functions that checks if the bytes have content 
                    PutContentInFileContent(clusterToRead, ref clusterToReadIndex,ref s);
                    fileContent += s;
                    clusterIndexOfFileContentToRead = nextClusterIndexOfFilesContentToRead;
                    if (nextClusterIndexOfFilesContentToRead != -1)
                    {
                        nextClusterIndexOfFilesContentToRead = FAT.GetClusterPointer(clusterIndexOfFileContentToRead);
                    }
                    else
                    {
                        //set cluster index to read to -1 to terminate the reading process
                        //am i suppose to run empty my cluster here?
                        clusterIndexOfFileContentToRead = -1;
                        //break form the for loop
                        break;
                    }


                }
                while (clusterIndexOfFileContentToRead != -1);
            }
          
            void PutContentInFileContent( byte[] clusterToRead, ref int clusterToReadIndex,ref string s)
            {
                byte[] byteArrayOfDirName = new byte[1024];
                for (int j = 0; j < 1024; j++)
                {
                    byteArrayOfDirName[j] = clusterToRead[clusterToReadIndex];
                    clusterToReadIndex++;
                }

                 s += Encoding.UTF8.GetString(byteArrayOfDirName).ToString();
            }
            

        }
        public void DeleteFile()
        {
            EmptyMyClusters();
            if(this.parent!=null)
            {
                this.parent.RemoveEntry(GetDirectoryEntry());
            }
        }
        public void PrintContent()
        {
            Console.WriteLine();
            Console.WriteLine($"{new string (directoryName) }");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(fileContent);
        }

    }
}
