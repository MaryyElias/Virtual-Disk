using System.Text;

namespace virtual_disk
{
    internal class Directory : DirectoryEntery
    {
        public List<DirectoryEntery> DirsAndFiles;
        public Directory? parent;
        public
            Directory(char[] directoryName, byte directoryAttribute, int directoryFisrtCluster, Directory parent) :
            base(directoryName, directoryAttribute, directoryFisrtCluster)
        {
            this.parent = parent;
            this.DirsAndFiles = new List<DirectoryEntery>();
        }
        public void WriteDirectory()
        {
            DirectoryEntery oldDirectory = GetDirectoryEntry();

            //store info of each dir or File in dirsAndFiles list
            //as bytes in buffer array.
            //buffer will be written to disk filestream.
            var buffer = new byte[DirsAndFiles.Count * 32];

            //used to store arrays of bytes each of size 1024 OR LESS (if more than 1, size maybe less that 1024)
            //resulted from splitting buffer
            List<byte[]> listOfArrayOfBytes = new List<byte[]>();

            int clusterIndex = 0;
            int lastCLuster = -1;

            PutAttributesOfEachDirAndfFileToArrayOfBytes(buffer);
            SplitBufferToListOfArrayOfBytes(buffer, listOfArrayOfBytes);
            SetDirFirstClusterIndex(ref clusterIndex);
            WriteClustersOfListOfArrayOfBytes(listOfArrayOfBytes, ref clusterIndex, ref lastCLuster);

            //every function here contains sub function that fisrt converts before putting.
            void PutAttributesOfEachDirAndfFileToArrayOfBytes(byte[] buffer)
            {
                int bufferIndex = 0;

                for (int i = 0; i < DirsAndFiles.Count; i++)
                {
                    //can i remove the third parameter ref i ?                   
                    PutDirNameInBuffer(buffer, ref bufferIndex, ref i);
                    // bufferIndex = 11;
                    PutDirAttributeInBuffer(buffer, ref bufferIndex, ref i);
                    // bufferIndex = 12;
                    //when did i ever assign dir empty? does it have a default?
                    PutDirEmptyInBuffer(buffer, ref bufferIndex, ref i);
                    // bufferIndex = 24;


                    //when did i ever assign file size?
                    // bufferIndex = 28;
                    PutDirFileSizeInBuffer(buffer, ref bufferIndex, ref i);
                    PutDirFirstCLusterInBuffer(buffer, ref bufferIndex, ref i);



                    void PutDirNameInBuffer(byte[] buffer, ref int bufferIndex, ref int i)
                    {
                        byte[] ConvertDirectoryNameToByteArray(ref int i)
                        {
                            //is "encoding" function corrrect?
                            byte[] directoryNameInBytes = Encoding.ASCII.GetBytes(DirsAndFiles[i].directoryName);
                            return directoryNameInBytes;
                        }
                        byte[] dirName = ConvertDirectoryNameToByteArray(ref i);
                        for (int j = 0; j < dirName.Length; j++)
                        {
                            buffer[bufferIndex] = dirName[j];
                            bufferIndex++;
                        }
                        bufferIndex += (11 - dirName.Length);
                    }

                    void PutDirAttributeInBuffer(byte[] buffer, ref int bufferIndex, ref int i)
                    {
                        buffer[bufferIndex] = DirsAndFiles[i].directoryAttribute;
                        bufferIndex++;
                    }
                    void PutDirEmptyInBuffer(byte[] buffer, ref int bufferIndex, ref int i)
                    {
                        for (int j = 0; j < DirsAndFiles[i].directoryEmpty.Length; j++)
                        {
                            buffer[bufferIndex] = DirsAndFiles[i].directoryEmpty[j];
                            bufferIndex++;
                        }
                    }
                    void PutDirFirstCLusterInBuffer(byte[] buffer, ref int bufferIndex, ref int i)
                    {
                        System.Buffer.BlockCopy(new int[] { DirsAndFiles[i].directoryFisrtCluster }, 0, buffer, bufferIndex, 4);
                        //buffer[bufferIndex] = Convert.ToByte(DirsAndFiles[i].directoryFisrtCluster);
                        bufferIndex += 4;
                    }
                    void PutDirFileSizeInBuffer(byte[] buffer, ref int bufferIndex, ref int i)
                    {
                        System.Buffer.BlockCopy(new int[] { DirsAndFiles[i].directoryFileSize }, 0, buffer, bufferIndex, 4);
                        //buffer[bufferIndex] = Convert.ToByte(DirsAndFiles[i].directoryFileSize);
                        bufferIndex += 4;
                    }
                }
            }
            void SplitBufferToListOfArrayOfBytes(byte[] buffer, List<byte[]> listOfArrayOfBytes)
            {

                if (buffer.Length > 1024)
                {
                    int numberOfArrayOfbBytes = ((buffer.Length % 1024) > 0) ? (buffer.Length / 1024) + 1 : (1024);
                    int bufferIndex2 = 0;
                    //to handel exception (index out of range)
                    int bufferIndex2Max = buffer.Length - 1;

                    for (int i = 0; i < numberOfArrayOfbBytes; i++)
                    {
                        byte[] splittedArrayOfBytes = new byte[1024];
                        for (int j = 0; j < 1024; j++)
                        {
                            if (bufferIndex2 >= bufferIndex2Max)
                            {
                                splittedArrayOfBytes[j] = buffer[bufferIndex2];
                                bufferIndex2++;
                            }
                            else
                                break;
                        }
                        listOfArrayOfBytes.Add(splittedArrayOfBytes);
                    }

                }
                else if (buffer.Length == 1024)
                {
                    listOfArrayOfBytes.Add(buffer);
                }
                else
                {

                    int bufferIndex2 = 0;
                    //to handel exception (index out of range)
                    int bufferIndex2Max = buffer.Length - 1;


                    byte[] splittedArrayOfBytes = new byte[1024];
                    for (int j = 0; j < 1024; j++)
                    {
                        if (bufferIndex2 <= bufferIndex2Max)
                        {
                            splittedArrayOfBytes[j] = buffer[bufferIndex2];
                            bufferIndex2++;
                        }
                        else
                            break;
                    }
                    listOfArrayOfBytes.Add(splittedArrayOfBytes);

                }
            }
            //*****************************************************
            void SetDirFirstClusterIndex(ref int clusterIndex)
            {
                clusterIndex = this.directoryFisrtCluster;
                //why would anyone initialize dirFirstCluster to zero!
                //what if someone initialized it to 2 or 3 or any reserved cluster?
                if (directoryFisrtCluster == 0)
                {
                    clusterIndex = FAT.GetEmptyClusterIndex();
                    directoryFisrtCluster = clusterIndex;
                }
                else
                if (clusterIndex == 5 && FAT.GetClusterPointer(clusterIndex) == 0)
                { return; }//root}
                else
                {   //cluster =5  6  -1
                    //pnter  = 0  0 
                    //next = 6  -1
                    //TRACE

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

            void WriteClustersOfListOfArrayOfBytes(List<byte[]> listOfArrayOfBytes, ref int clusterIndex, ref int lastCLuster)
            {

                for (int i = 0; i < listOfArrayOfBytes.Count; i++)
                {
                    //cluster index is set in SetDirFirstClusterIndex
                    //and it's equal to dirFirstCluster
                    
                    VirtualDisk.WriteCluster(clusterIndex, listOfArrayOfBytes[i]);
                    FAT.SetClusterPointer(clusterIndex, -1); //means : this cluster is full
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
        public void ReadDirectory()

        {
            if (directoryFisrtCluster != 0)
            {
                DirsAndFiles = new List<DirectoryEntery>();
                int clusterIndexOfDirsAndFilesToRead = directoryFisrtCluster;
                int nextClusterIndexOfDirsAndFilesToRead
                    = FAT.GetClusterPointer(clusterIndexOfDirsAndFilesToRead);
                do
                {

                    byte[] clusterToRead = VirtualDisk.ReadCluster(clusterIndexOfDirsAndFilesToRead);


                    int clusterToReadIndex = 0;
                    int numberOfDirsOrFiles = clusterToRead.Length / 32;

                    for (int index = 0; index < numberOfDirsOrFiles;
                        index++)
                    {
                        DirectoryEntery directory = new DirectoryEntery();

                        //each function here converts from byte to the required type before putting
                        bool hasContent = false;
                        hasContent = CheckIfCurrentBytesHasContent(clusterToRead, clusterToReadIndex);
                        if (hasContent)
                        {
                            PutDirNameInDirsAndFiles(index, clusterToRead, ref clusterToReadIndex, directory); // 11bytes
                            PutDirAttributeInDirsAndFiles(clusterToRead, ref clusterToReadIndex, index, directory);// 1 byte
                            PutDirEmptyInDirsAndFiles(index, clusterToRead, ref clusterToReadIndex, directory);  // 12 bytes
                            PutDirFirstClusterInDirsAndFiles(index, clusterToRead, ref clusterToReadIndex, directory); // 4 bytes
                            PutDirFileSizeInDirsAndFiles(index, clusterToRead, ref clusterToReadIndex, directory); // 4 bytes
                            DirsAndFiles.Add(directory);
                            clusterIndexOfDirsAndFilesToRead = nextClusterIndexOfDirsAndFilesToRead;
                            if (nextClusterIndexOfDirsAndFilesToRead != -1)
                            {
                                nextClusterIndexOfDirsAndFilesToRead = FAT.GetClusterPointer(clusterIndexOfDirsAndFilesToRead);
                            }
                        }
                        else
                        {
                            //set cluster index to read to -1 to terminate the reading process
                            //am i suppose to run empty my cluster here?
                            clusterIndexOfDirsAndFilesToRead = -1;
                            //break form the for loop
                            break;
                        }
                    }

                }
                while (clusterIndexOfDirsAndFilesToRead != -1);
            }
            void PutDirAttributeInDirsAndFiles(byte[] clusterToRead, ref int clusterToReadIndex, int x, DirectoryEntery directory)
            {
                byte dirAttrubute = clusterToRead[clusterToReadIndex];
                directory.directoryAttribute = dirAttrubute;
                clusterToReadIndex++;

            }
            void PutDirNameInDirsAndFiles(int x, byte[] clusterToRead, ref int clusterToReadIndex, DirectoryEntery directory)
            {
                byte[] byteArrayOfDirName = new byte[11];
                for (int j = 0; j < 11; j++)
                {
                    byteArrayOfDirName[j] = clusterToRead[clusterToReadIndex];
                    clusterToReadIndex++;
                }

                directory.directoryName = Encoding.UTF8.GetString(byteArrayOfDirName).ToCharArray();
            }
            void PutDirEmptyInDirsAndFiles(int x, byte[] clusterToRead, ref int clusterToReadIndex, DirectoryEntery directory)
            {
                byte[] byteArrayOfDirEmpty = new byte[12];
                for (int j = 0; j < 12; j++)
                {
                    byteArrayOfDirEmpty[j] = clusterToRead[clusterToReadIndex];
                    directory.directoryEmpty[j] = byteArrayOfDirEmpty[j];
                    clusterToReadIndex++;
                }


            }
            void PutDirFirstClusterInDirsAndFiles(int x, byte[] clusterToRead, ref int clusterToReadIndex, DirectoryEntery directory)
            {
                byte[] arrayOfbytesOfDirectoryFirstCluster = new byte[4];
                for (int j = 0; j < 4; j++)
                {
                    arrayOfbytesOfDirectoryFirstCluster[j] = clusterToRead[clusterToReadIndex];
                    clusterToReadIndex++;
                }
                directory.directoryFisrtCluster
                    = BitConverter.ToInt32(arrayOfbytesOfDirectoryFirstCluster, 0);

            }
            void PutDirFileSizeInDirsAndFiles(int x, byte[] clusterToRead, ref int clusterToReadIndex, DirectoryEntery directory)
            {
                byte[] arrayOfbytesOfDirectoryFileSize = new byte[4];
                for (int j = 0; j < 4; j++)
                {
                    arrayOfbytesOfDirectoryFileSize[j] = clusterToRead[clusterToReadIndex];
                    clusterToReadIndex++;
                }
                directory.directoryFisrtCluster
                    = BitConverter.ToInt32(arrayOfbytesOfDirectoryFileSize, 0);

            }

            bool CheckIfCurrentBytesHasContent(byte[] clusterToRead, int clusterToReadIndex)
            {
                bool hasContent = false;
                byte[] bytesToRead = new byte[32];
                for (int i = 0; i < 32; i++)
                {
                    bytesToRead[i] = clusterToRead[clusterToReadIndex];
                    clusterToReadIndex++;
                }
                int[] intgerBytesToRead = new int[bytesToRead.Length / 4];
                System.Buffer.BlockCopy(bytesToRead, 0, intgerBytesToRead, 0, 32);
                //the idea is based on that an empty cluster in FAT
                //is firstly initialized to zeroes
                //otherwise, values are assigned to it.
                for (int i = 0; i < intgerBytesToRead.Length; i++)
                {
                    if (bytesToRead[i] != 0)
                        hasContent = true;
                }
                return hasContent;
            }

        }
        public DirectoryEntery GetDirectoryEntry()
        {
            DirectoryEntery myDir = new DirectoryEntery(this.directoryName, this.directoryAttribute, this.directoryFisrtCluster);
            myDir.directoryFileSize = this.directoryFileSize;
            myDir.directoryEmpty = this.directoryEmpty;
            return myDir;

        }
        public int SearchForDirsAndFiles(char[] name)
        {
            //Do i need to activate read directory?
            ReadDirectory();
            for (int i = 0; i < DirsAndFiles.Count; i++)
            {
                char[] dirName = DirsAndFiles[i].directoryName;
                bool equal = true;
                for (int j = 0; j < name.Length; j++)
                {
                    if (name[j] != dirName[j])
                    {
                        equal = false;
                        break;
                    }
                }
                if (equal)
                    return i;
            }
            return -1;
        }
        public void UpdateInfo(DirectoryEntery oldInfo, DirectoryEntery newInfo)
        {
            ReadDirectory();

          //  if (parent != null) { 
                int index = this.SearchForDirsAndFiles(oldInfo.directoryName);
                if (index != -1)
                {
                    this.DirsAndFiles.RemoveAt(index);
                    this.DirsAndFiles.Add(newInfo);
                }
               // }
            
        }
        public void RemoveEntry(DirectoryEntery directory)
        {
            ReadDirectory();
            int index = SearchForDirsAndFiles(directory.directoryName);
            DirsAndFiles.RemoveAt(index);
            WriteDirectory();
        }
        public void AddEntry(DirectoryEntery directory)
        {
            DirsAndFiles.Add(directory);
            WriteDirectory();
        }
        public void EmptyMyClusters()
        {
            //if dirFirstCluster == 0 , it's already empty
            //if it's root, return
            //otherwise, empty

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
        public void DeleteDirectory()

        {
            EmptyMyClusters();
            if (this.parent != null)
            {
                this.parent.RemoveEntry(GetDirectoryEntry());
            }
            if (Program.currentDirectory == this)
            {
                Console.WriteLine("this action can't be completed because the folder or file");
                Console.WriteLine("in it is open in another program");
            }

        }
        public int GetMySizeonDisk()
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
        public int GetLogicalFreeSpace()
        {
            return FAT.GetAvailableClusters() * 1024;
        }
        public bool CheckIfAnEntryCanBeAdded(DirectoryEntery directory)
        {
            bool canAddDirectory = false;
            int neededSize = (DirsAndFiles.Count + 1) * 32;
            int neededClusters = neededSize / 1024;
            int remainder = neededSize % 1024;
            if (remainder > 0)
                neededClusters++;

            neededClusters += directory.directoryFileSize / 1024;
            int remainder2 = directory.directoryFileSize % 1024;
            if (remainder2 > 0)
                neededClusters++;

            if (GetMySizeonDisk() + FAT.GetAvailableClusters() >= neededClusters)
                canAddDirectory = true;
            return canAddDirectory;
        }
    }
}
