using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace virtual_disk
{
    internal static class FAT
    {
       static public int[] FATarray= new int[1024];
        
        public static void PrepareFAT()
        {
            for (int i = 0; i < FATarray.Length; i++)
            {
                if (i == 0 || i == 4)
                    FATarray[i] = -1;
                else if (i == 1 || i == 2 || i == 3)
                    FATarray[i] = i + 1;
                else
                    FATarray[i] = 0;
            }
        }
        public static void WriteFAT()
        {
            byte[] FATarrayInBytes = new byte[4096];
            int indexOfFATarrayInBytes;
            System.Buffer.BlockCopy(FATarray,0, FATarrayInBytes, 0, FATarrayInBytes.Length);
            for (int i = 0; i < 4; i++)
            {
                indexOfFATarrayInBytes = i*1024;
                byte[] clusterBytes = new byte[1024];
                for (int j = 0; j < clusterBytes.Length; j++)
                {
                    clusterBytes[j] = FATarrayInBytes[indexOfFATarrayInBytes];
                    indexOfFATarrayInBytes++;
                }
               // i+1 because cluster 0 (first of five 0 ,1 ,2 ,3 ,4) is filled with zeroes.
               VirtualDisk.WriteCluster(i + 1, clusterBytes);
            }
        }
        public static void printFat()
        {
            for(int i = 0; i < FATarray.Length; i++)
            {
                System.Console.WriteLine($"FAT[{i}] = {FATarray[i]}");
            }
        }
        public static void ReadFAT()
        {
            int indexOfFATarrayInBytes = 0;
            byte[] FATarrayInBytes = new byte[4096];
          
            for (int i = 0; i < 4; i++)
            {
                indexOfFATarrayInBytes = i * 1024;
                byte[] clusterBytes = new byte[1024];
                clusterBytes = VirtualDisk.ReadCluster(i + 1);
                for (int j = 0; j < clusterBytes.Length; j++)
                {
                    FATarrayInBytes[indexOfFATarrayInBytes]=clusterBytes[j];
                    indexOfFATarrayInBytes++;                    
                }
            }
            System.Buffer.BlockCopy(FATarrayInBytes, 0, FATarray, 0, FATarrayInBytes.Length);
        }
        public static void SetClusterPointer(int clusterIndex, int pointer)
        {
            FATarray[clusterIndex] = pointer;
        }
        public static int GetClusterPointer(int clusterIndex)
        {
           return FATarray[clusterIndex] ;
        }
        public static int GetEmptyClusterIndex()
        {
            for(int i = 5; i < 1024; i++)
            {
                if (FATarray[i] == 0)
                {
                    return i ;
                }
            }
            return -1;
        }
        public static int GetAvailableClusters()
        {
            int numberOfAvailableClusters = 0;
            for (int i = 0; i < 1024; i++)
            {
                if (FAT.FATarray[i] == 0)
                    numberOfAvailableClusters++;

            }
            return numberOfAvailableClusters;
        }
    }
}
