using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SMB2forXamarinForms.Models;
using SMB2forXamarinForms.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMB2forXamarinForms.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }
        protected IStorageService StorageService { get; private set; }
        protected IPopupService PopupService { get; private set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService, IStorageService storageService, IPopupService popupService)
        {
            NavigationService = navigationService;
            StorageService = storageService;
            PopupService = popupService;
        }

        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }
    }
}
