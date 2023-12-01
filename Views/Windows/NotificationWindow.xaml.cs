using GameOn.DataAccesLayer;
using GameOn.Models;
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
using System.Windows.Shapes;

namespace GameOn.Views.Windows
{
    /// <summary>
    /// Logique d'interaction pour NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window, INotifyPropertyChanged
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
        #endregion


        public NotificationWindow()
        {
            InitializeComponent();
            List<Notification> list = GetNotifications();
            this.DataContext = this;
            notificationListBox.ItemsSource = list.Select(n=>n.Content);

            //mettre tous les notif comme vue
            DAL dal = new DAL();
            foreach (Notification n in list)
            {
                dal.NotifFactory.SetNotifAsSeen(n);
            }

        }

        // Méthode pour ajouter une notification à la collection
        private List<Notification> GetNotifications()
        {
            DAL dal = new DAL();
            return dal.NotifFactory.GetUnseenNotificationsOfUser(ConnectionSingleton.UserConnected.Id);
        }
    }
}
