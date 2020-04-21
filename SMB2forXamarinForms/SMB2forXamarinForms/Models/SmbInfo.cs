using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMB2forXamarinForms.Models
{
    public class SmbInfo : BindableBase
    {
        private string _serverDirectory;
        public string ServerDirectory
        {
            get { return _serverDirectory; }
            set { SetProperty(ref _serverDirectory, value); }
        }

        private string _user;
        public string User
        {
            get { return _user; }
            set { SetProperty(ref _user, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { SetProperty(ref _fileName, value); }
        }

        private string _localDirectory;
        public string LocalDirectory
        {
            get { return _localDirectory; }
            set { SetProperty(ref _localDirectory, value); }
        }
    }

}
