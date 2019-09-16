using InkApp.Models;
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

        public string _name;
        public string Name { get { return _name; } set { SetProperty(ref _name, value); } }

        public string _tatuador;
        public string Tatuador { get { return _tatuador; } set { SetProperty(ref _tatuador, value); } }

        public string _cpf;
        public string Cpf { get { return _cpf; } set { SetProperty(ref _cpf, value); } }

        public string _titulo;
        public string Titulo { get { return _titulo; } set { SetProperty(ref _titulo, value); } }

        public Pessoa _pessoa;
        public Pessoa Pessoa { get { return _pessoa; } set { SetProperty(ref _pessoa, value); } }

        public DelegateCommand SendCommand { get; }

        public ReportPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService)
        {
            SendCommand = new DelegateCommand(SendReportAsync);
            PageDialogService = pageDialogService;
        }

        private async void SendReportAsync()
        {
            if(!String.IsNullOrWhiteSpace(Cpf) && !String.IsNullOrWhiteSpace(Name) && !String.IsNullOrWhiteSpace(Tatuador) && !String.IsNullOrWhiteSpace(Titulo) && !String.IsNullOrWhiteSpace(ReportText))
            {
                Report r = new Report()
                {
                    Cpf = this.Cpf,
                    Name = this.Name,
                    Tatuador = this.Tatuador,
                    Titulo = this.Titulo,
                    Motivo = this.ReportText
                };

                if (new Repository().AddReport(r))
                {
                    await PageDialogService.DisplayAlertAsync("Report", "Report efetuado com sucesso.", "Ok");
                }
                else
                {
                    await PageDialogService.DisplayAlertAsync("Report", "Ocorreu algum problema ao efetuar o report.", "Ok");
                }
            }
            else
            {
                await PageDialogService.DisplayAlertAsync("Report", "Há campos vazios", "Ok");
            }


        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            _pessoa = parameters["Pessoa"] as Pessoa;
            Tatuador = _pessoa.Name;
        }
    }
}
