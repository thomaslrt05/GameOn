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

        private String txtPassword = string.Empty;

        public static MainWindowVM Instance = MainWindowVM.Instance;


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
            MainWindowVM mainWindowVM = MainWindowVM.Instance;
            mainWindowVM.CurrentPage = new LandingPage();
        }

        private bool CanConnexion(object parameter)
        {
            return true;
        }



    }
}
