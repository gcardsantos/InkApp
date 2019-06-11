using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InkApp.ViewModels
{
    public class TabPageViewModel : ViewModelBase
    {

        public TabPageViewModel(INavigationService navigationService) : base(navigationService)
        {

        }

    }
}
