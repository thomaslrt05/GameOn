﻿using GameOn.Views.Pages;
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

        private String txtPassword = string.Empty;

        public static MainWindowVM Instance = new MainWindowVM();

        private Page _currentPage;

        public Page CurrentPage
        {
            get
            {
                // Retourne la valeur de l'attribut privé
                return _currentPage;
            }
            set
            {
                // Modifie la valeur de l'attribut privé et notifie la view
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


        private void OnConnexion(object parameter) 
        {
            /*
            string pwd = TxtPassword;
            string mail = Email;
            string hash = User.Hash(pwd);
            User user = new DAL().UserFact.GetByMail(mail);
            if (user != null)
            {
                MessageBox.Show(user.Password == hash ? "logged in" : "error");
            }
            MessageBox.Show("error");
            */
            CurrentPage = new LandingPage();
        }

        private bool CanConnexion(object parameter)
        {
            return true;
        }



  
    }
}
