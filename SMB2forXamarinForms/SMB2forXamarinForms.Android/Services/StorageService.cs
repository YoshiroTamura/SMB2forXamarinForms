using System;
using System.Collections.Generic;
using System.Linq;
using Java.IO;
using SMB2forXamarinForms.Models;
using SMB2forXamarinForms.Services;

namespace SMB2forXamarinForms.Droid.Services
{
    public class StorageService : IStorageService
    {
        public string GetPublicDirectoryPath(string directoryType)
        {
            var publicDirPath = "";
            var strLocation = "";

            try
            {
                switch (directoryType)
                {
                    case "Dcim":
                        strLocation = Android.OS.Environment.DirectoryDcim;
                        break;
                    case "Pictures":
                        strLocation = Android.OS.Environment.DirectoryPictures;
                        break;
                    case "Movies":
                        strLocation = Android.OS.Environment.DirectoryMovies;
                        break;
                    case "Documents":
                        strLocation = Android.OS.Environment.DirectoryDocuments;
                        break;
                    case "Music":
                        strLocation = Android.OS.Environment.DirectoryMusic;
                        break;
                    default:
                        strLocation = Android.OS.Environment.DirectoryDownloads;
                        break;
                }
                publicDirPath = Android.OS.Environment.GetExternalStoragePublicDirectory(strLocation).AbsolutePath;

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            return publicDirPath;
        }

        public List<SmbInfo> GetFileList(string path)
        {
            var fileList = new List<SmbInfo>();
            try
            {
                var files = new File(path).ListFiles();

                if (files != null && files.Length > 0)
                {
                    foreach (var file in files)
                    {
                        if (file != null && !string.IsNullOrEmpty(file.Path)) 
                        {
                            if (file.IsFile)
                            {
                                fileList.Add(new SmbInfo {
                                    FileName = file.Name,
                                    LocalDirectory = file.Path.Substring(0, file.Path.Length - file.Name.Length).TrimEnd('/'),

                                });
                            }
                            else if (file.IsDirectory)
                            {
                                fileList.AddRange(GetFileList(path + "/" + file.Name));
                            }

                        }



                    }
                }

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            return fileList;
        }

        public int CreateText(string path, string txt)
        {
            var res = 0;
            try
            {
                using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Append ))
                {
                    using (var sw = new System.IO.StreamWriter(fs))
                    {
                        sw.WriteLine(txt);
                    }
                }
                res = 1;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            return res;

        }



    }


}