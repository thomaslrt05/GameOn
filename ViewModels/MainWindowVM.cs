using GameOn.Views.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GameOn.ViewModels
{
    public class MainWindowVM  : INotifyPropertyChanged
    {
        public static MainWindowVM Instance = new MainWindowVM();

        public ICommand ChangePageCommand => new RelayCommand<string>(ChangePage);

        [ObservableProperty]
        private Page currentFrame;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindowVM() 
        {
            CurrentPage = new DashBoardView();
        }

        private void ChangePage(string pageName)
        {
            switch(pageName) 
            {
                case "LoginPage":
                    CurrentPage = new LoginPage();
                    break;
                case "LandingPage":
                    CurrentPage = new LandingPage();
                    break;
            }
        }
    }
}
