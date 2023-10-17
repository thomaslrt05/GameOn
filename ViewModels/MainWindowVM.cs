using GameOn.Views.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GameOn.Models;
using GameOn.DataAccesLayer;

namespace GameOn.ViewModels
{
    public class MainWindowVM  : INotifyPropertyChanged
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


        public static MainWindowVM Instance = new MainWindowVM();

        private Page _currentPage;

        public Page CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        protected MainWindowVM() 
        {

            CurrentPage = new LoginPage();
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

        private ICommand _changePageCommand;

        public ICommand ChangePageCommand 
        { 
            get { return _changePageCommand; } 
            set { _changePageCommand = value; } 
        }
 
    }
}
