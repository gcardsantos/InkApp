﻿using InkApp.Models;
using InkApp.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InkApp.ViewModels
{
    public class RequestPageViewModel : ViewModelBase
    {
        public string _nameText;
        public string NameText { get { return _nameText; } set { SetProperty(ref _nameText, value); } }

        public string _email;
        public string EmailText { get { return _email; } set { SetProperty(ref _email, value); } }

        private readonly IPageDialogService PageDialogService;

        public DelegateCommand SendCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public RequestPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            SendCommand = new DelegateCommand(SendEmail);
            CancelCommand = new DelegateCommand(CancelEvent);
            PageDialogService = pageDialogService;
        }

        private async void CancelEvent()
        {
            await NavigationService.GoBackToRootAsync();
        }

        private async void SendEmail()
        {
            Solicitacao s = new Solicitacao();
            s.Email = EmailText;
            s.Nome = NameText;

            if (App.Repository.Request(s))
            {
                await PageDialogService.DisplayAlertAsync("Requisição", "Requisição efetuada com sucesso.", "Ok");
            }
            else
            {
                await PageDialogService.DisplayAlertAsync("Requisição", "Ocorreu algum problema ao efetuar requisição.", "Ok");
            }

            await NavigationService.GoBackToRootAsync();
        }
    }
}
