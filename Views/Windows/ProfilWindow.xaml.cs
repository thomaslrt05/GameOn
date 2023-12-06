using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GameOn.DataAccesLayer;
using GameOn.ViewModels;
using GameOn.Models;
using System.ComponentModel;

namespace GameOn.Views.Windows
{
   
    public partial class ProfilWindow : Window, INotifyPropertyChanged
    {
        #region Propriété 
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string nomPropriete)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
            }
        }

        private User _userConnected;
        public User UserConnected
        {
            get { return _userConnected; }
            set
            {
                _userConnected = value;
                OnPropertyChanged(nameof(UserConnected)); // Assurez-vous que cette méthode notifie la vue du changement.
            }
        }

        private DAL dal;

        private string _currentPassword;
        public string CurrentPassword
        {
            set
            {
                _currentPassword = value;
                OnPropertyChanged(nameof(CurrentPassword));
            }
        }
        private string _newPassword;
        public string NewPassword
        {
            set
            {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
            }
        }

        private string _newPasswordComfirmed;
        public string NewPasswordComfirmed
        {
            set
            {
                _newPasswordComfirmed = value;
                OnPropertyChanged(nameof(NewPasswordComfirmed));
            }
        }
        #endregion

        public ProfilWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            UserConnected = ConnectionSingleton.UserConnected;
            dal = new DAL();
        }

        public void ExecuteModifyButton(object sender, RoutedEventArgs e)
        {
            _currentPassword = currentPassword.Password;
            _newPassword = newPassword.Password;
            _newPasswordComfirmed = newPasswordComfirmed.Password;

            if (_currentPassword is not null || _newPassword is not null || _newPasswordComfirmed is not null)
                if (UserConnected.Password == User.Hash(_currentPassword))
                {
                    if (_newPassword.Equals(_newPasswordComfirmed))
                    {
                        dal.UserFact.ChangePassword(UserConnected, _newPassword);
                        MessageBox.Show("Mot de passe changé");
                        currentPassword.Password = string.Empty;
                        newPassword.Password = string.Empty;
                        newPasswordComfirmed.Password = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("Les nouveaux mots de passe ne correspondents pas");
                        currentPassword.Password = string.Empty;
                        newPassword.Password = string.Empty;
                        newPasswordComfirmed.Password = string.Empty;
                    }
                }
                else MessageBox.Show("Le mot de passe actuel est éronnée");
            else MessageBox.Show("Un des champs requis est vide");
        }



    }
}
