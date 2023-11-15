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
    internal class LoginPageVM : INotifyPropertyChanged
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


        #region Propriété

        public static MainWindowVM Instance = MainWindowVM.Instance;
        private String _txtPassword;

        public String TxtPassword
        {
            get
            {
                return _txtPassword;
            }
            set
            {
                _txtPassword = value;
                OnPropertyChanged(nameof(TxtPassword));
            }
        }

        private String _txtEmail;

        public String TxtEmail
        {
            get
            {
                return _txtEmail;
            }
            set
            {
                _txtEmail = value;
                OnPropertyChanged(nameof(TxtEmail));
            }
        }

       

       
        #endregion


        public LoginPageVM()
        {
            this.ChangePageCommand = new RelayCommand(OnConnexion, CanConnexion);
            
        }


        private ICommand _changePageCommand;

        public ICommand ChangePageCommand
        {
            get { return _changePageCommand; }
            set { _changePageCommand = value; }
        }


        private void OnConnexion(object parameter)
        {
            
            User user = new DAL().UserFact.GetByMail(_txtEmail);
            if (user != null)
            {
                if (user.Password == User.Hash(_txtPassword))
                {
                    ConnectionSingleton.UserConnected = user;
                    //MainWindowVM mainWindowVM = MainWindowVM.Instance;
                    MainWindowVM.Instance.CurrentPage = new LandingPage();
                }
                else MessageBox.Show("Le mot de passe est eronné");
            }
            else MessageBox.Show("Le login donné n'existe pas");
        }

        private bool CanConnexion(object parameter)
        {
            return true;
        }

        

    }
}
