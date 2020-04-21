using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SMB2forXamarinForms.Models;
using SMB2forXamarinForms.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SMB2forXamarinForms.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        //public ICommand DownloadCommand { get; }
        //public ICommand UploadCommand { get; }
        public ICommand FileTransferCommand { get; }
        public ICommand SelectLocalDirectoryCommand { get; }
        public ICommand FilePickerShowCommand { get; }
        public ICommand DirectoryPickerShowCommand { get; }
        public ISmbService smbService;
        public SmbInfo MySmbInfo { get; set; }

        private ImageSource _imgRadioChecked;
        public ImageSource ImgRadioChecked
        {
            get { return _imgRadioChecked; }
            set { SetProperty(ref _imgRadioChecked, value); }
        }
        private ImageSource _imgRadioUnchecked;
        public ImageSource ImgRadioUnchecked
        {
            get { return _imgRadioUnchecked; }
            set { SetProperty(ref _imgRadioUnchecked, value); }
        }
        public ICommand SelectRadioCommand { get; }

        private int _transferType;
        public int TransferType
        {
            get { return _transferType; }
            set { SetProperty(ref _transferType, value); }
        }

        private string _transferTypeTxt;
        public string TransferTypeTxt
        {
            get { return _transferTypeTxt; }
            set { SetProperty(ref _transferTypeTxt, value); }
        }

        private List<ImageSource> _imgRadio;
        public List<ImageSource> ImgRadio
        {
            get { return _imgRadio; }
            set
            {
                SetProperty(ref _imgRadio, value);
            }
        }

        public List<string> LocalDirectoryList { get; set; }

        private string _selectedLocalDirectory;
        public string SelectedLocalDirectory
        {
            get { return _selectedLocalDirectory; }
            set 
            { 
                SetProperty(ref _selectedLocalDirectory, value);
                SelectedLocalDirectoryChanged(value);
            }
        }

        private ObservableCollection<SmbInfo> _fileList;
        public ObservableCollection<SmbInfo> FileList
        {
            get { return _fileList; }
            set
            {
                SetProperty(ref _fileList, value);
                RaisePropertyChanged("FileList");
            }
        }

        private SmbInfo _selectedFile;
        public SmbInfo SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                SetProperty(ref _selectedFile, value);
                SelectedFileChanged(value);
            }
        }

        public MainPageViewModel(INavigationService navigationService, IStorageService storageService, IPopupService popupService)
            : base(navigationService, storageService, popupService)
        {
            Title = "File Transfer via SMB2.0";

            //set default value
            MySmbInfo = new SmbInfo()
            {
                ServerDirectory = @"192.168.11.101/share",
                User = "user",
                Password = "password",
            };

            smbService = new SmbService();

            FileTransferCommand = new DelegateCommand(async () => await SmbFileTransfer());

            ImgRadioChecked = ImageSource.FromResource("SMB2forXamarinForms.Images.round_radio_button_checked_black.png");
            ImgRadioUnchecked = ImageSource.FromResource("SMB2forXamarinForms.Images.round_radio_button_unchecked_black.png");

            ImgRadio = new List<ImageSource>();

            for (var i = 0; i < 2; i++)
            {
                ImgRadio.Add(ImgRadioUnchecked);
            }
            SelectRadio(0);
            //RaisePropertyChanged("ImgRadio");

            SelectRadioCommand = new DelegateCommand<string>(x => SelectRadio(Convert.ToInt32(x)));

            LocalDirectoryList = new List<string>()
            {
                "Downloads","Dcim","Pictures","Movies","Documents","Music",
            };
            SelectedLocalDirectory = LocalDirectoryList[0];

            FilePickerShowCommand = new DelegateCommand<Picker>((Picker picker) => ShowFilePicker(picker));
            DirectoryPickerShowCommand = new DelegateCommand<Picker>((Picker picker) => picker.Focus());

        }

        public async Task SmbFileTransfer()
        {
            var msg = "";
            try
            {
                if (string.IsNullOrEmpty(MySmbInfo.ServerDirectory))
                {
                    msg += "The folder is not specifed." + Environment.NewLine;
                }
                if (string.IsNullOrEmpty(MySmbInfo.LocalDirectory)) 
                {
                    msg += "Local directory is not selected." + Environment.NewLine;
                }
                if (string.IsNullOrEmpty(MySmbInfo.FileName))
                {
                    msg += "The file is not selected." + Environment.NewLine;
                }

                if (!string.IsNullOrEmpty(msg))
                {
                    msg = msg.TrimEnd(Environment.NewLine.ToCharArray());
                    PopupService.ShowToast(msg, false);
                    return;
                }
                else
                {
                    if (TransferType == 0)
                    {
                        var res = await smbService.Smb2Download(MySmbInfo).ConfigureAwait(false);
                        if (res == 1)
                        {
                            msg = "Succeeded to download" + Environment.NewLine + Path.Combine(MySmbInfo.LocalDirectory, MySmbInfo.FileName);
                        }
                        else
                        {
                            msg = "Failed to download the file!";
                        }
                    }
                    else
                    {
                        var res = await smbService.Smb2Upload(MySmbInfo).ConfigureAwait(false);
                        if (res == 1)
                        {
                            msg = "Succeeded to upload" + Environment.NewLine + Path.Combine(MySmbInfo.ServerDirectory, MySmbInfo.FileName);
                        }
                        else
                        {
                            msg = "Failed to upload the file!";
                        }
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        PopupService.ShowToast(msg, true);
                    });

                }

            }
            catch (Exception ex)
            {
                msg = "Error:" + ex.Message;
                Console.WriteLine(ex.Message);
            }

        }


        public void SelectRadio(int idx)
        {
            for (var i = 0; i < 2; i++)
            {
                if (i == idx)
                {
                    ImgRadio[i] = ImgRadioChecked;
                }
                else
                {
                    ImgRadio[i] = ImgRadioUnchecked;
                }
            }
            RaisePropertyChanged("ImgRadio");
            TransferType = idx;
            TransferTypeTxt = idx == 0 ? "Download" : "Upload";
            MySmbInfo.FileName = null;
        }

        public void SelectedLocalDirectoryChanged(string value)
        {
            MySmbInfo.LocalDirectory = StorageService.GetPublicDirectoryPath(value);
            MySmbInfo.FileName = null;
        }

        public void SelectedFileChanged(SmbInfo smb)
        {
            if (smb != null)
            {
                MySmbInfo.FileName = smb.FileName;
                if (TransferType == 1)
                {
                    MySmbInfo.LocalDirectory = smb.LocalDirectory;
                }
            }
        }

        public void ShowFilePicker(Picker picker)
        {
            try
            {
                SelectedFile = null;// new SmbInfo();

                var fileList = new List<SmbInfo>();

                if (TransferType == 1 && !string.IsNullOrEmpty(MySmbInfo.LocalDirectory))
                {
                    fileList = StorageService.GetFileList(MySmbInfo.LocalDirectory);
                }
                else if (TransferType == 0 && !string.IsNullOrEmpty(MySmbInfo.ServerDirectory))
                {
                    fileList = smbService.Smb2GetFileList(MySmbInfo);
                }

                FileList = new ObservableCollection<SmbInfo>(fileList);
                RaisePropertyChanged("FileList");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            picker.Focus();
        }

    }
}
