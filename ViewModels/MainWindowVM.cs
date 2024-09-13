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
 
        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged(string nomPropriete)
        {

            if (this.PropertyChanged != null)
            {
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
            DAL.ConnectionString = "Server=serverName;Port=port;Database=databaseName;Uid=idUser;Pwd=password";
            CurrentPage = new LoginPage();
        }



        private ICommand _changePageCommand;

        public ICommand ChangePageCommand 
        { 
            get { return _changePageCommand; } 
            set { _changePageCommand = value; } 
        }
 
    }
}
