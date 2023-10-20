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
        Sudoku sudoku { get; set; }
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

        }

        
        private string enteredNumber;
        public string EnteredNumber
        {
            get { return enteredNumber; }
            set
            {
                enteredNumber = value;
                OnPropertyChanged(nameof(EnteredNumber));
            }
        }
        private void CellMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Focus();
            }
        }
        public bool IsNumberValid(int row, int col, int number)
        {
            return true;
        }

        private ICommand _changePageCommand;

        public ICommand ChangePageCommand
        {
            get { return _changePageCommand; }
            set { _changePageCommand = value; }
        }

        private void ExecuteCellClick(object parameter)
        {
            if (parameter is string cellNumber)
            {
                // Faites ce que vous devez faire en fonction du numéro de cellule, par exemple :
                MessageBox.Show("Cellule " + cellNumber + " cliquée");
            }
        }

        private bool CanExecuteCellClick(object parameter)
        {
            return true; // Vous pouvez ajouter une logique de validation ici si nécessaire
        }

        /*
        private void CellMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ContentControl contentControl)
            {
                // Vous pouvez accéder à des informations spécifiques à la cellule à partir du DataContext du ContentControl.
                // Par exemple, pour obtenir le numéro de cellule, vous pouvez utiliser le chemin du Binding.
                string cellNumber = contentControl.GetBindingExpression(ContentControl.ContentProperty)?.ParentBinding.Path.Path;

                // Faites ce que vous devez faire en fonction du numéro de cellule, par exemple :
                MessageBox.Show("Cellule " + cellNumber + " cliquée");

            }
        }
        */
        private void ProcessCellClick(Int32 col, Int32 row)
        {
            return;
        }


    }

}
