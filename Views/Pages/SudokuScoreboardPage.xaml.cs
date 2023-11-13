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
using GameOn.DataAccesLayer;
using GameOn.ViewModels;
using GameOn.Models;

namespace GameOn.Views.Pages
{
    /// <summary>
    /// Logique d'interaction pour SudokuScoreboardPage.xaml
    /// </summary>
    public partial class SudokuScoreboardPage : Page
    {
        public SudokuScoreboardPage()
        {
            InitializeComponent();
            LoadScoreboard();
        }

        public void LoadScoreboard()
        {
            try
            {
                DAL dal = new DAL();
                List<User>? usersList = dal.UserFact.GetAllUser();
                List<UserViewModel> dataList = new List<UserViewModel>();
                int points = 0;
                foreach (User user in usersList)
                {
                    points = dal.UserFact.GetAllPointOfUser(user);
                    dataList.Add(new UserViewModel(user, points));
                }
                dataGrid.ItemsSource = dataList;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Une erreur est survenue lors du chargement du scoreboard");
            }
        }

        private void BackToLandingPage(object sender, RoutedEventArgs e)
        {
            MainWindowVM.Instance.CurrentPage = new LandingPage();
        }
    }
}
