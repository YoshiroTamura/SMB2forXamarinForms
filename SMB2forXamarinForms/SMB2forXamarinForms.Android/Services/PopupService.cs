using System;
using System.Collections.Generic;
using System.Linq;
using Android.Support.Design.Widget;
using Android.Widget;
using Java.IO;
using SMB2forXamarinForms.Models;
using SMB2forXamarinForms.Services;
using Xamarin.Forms;

namespace SMB2forXamarinForms.Droid.Services
{
    public class PopupService : IPopupService
    {
        public void ShowToast(string msg, bool longFlg = false)
        {
            Toast.MakeText(Android.App.Application.Context, msg, longFlg ? ToastLength.Long : ToastLength.Short).Show();
        }

    }


}