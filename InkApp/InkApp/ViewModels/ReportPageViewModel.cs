using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InkApp.ViewModels
{
    public class ReportPageViewModel : ViewModelBase
    {
        private readonly IPageDialogService PageDialogService;

        public string _reportText;
        public string ReportText { get { return _reportText; } set { SetProperty(ref _reportText, value); } }

        public Pessoa _pessoa;
        public Pessoa Pessoa { get { return _pessoa; } set { SetProperty(ref _pessoa, value); } }

        public DelegateCommand SendCommand { get; }

        public ReportPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            SendCommand = new DelegateCommand(SendReport);
            PageDialogService = pageDialogService;
        }

        private void SendReport()
        {
            
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            _pessoa = parameters["Pessoa"] as Pessoa;
        }
    }
}
