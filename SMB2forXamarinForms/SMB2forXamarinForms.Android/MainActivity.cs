using Android.App;
using Android.Content.PM;
using Android.OS;
using Prism;
using Prism.Ioc;
using SMB2forXamarinForms.Droid.Services;
using SMB2forXamarinForms.Services;

namespace SMB2forXamarinForms.Droid
{
    [Activity(Label = "SMB2forXamarinForms", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App(new AndroidInitializer()));

            RequestPermission();
        }

        public const int REQUEST_CODE_PERMISSIONS = 1001;

        public void RequestPermission()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                string[] permissions = new string[]
                {
                    Android.Manifest.Permission.WriteExternalStorage,
                    Android.Manifest.Permission.ReadExternalStorage,
                };

                foreach (string permission in permissions)
                {
                    if (CheckSelfPermission(permission) != Permission.Granted)
                    {
                        RequestPermissions(permissions, REQUEST_CODE_PERMISSIONS);
                        break;
                    }
                }
            }
        }
    }



    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
            containerRegistry.RegisterInstance<IStorageService>(new StorageService());
            containerRegistry.RegisterInstance<IPopupService>(new PopupService());

        }
    }
}

