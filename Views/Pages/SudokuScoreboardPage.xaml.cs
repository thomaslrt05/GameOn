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
using Org.BouncyCastle.Asn1.BC;

namespace GameOn.Views.Pages
{
    /// <summary>
    /// Logique d'interaction pour SudokuScoreboardPage.xaml
    /// </summary>
    public partial class SudokuScoreboardPage : Page
    {
        public bool IsGeneral { get; set; } = true;
        public SudokuScoreboardPage()
        {
            InitializeComponent();
            LoadScoreboard();
            InitComboBoxDepartement();
        }

        public void InitComboBoxDepartement()
        {
            try
            {
                DAL dal = new DAL();
                List<Departement>? departements = dal.DepartementFact.GetAll();
                ComboBoxDepartement.Items.Clear();
                foreach (Departement departement in departements)
                {
                    ComboBoxDepartement.Items.Add(departement.Name);
                }
                if (ComboBoxDepartement.Items.Count > 0)
                {
                    ComboBoxDepartement.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur est survenue lors du chargement de la comboBox");
            }
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
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur est survenue lors du chargement du scoreboard");
            }
        }

        public void LoadWithFilter(object sender, RoutedEventArgs e)
        {
            string selectedDepartment = ComboBoxDepartement.SelectedItem.ToString();
            if (IsGeneral) LoadScoreboardByDepartement(selectedDepartment);
            else LoadScoreboardWeekByDepartement(selectedDepartment);
        }


        public void LoadScoreboardGeneral(object sender, RoutedEventArgs e)
        {
            try
            {
                IsGeneral = true;
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

        public void LoadScoreboardWeek(object sender, RoutedEventArgs e)
        {
            try
            {
                IsGeneral = false;
                DAL dal = new DAL();
                Dictionary<User, Dictionary<string, int>> pointsByUserAndWeek = dal.UserFact.GetPointsByWeek();
                List<UserViewModel> dataList = new List<UserViewModel>();

                foreach (var userPointsEntry in pointsByUserAndWeek)
                {
                    User user = userPointsEntry.Key;
                    Dictionary<string, int> pointsByWeek = userPointsEntry.Value;

                    // Vous pouvez ajuster cette partie pour afficher les informations comme vous le souhaitez
                    foreach (var weekEntry in pointsByWeek)
                    {
                       
                        int points = weekEntry.Value;
                        dataList.Add(new UserViewModel(user, points));
                    }
                }

                dataGrid.ItemsSource = dataList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur est survenue lors du chargement du scoreboard : " + ex.Message);
            }
        }


        public void LoadScoreboardByDepartement(string departement)
        {
            try
            {
                DAL dal = new DAL();
                List<User>? usersList = dal.UserFact.GetAllUser();
                List<UserViewModel> dataList = new List<UserViewModel>();
                int points = 0;
                foreach (User user in usersList)
                {
                    points = dal.UserFact.GetAllPointOfUserByDepartement(user,departement);
                    dataList.Add(new UserViewModel(user, points));
                }
                dataGrid.ItemsSource = dataList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur est survenue lors du chargement du scoreboard");
            }
        }

        public void LoadScoreboardWeekByDepartement(string departement)
        {
            try
            {
                
                DAL dal = new DAL();
                Dictionary<User, Dictionary<string, int>> pointsByUserAndWeek = dal.UserFact.GetPointsByWeekByDepartement(departement);
                List<UserViewModel> dataList = new List<UserViewModel>();

                foreach (var userPointsEntry in pointsByUserAndWeek)
                {
                    User user = userPointsEntry.Key;
                    Dictionary<string, int> pointsByWeek = userPointsEntry.Value;

                    // Vous pouvez ajuster cette partie pour afficher les informations comme vous le souhaitez
                    foreach (var weekEntry in pointsByWeek)
                    {

                        int points = weekEntry.Value;
                        dataList.Add(new UserViewModel(user, points));
                    }
                }

                dataGrid.ItemsSource = dataList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur est survenue lors du chargement du scoreboard : " + ex.Message);
            }
        }



        private void BackToLandingPage(object sender, RoutedEventArgs e)
        {
            MainWindowVM.Instance.CurrentPage = new LandingPage();
        }
    }
}
