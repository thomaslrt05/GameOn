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

        private bool CanExecute(object parameter)
        {
            return true;
        }

        public ProfilWindowVM()
        {

            UserConnected = ConnectionSingleton.UserConnected;
            this.ModifyButton = new RelayCommand(ExecuteModifyButton, CanExecute);
           
        }

        public ICommand ModifyButton { get; private set; }

        public void ExecuteModifyButton(object parameter)
        {

        }



    }
}
