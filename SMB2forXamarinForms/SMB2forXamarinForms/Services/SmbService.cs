using SMB2forXamarinForms.Models;
using SMBLibrary;
using SMBLibrary.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SMB2forXamarinForms.Services
{
    public class SmbService : ISmbService
    {
        public async Task<int> Smb2Download(SmbInfo smb)
        {
            var res = 0;
            var filePathServer = Path.Combine(smb.ServerDirectory, smb.FileName);
            var filePathLocal = Path.Combine(smb.LocalDirectory, smb.FileName);

            try
            {
                var client = new SMBLibrary.Client.SMB2Client();

                var pathArr = filePathServer.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

                if (pathArr.Length > 2 && client.Connect(System.Net.IPAddress.Parse(pathArr[0]), SMBTransportType.DirectTCPTransport))
                {
                    var status = client.Login(String.Empty, smb.User ?? "", smb.Password ?? "");
                    if (status == NTStatus.STATUS_SUCCESS)
                    {
                        var share = client.TreeConnect(pathArr[1], out status);
                        if (status == NTStatus.STATUS_SUCCESS)
                        {
                            var path = string.Join(@"\", pathArr, 2, pathArr.Length - 2);
                            object handle;
                            FileStatus fileStatus;

                            status = share.CreateFile(out handle, out fileStatus, path, AccessMask.GENERIC_READ, 0, ShareAccess.Read, CreateDisposition.FILE_OPEN, CreateOptions.FILE_NON_DIRECTORY_FILE, null);
                            if (status == NTStatus.STATUS_SUCCESS)
                            {
                                FileInformation fileInfo;
                                status = share.GetFileInformation(out fileInfo, handle, FileInformationClass.FileStandardInformation);
                                if (status == NTStatus.STATUS_SUCCESS)
                                {
                                    var len = ((FileStandardInformation)fileInfo).EndOfFile;
                                    var offset = 0;

                                    using (var fs = new FileStream(filePathLocal, FileMode.OpenOrCreate, FileAccess.Write))
                                    {
                                        var bufferSize = (int)client.MaxReadSize;
                                        byte[] buffer;

                                        do
                                        {
                                            int cnt = (int)Math.Min(bufferSize, len - offset);
                                            status = share.ReadFile(out buffer, handle, offset, cnt);
                                            if (status == NTStatus.STATUS_END_OF_FILE || buffer?.Length == 0)
                                            {
                                                break;
                                            }
                                            else if (status != NTStatus.STATUS_SUCCESS)
                                            {
                                                throw new Exception(status.ToString());
                                            }
                                            await fs.WriteAsync(buffer, 0, buffer.Length);
                                            offset += buffer.Length;
                                        }
                                        while (offset < len);
                                    }
                                    res = 1;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return res;
        }

        public async Task<int> Smb2Upload(SmbInfo smb)
        {
            var res = 0;
            var filePathServer = Path.Combine(smb.ServerDirectory, smb.FileName);
            var filePathLocal = Path.Combine(smb.LocalDirectory, smb.FileName);

            try
            {
                var client = new SMBLibrary.Client.SMB2Client();

                var pathArr = filePathServer.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

                if (pathArr.Length > 2 && client.Connect(System.Net.IPAddress.Parse(pathArr[0]), SMBTransportType.DirectTCPTransport))
                {
                    var status = client.Login(String.Empty, smb.User ?? "", smb.Password ?? "");
                    if (status == NTStatus.STATUS_SUCCESS)
                    {
                        var share = client.TreeConnect(pathArr[1], out status);
                        if (status == NTStatus.STATUS_SUCCESS)
                        {
                            var path = string.Join(@"\", pathArr, 2, pathArr.Length - 2);
                            object handle;
                            FileStatus fileStatus;
                            status = share.CreateFile(out handle, out fileStatus, path, AccessMask.GENERIC_READ | AccessMask.GENERIC_WRITE, 0, ShareAccess.None, CreateDisposition.FILE_OPEN_IF, CreateOptions.FILE_NON_DIRECTORY_FILE, null);
                            if (status == NTStatus.STATUS_SUCCESS)
                            {
                                using (var fs = new FileStream(filePathLocal, FileMode.Open, FileAccess.Read))
                                {
                                    var offset = 0;
                                    var bufferSize = (int)client.MaxReadSize;

                                    do
                                    {
                                        var cnt = (int)Math.Min(bufferSize, fs.Length - offset);
                                        var buffer = new byte[cnt];

                                        if (cnt <= 0)
                                        {
                                            break;
                                        }

                                        var readSize = await fs.ReadAsync(buffer, 0, cnt).ConfigureAwait(false);

                                        int numberOfBytesWritten;
                                        status = share.WriteFile(out numberOfBytesWritten, handle, offset, buffer);

                                        if (status != NTStatus.STATUS_SUCCESS)
                                        {
                                            throw new Exception(status.ToString());
                                        }

                                        offset += buffer.Length;
                                    }
                                    while (offset < fs.Length);
                                }

                                share.CloseFile(handle);
                                res = 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return res;
        }

        public List<SmbInfo> Smb2GetFileList(SmbInfo smb)
        {
            var fileList = new List<SmbInfo>();
            try
            {
                var client = new SMBLibrary.Client.SMB2Client();
                var pathArr = smb.ServerDirectory.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

                if (pathArr.Length > 1 && client.Connect(System.Net.IPAddress.Parse(pathArr[0]), SMBTransportType.DirectTCPTransport))
                {
                    var status = client.Login(String.Empty, smb.User ?? "", smb.Password ?? "");
                    if (status == NTStatus.STATUS_SUCCESS)
                    {
                        object handle;
                        SMBLibrary.FileStatus fileStatus;

                        var share = client.TreeConnect(pathArr[1], out status) as SMB2FileStore;
                        if (status == NTStatus.STATUS_SUCCESS)
                        {
                            var subDir = pathArr.Length > 2 ? string.Join(@"\", pathArr, 2, pathArr.Length - 2) : "";
                            status = share.CreateFile(out handle, out fileStatus, subDir, AccessMask.GENERIC_READ, 0, ShareAccess.Read, CreateDisposition.FILE_OPEN, CreateOptions.FILE_DIRECTORY_FILE, null);
                            if (status == NTStatus.STATUS_SUCCESS)
                            {
                                status = share.QueryDirectory(out List<QueryDirectoryFileInformation> files, handle, "*", FileInformationClass.FileDirectoryInformation);
                                foreach (var file in files)
                                {
                                    var fileinfo = (FileDirectoryInformation)file;
                                    if (fileinfo.FileAttributes != SMBLibrary.FileAttributes.Directory)
                                    {
                                        fileList.Add(new SmbInfo {
                                            ServerDirectory = smb.ServerDirectory,
                                            //LocalDirectory = smb.LocalDirectory,
                                            FileName = fileinfo.FileName,
                                        });
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("CreateFile Error:" + status.ToString());
                            }
                        }
                        else
                        {
                            Console.WriteLine("Tree Connect Error:" + status.ToString());
                        }
                    }
                    else
                    {
                        Console.WriteLine("Login Error:" + status.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return fileList;
        }

    }
}
