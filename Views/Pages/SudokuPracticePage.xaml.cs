using GameOn.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameOn.Views.Pages
{
    /// <summary>
    /// Logique d'interaction pour SudokuPracticePage.xaml
    /// </summary>
    public partial class SudokuPracticePage : Page, INotifyPropertyChanged
    {
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

        public SudokuPracticePage()
        {
            InitializeComponent();
            this.ChangePageCommand = new RelayCommand(ExecuteBackToMenuButton, CanExecute);
        }

        private ICommand _changePageCommand;

        public ICommand ChangePageCommand
        {
            get { return _changePageCommand; }
            set { _changePageCommand = value; }
        }


        private void ExecuteBackToMenuButton(object parameter)
        {
            MainWindowVM.Instance.CurrentPage = new SudokuMainPage();
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }


    }
}



    

        







