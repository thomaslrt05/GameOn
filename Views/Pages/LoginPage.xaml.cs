using GameOn.DataAccesLayer;
using GameOn.Models;
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
    /// Logique d'interaction pour LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page, INotifyPropertyChanged
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

        public LoginPage()
        {
            InitializeComponent();
            this.DataContext = this;
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
            _txtPassword = password.Password;
            User user = new DAL().UserFact.GetByMail(_txtEmail);
            if (user != null)
            {
                if (user.Password == User.Hash(_txtPassword))
                {
                    ConnectionSingleton.UserConnected = user;
                    MainWindowVM.Instance.CurrentPage = new LandingPage();
                }
                else MessageBox.Show("Le mot de passe est eronné"); password.Password = string.Empty;

            }
            else MessageBox.Show("Le login donné n'existe pas"); password.Password = string.Empty;
        }

        private bool CanConnexion(object parameter)
        {
            return true;
        }


    }
}
