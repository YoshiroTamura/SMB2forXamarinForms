using SMB2forXamarinForms.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMB2forXamarinForms.Services
{
    public interface IStorageService
    {
        string GetPublicDirectoryPath(string directoryType);
        List<SmbInfo> GetFileList(string path);
        int CreateText(string path, string txt);
    }
}
