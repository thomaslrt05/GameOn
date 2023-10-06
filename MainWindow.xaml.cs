using GameOn.ViewModels;
using GameOn.DataAccesLayer;
using GameOn.Models;
using System;
using System.Collections.Generic;
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

namespace GameOn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Crée la connexion a la BD
            /* Serveur KC
            DAL.ConnectionString = "Server=sql.decinfo-cchic.ca;Port=33306;Database=a23_e80_hourglass_gameon;Uid=dev-2334653;Pwd=Sayer123";
            */

            // Utiliser les VM en implementant INotifyPropertyChanged  
            this.DataContext = MainWindowVM.Instance;
        }

    }
}
