using SMB2forXamarinForms.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMB2forXamarinForms.Services
{
    public interface IPopupService
    {
        void ShowToast(string msg, bool longFlg = false);
    }
}
