using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuietFileCombine
{
    public class QuietFileCombineAPI
    {
        const string FlagStart = "[COMBINE_START]";
        const string FlagEnd = "[COMBINE_END]";
        private QuietFileCombineAPI() { }
        private static QuietFileCombineAPI _Instance { get; set; } = null;
        public static QuietFileCombineAPI GetInstance()
        {
            _Instance = _Instance ?? new QuietFileCombineAPI();
            return _Instance;
        }

        /// <summary>
        /// Combine addition file
        /// </summary>
        /// <param name="MainFileFullPath">Main file</param>
        /// <param name="AdditionFileFullPath">Additional file</param>
        public void CombineDistinctFile(string MainFileFullPath, string AdditionFileFullPath)
        {
            var temp = GetTempFileName(MainFileFullPath);

            CombineMultipleFilesIntoSingleFile(MainFileFullPath, AdditionFileFullPath, temp);


            if (!System.IO.File.Exists(temp)) return;
            if (!System.IO.File.Exists(MainFileFullPath)) return;

            //刪除原檔
            System.IO.File.Delete(MainFileFullPath);

            //以合併後的檔案取代原檔
            System.IO.File.Move(temp, MainFileFullPath);
        }

        /// <summary>
        /// Check the file is allowed to extend 
        /// </summary>
        /// <param name="MainFileFullPath"></param>
        /// <returns></returns>
        public bool IsAllowExtended(string MainFileFullPath)
        {
            string str;
            if (!ReadTxtFile(MainFileFullPath, out str)) return false;
            if (str.Contains(FlagStart) && str.Contains(FlagEnd)) return true;       
            return false;
        }

        /// <summary>
        /// Extended file
        /// </summary>
        /// <param name="MainFileFullPath"></param>
        /// <param name="ExtendedFileFullPath"></param>
        /// <returns></returns>
        public void ExtendFile(string MainFileFullPath, string ExtendedFileFullPath)
        {
            //The File Allow Split
            string str;
            if (!IsCanSplit(MainFileFullPath,out str))
            {
                string msg = string.Format(@"This file Can't Spit! {0}", MainFileFullPath);
                throw new Exception(msg);
            }

            //Get base64Str information
            string base64Str = GetBetweenLast(str,FlagStart, FlagEnd);

            //
            if (System.IO.File.Exists(ExtendedFileFullPath)) System.IO.File.Delete(ExtendedFileFullPath);
            if (!Base64StringToFile(base64Str, ExtendedFileFullPath))
            {
                string msg = string.Format(@"Create Child File Error! {0}", MainFileFullPath);
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Hide additional file
        /// </summary>
        /// <param name="MainFileFullPath">Main file</param>
        /// <param name="AdditionFileFullPath">Additional file</param>
        /// <param name="FinalFileFullPath">Out put Main file full path</param>
        private void CombineMultipleFilesIntoSingleFile(string MainFileFullPath, string AdditionFileFullPath, string FinalFileFullPath)
        {
            if (MainFileFullPath != FinalFileFullPath
                && System.IO.File.Exists(FinalFileFullPath)) System.IO.File.Delete(FinalFileFullPath);

            AdditionFileFullPath = GenerateTempBase64File(AdditionFileFullPath);
            string[] inputFilePaths = new string[] { MainFileFullPath, AdditionFileFullPath };

            using (var outputStream = File.Create(FinalFileFullPath))
            {
                foreach (var inputFilePath in inputFilePaths)
                {
                    using (var inputStream = File.OpenRead(inputFilePath))
                    {
                        inputStream.CopyTo(outputStream);
                    }
                }
            }

            if (System.IO.File.Exists(AdditionFileFullPath)) System.IO.File.Delete(AdditionFileFullPath);
        }

        private bool IsCanSplit(string MainFileFullPath, out string source)
        {
            source = "";
            string str;
            if (!ReadTxtFile(MainFileFullPath, out str)) return false;
            if (str.Contains(FlagStart) && str.Contains(FlagEnd))
            {
                source = str;
                return true;
            }
            return false;
        }
        private string GetBetweenLast(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                End = strSource.LastIndexOf(strEnd, strSource.Length);
                Start = strSource.LastIndexOf(strStart, strSource.Length) + strStart.Length;

                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
        private string GetBetweenFirst(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
        private string GetTempFileName(string fileFullPath)
        {
            string dir = System.IO.Path.GetDirectoryName(fileFullPath);
            string extension = System.IO.Path.GetExtension(fileFullPath);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(fileFullPath);
            string temp = string.Format(@"{0}\{1}-{2}{3}", dir, fileName, "TempFile", extension);

            return temp;
        }   
        private bool ReadTxtFile(string filePath, out string jsonTxt)
        {
            jsonTxt = "";
            if (!System.IO.File.Exists(filePath)) return false;

            using (System.IO.StreamReader file = new System.IO.StreamReader(filePath, true))
            {
                jsonTxt = jsonTxt + file.ReadToEnd();
                file.Dispose();
            }
            return true;
        }
        private string GetFlagMsg(string base64Str)
        {
            return string.Format(@"{0}{1}{2}", FlagStart, base64Str, FlagEnd);
        }
        private void CreateFile(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
            {
                using (FileStream fs = System.IO.File.Create(fileName))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes("");
                    fs.Write(info, 0, info.Length);
                }
            }
        }
        private string GenerateTempBase64File(string filePath)
        {
            string temp = GetTempFileName(filePath);

            string base64Str = FileToBase64String(filePath);
            string msg = GetFlagMsg(base64Str);

            CreateFile(temp);

            if (!WriteTxtFile(temp, msg)) return temp;
            return temp;
        }
        private bool WriteTxtFile(string filePath, string fileText)
        {
            if (!System.IO.File.Exists(filePath)) return false;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, false))
            {
                file.WriteLine(fileText);
                file.Dispose();
            }
            return true;
        }
        private byte[] FileToByteArry(string FilePath)
        {
            FileStream inFile = new System.IO.FileStream(FilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            byte[] binaryData = new Byte[inFile.Length];
            long bytesRead = inFile.Read(binaryData, 0, (int)inFile.Length);
            inFile.Close();
            return binaryData;
        }
        private string ByteArryToBase64Str(byte[] byteArry)
        {
            string base64String = System.Convert.ToBase64String(byteArry, 0, byteArry.Length);

            return base64String;
        }
        private string FileToBase64String(string FilePath)
        {
            byte[] binaryData = FileToByteArry(FilePath);
            string base64String = ByteArryToBase64Str(binaryData);

            return base64String;
        }
        private bool Base64StringToFile(string base64String, string FilePath)
        {
            try
            {
                File.WriteAllBytes(FilePath, Convert.FromBase64String(base64String));
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
