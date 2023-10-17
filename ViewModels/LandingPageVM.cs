using GameOn.Views.Pages;
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
            this.ButtonSudokuGame = new RelayCommand(ExecuteButtonSudokuGame, CanSudokuGame);
        }

        public ICommand ButtonSudokuGame { get; private set; }

        private bool CanSudokuGame(object parameter)
        {
            return true;
        }

        private void ExecuteButtonSudokuGame(object parameter)
        {
            MainWindowVM mainWindowVM = MainWindowVM.Instance;
            mainWindowVM.CurrentPage = new SodukuMainPage();
        }
    }
}
