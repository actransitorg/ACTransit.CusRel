using System;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Text;
using Microsoft.SqlServer.Server;

namespace FileUtility
{
    public static class FileUtility
    {
        [Microsoft.SqlServer.Server.SqlProcedure]
        public static int Unzip(string zipFile)
        {
            return UnzipToPath(zipFile, zipFile.Replace(Path.GetExtension(zipFile), String.Empty));
        }

        [Microsoft.SqlServer.Server.SqlProcedure]
        public static int UnzipToPath(string zipFile, string extractDirectory)
        {
            int returnValue = 1;//SQL Stored Proc Error = 1;
            try
            {
                if (Directory.Exists(extractDirectory))
                    Directory.Delete(extractDirectory, true);
                if (File.Exists(zipFile))
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, extractDirectory);
                    returnValue = 0;//SQL Stored Proc Success = 0
                }
            }
            catch (Exception)
            {
            }
            return returnValue;
        }

        [Microsoft.SqlServer.Server.SqlProcedure]
        public static int CreateEmptyFile(string file)
        {
            int returnValue = 1;//SQL Stored Proc Error = 1;
            try
            {
                File.WriteAllText(file, "", Encoding.UTF8);
                returnValue = 0;//SQL Stored Proc Success = 0
            }
            catch (Exception)
            {
                throw;
            }
            return returnValue;
        }

        [Microsoft.SqlServer.Server.SqlProcedure]
        public static int CreateZipFile(string sourceDirectoryName, string destinationArchiveFileName)
        {
            int returnValue = 1;//SQL Stored Proc Error = 1;
            try
            {
                ZipFile.CreateFromDirectory(sourceDirectoryName, destinationArchiveFileName);
                returnValue = 0;//SQL Stored Proc Success = 0
            }
            catch (Exception)
            {
            }
            return returnValue;
        }



        [Microsoft.SqlServer.Server.SqlProcedure]
        public static int CopyFile(string sourceFile, string destinationFile)
        {
            int returnValue = 1;//SQL Stored Proc Error = 1;
            try
            {
                File.Copy(sourceFile, destinationFile, true);//overwrite
                returnValue = 0;//SQL Stored Proc Success = 0
            }
            catch (Exception)
            {
            }
            return returnValue;
        }

        [Microsoft.SqlServer.Server.SqlProcedure]
        public static int RenameFile(string sourceFile, string destinationFile)
        {
            int returnValue = 1;//SQL Stored Proc Error = 1;
            try
            {
                if (File.Exists(destinationFile))
                    File.Delete(destinationFile);
                File.Move(sourceFile, destinationFile);
                returnValue = 0;//SQL Stored Proc Success = 0
            }
            catch (Exception)
            {
            }
            return returnValue;
        }

        [Microsoft.SqlServer.Server.SqlProcedure]
        public static int DeleteFile(string file)
        {
            int returnValue = 1;//SQL Stored Proc Error = 1;
            try
            {
                if (File.Exists(file))
                    File.Delete(file);
                returnValue = 0;//SQL Stored Proc Success = 0
            }
            catch (Exception)
            {
            }
            return returnValue;
        }

        public static int CreateDirectory(string directory)
        {
            int returnValue = 1;//SQL Stored Proc Error = 1;
            try
            {
                if (System.IO.Directory.Exists(directory))
                    System.IO.Directory.Delete(directory);
                System.IO.Directory.CreateDirectory(directory);
                returnValue = 0;//SQL Stored Proc Success = 0
            }
            catch (Exception)
            {
            }
            return returnValue;
        }

        public static int RenameDirectory(string sourceDirectory, string destinationDirectory)
        {
            int returnValue = 1;//SQL Stored Proc Error = 1;
            try
            {
                if (System.IO.Directory.Exists(destinationDirectory))
                    System.IO.Directory.Delete(destinationDirectory);
                System.IO.Directory.Move(sourceDirectory, destinationDirectory);
                returnValue = 0;//SQL Stored Proc Success = 0
            }
            catch (Exception)
            {
            }
            return returnValue;
        }

        public static int DeleteDirectory(string directory)
        {
            int returnValue = 1;//SQL Stored Proc Error = 1;
            try
            {
                if (System.IO.Directory.Exists(directory))
                    System.IO.Directory.Delete(directory);
                returnValue = 0;//SQL Stored Proc Success = 0
            }
            catch (Exception)
            {
            }
            return returnValue;
        }
    }
}
