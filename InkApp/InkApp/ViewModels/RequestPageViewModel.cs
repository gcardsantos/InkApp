using InkApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InkApp.ViewModels
{
    public class RequestPageViewModel : ViewModelBase
    {
        public string NameText { get; set; }
        public string UserText { get; set; }
        public string FaceText { get; set; }
        public string NumberText { get; set; }
        public string AboutText { get; set; }

        public DelegateCommand SendCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public RequestPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            SendCommand = new DelegateCommand(SendEmail);
            CancelCommand = new DelegateCommand(CancelEvent);
        }

        private async void CancelEvent()
        {
            await NavigationService.GoBackToRootAsync();
        }

        private async void SendEmail()
        {
            var email = new EmailService();
            var body = NameText + " - " + UserText + " - " + FaceText + " - " + NumberText + " - " + AboutText;
            await email.SendEmail("[InkApp] Solicitação", body, new List<string>() { "y286708@nwytg.net" });
        }
    }
}
