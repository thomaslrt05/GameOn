using GameOn.DataAccesLayer;
using GameOn.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input; 


namespace GameOn.ViewModels
{
    internal class ProfilWindowVM : INotifyPropertyChanged
    {

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

        private bool CanExecute(object parameter)
        {
            return true;
        }

        public ProfilWindowVM()
        {

            UserConnected = ConnectionSingleton.UserConnected;
            dal = new DAL();
            this.ModifyButton = new RelayCommand(ExecuteModifyButton, CanExecute);
           
        }

        public ICommand ModifyButton { get; private set; }

        public void ExecuteModifyButton(object parameter)
        {
            if (_currentPassword is not null || _newPassword is not null)
                if (UserConnected.Password == User.Hash(_currentPassword))
                {
                    dal.UserFact.ChangePassword(UserConnected, _newPassword);
                    MessageBox.Show("Mot de passe changé");
                    CurrentPassword = string.Empty;
                    NewPassword = string.Empty;
                }
                else MessageBox.Show("Le mot de passe actuel est éronnée");
            else MessageBox.Show("Un des champs requis est vide");
        }

   
    }
}
