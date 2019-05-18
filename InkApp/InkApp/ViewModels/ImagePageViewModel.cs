using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InkApp.ViewModels
{
    public class ImagePageViewModel : ViewModelBase
    {

        public ImagePageViewModel(INavigationService navigationService) :base(navigationService)
        {
            
        }
    }
}
