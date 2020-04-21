using SMB2forXamarinForms.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SMB2forXamarinForms.Services
{
    public interface ISmbService
    {
        Task<int> Smb2Download(SmbInfo smb);
        Task<int> Smb2Upload(SmbInfo smb);
        List<SmbInfo> Smb2GetFileList(SmbInfo smb);
    }
}
