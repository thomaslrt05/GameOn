using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GameOn.Models.Game;

namespace GameOn.ViewModels
{
    class SudokuRankedPageVM : INotifyPropertyChanged
    {
        SudokuLogic sudoku { get; set; }
        public List<List<SudokuCell>> SudokuGrid { get; set; }


        #region Inotify
        // Obligatoire par l'implémentation de l'interface INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        //Fonction qui gère le lancement de l'événément PropertyChanged
        protected virtual void OnPropertyChanged(string nomPropriete)
        {
            //Vérifie si il y a au moins 1 abonné
            if (this.PropertyChanged != null)
            {
                //Lance l'événement -> tous les abonnés seront notifiés
                this.PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
            }
        }
        #endregion


        public static MainWindowVM Instance = MainWindowVM.Instance;
        public ICommand CellClick { get; }


        public SudokuRankedPageVM()
        {
            this.ChangePageCommand = new RelayCommand(ExecuteBackToMenuButton,CanExecute);
        }

        private ICommand _changePageCommand;

        public ICommand ChangePageCommand
        {
            get { return _changePageCommand; }
            set { _changePageCommand = value; }
        }

       
        private void ExecuteBackToMenuButton(object parameter)
        {
            MainWindowVM mainWindowVM = MainWindowVM.Instance;
            Instance.CurrentPage = new LandingPage();
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

    }

}
