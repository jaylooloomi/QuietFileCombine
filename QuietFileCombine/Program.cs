using QuietFileCombine.API;
using System;
using System.IO;
using System.Text;

namespace QuietFileCombine
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var api = QuietFileCombineAPI.GetInstance();
            string mainFile = @"C:\Video.flv";
            string subFile1 = @"C:\Action.exe";
            string subFile2 = @"C:\Action2.exe";

            //Combine distinct file
            api.CombineDistinctFile(mainFile, subFile1);

            //Extend file which is already combine
            api.ExtendFile(mainFile, subFile2);
        }

      
    }
}
