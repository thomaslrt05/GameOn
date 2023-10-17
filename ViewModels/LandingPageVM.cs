using GameOn.Views.Pages;
using GameOn.Views.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GameOn.ViewModels
{
    internal class LandingPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string nomPropriete)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
            }
        }

        public static MainWindowVM Instance = MainWindowVM.Instance;

        public LandingPageVM() 
        {
            this.ButtonSudokuGame = new RelayCommand(ExecuteButtonSudokuGame, CanExecute);
            this.ButtonLogOut = new RelayCommand(ExecuteLogOut, CanExecute);
            this.ButtonNotif = new RelayCommand(ExecuteNotif, CanExecute);
            this.ButtonProfil = new RelayCommand(ExecuteButtonProfil, CanExecute);
        }

        

        private bool CanExecute(object parameter)
        {
            return true;
        }

        public ICommand ButtonSudokuGame { get; private set; }

        private void ExecuteButtonSudokuGame(object parameter)
        {
            MainWindowVM mainWindowVM = MainWindowVM.Instance;
            mainWindowVM.CurrentPage = new SodukuMainPage();
        }

        public ICommand ButtonProfil { get; private set; }

        private void ExecuteButtonProfil(object parameter)
        {
            ProfilWindow profilWindow = new ProfilWindow();
            profilWindow.Show();
        }

        public ICommand ButtonNotif { get; private set; }

        private void ExecuteNotif(object parameter)
        {
            
        }

        public ICommand ButtonLogOut { get; private set; }

        private void ExecuteLogOut(object parameter)
        {
            
        }





    }
}
