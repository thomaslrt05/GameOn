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
        private readonly string AllDepartementFilter = "Par departement"; 
        private readonly string NoFilter = "Aucun filtre";
        public bool IsGeneralMode { get; set; } = true;
        public bool IsWeekMode { get; set; } = false;

        public SudokuScoreboardPage()
        {
            InitializeComponent();
            LoadColumnsUserDataGrid();
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
                ComboBoxDepartement.Items.Add(NoFilter);

                foreach (Departement departement in departements)
                {
                    ComboBoxDepartement.Items.Add(departement.Name);
                }
                ComboBoxDepartement.Items.Add(AllDepartementFilter);

                ComboBoxDepartement.SelectedIndex = 0;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur est survenue lors du chargement de la comboBox");
            }
        }

        public void LoadScoreboard()
        {
            LoadColumnsUserDataGrid();

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

        
        private void LoadColumnsUserDataGrid()
        {
            DeleteColumnsDataGrid();

            DataGridTextColumn colonneNom = new DataGridTextColumn
            {
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                Header = "Nom",
                Binding = new System.Windows.Data.Binding("Utilisateur.LastName")
            };

            DataGridTextColumn colonnePrenom = new DataGridTextColumn
            {
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                Header = "Prénom",
                Binding = new System.Windows.Data.Binding("Utilisateur.Name")
            };

            DataGridTextColumn colonneDepartement = new DataGridTextColumn
            {
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                Header = "Département",
                Binding = new System.Windows.Data.Binding("Utilisateur.Departement.Name")
            };

            DataGridTextColumn colonnePoints = new DataGridTextColumn
            {
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                Header = "Total points",
                Binding = new System.Windows.Data.Binding("Points")
            };

            Style stylePoints = new Style(typeof(TextBlock));
            stylePoints.Setters.Add(new Setter(TextBlock.FontWeightProperty, FontWeights.Bold));
            colonnePoints.ElementStyle = stylePoints;

            dataGrid.Columns.Add(colonneNom);
            dataGrid.Columns.Add(colonnePrenom);
            dataGrid.Columns.Add(colonneDepartement);
            dataGrid.Columns.Add(colonnePoints);
        }
        private void DeleteColumnsDataGrid()
        {
            dataGrid.Columns.Clear();
        }
        
        private void LoadColumnsDepartement()
        {
            DeleteColumnsDataGrid();

            DataGridTextColumn colonneDepartement = new DataGridTextColumn
            {
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                Header = "Département",
                Binding = new System.Windows.Data.Binding("Departement.Name")
            };

            DataGridTextColumn colonnePoints = new DataGridTextColumn
            {
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                Header = "Points",
                Binding = new System.Windows.Data.Binding("Points")
            };

            dataGrid.Columns.Add(colonneDepartement);
            dataGrid.Columns.Add(colonnePoints);
        }


        public void LoadWithFilter(object sender, RoutedEventArgs e)
        {
            string selectedDepartment = ComboBoxDepartement.SelectedItem.ToString();
            if (IsGeneralMode) LoadFiltredScoreboardOfDepartement(selectedDepartment);
            else if (IsWeekMode) LoadFiltredScoreboardOfDepartementByWeek(selectedDepartment);
        }




        public void LoadScoreboardGeneral(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadColumnsUserDataGrid();
                IsGeneralMode = true;
                IsWeekMode = false;
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

        public void LoadScoreboardWeek(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadColumnsUserDataGrid();
                IsGeneralMode = false;
                IsWeekMode = true;
                DAL dal = new DAL();
                Dictionary<User, Dictionary<string, int>> pointsByUserAndWeek = dal.UserFact.GetPointsByWeek();
                List<UserViewModel> weeklyRankingList = new List<UserViewModel>();

                foreach (var userPointsEntry in pointsByUserAndWeek)
                {
                    User user = userPointsEntry.Key;
                    Dictionary<string, int> pointsByWeek = userPointsEntry.Value;


                    var lastWeekPoints = pointsByWeek.LastOrDefault();
                    if (lastWeekPoints.Key != null)
                    {
                        int points = lastWeekPoints.Value;
                        weeklyRankingList.Add(new UserViewModel(user, points));
                    }
                }

                dataGrid.ItemsSource = weeklyRankingList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur est survenue lors du chargement du scoreboard : " + ex.Message);
            }
        }



        public void LoadFiltredScoreboardOfDepartement(string departementName) 
        {
            try
            {
                DAL dal = new DAL();

                if (departementName == AllDepartementFilter)
                {
                    LoadColumnsDepartement();
                    List<Departement>? departements = dal.DepartementFact.GetAll();
                    if (departements is null)
                    {
                        MessageBox.Show("Il n'y a aucun département");
                        return;
                    }
                    List<DepartementViewModel> data = new List<DepartementViewModel>();

                    foreach (Departement departement in departements)
                    {
                        int p = dal.SudokuParticipationFact.GetPointsOfDepartement(departement.Id);
                        data.Add(new DepartementViewModel(departement, p));

                    }
                    dataGrid.ItemsSource = data;

                    return;

                }
                LoadColumnsUserDataGrid();

                if (departementName == NoFilter)
                {
                    LoadScoreboard();
                    return;
                }

                List<User>? usersList = dal.UserFact.GetAllUsersOfDepartement(departementName);
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



        //
        // Fonctionne pas 
        //
        public void LoadFiltredScoreboardOfDepartementByWeek(string departementName)
        {
            try
            {
                DAL dal = new DAL();

                if (departementName == AllDepartementFilter)
                {
                  
                    LoadColumnsDepartement();
                    List<Departement>? departements = dal.DepartementFact.GetAll();
                    if (departements is null)
                    {
                        MessageBox.Show("Il n'y a aucun département");
                        return;
                    }

                    List<DepartementViewModel> data = new List<DepartementViewModel>();

                    foreach (Departement departement in departements)
                    {
                        int p = dal.SudokuParticipationFact.GetPointsOfDepartementByWeek(departement.Id);
                        data.Add(new DepartementViewModel(departement, p));
                    }

                    dataGrid.ItemsSource = data;
                    return;
                }

                LoadColumnsUserDataGrid();

                if (departementName == NoFilter)
                {
                    LoadScoreboard();
                    return;
                }

               
                Dictionary<User, Dictionary<string, int>> pointsByUserAndWeek = dal.UserFact.GetPointsByWeekAndDepartement(departementName);

                
                List<UserViewModel> dataList = pointsByUserAndWeek
                    .Select(kv => new UserViewModel(kv.Key, kv.Value.Sum(w => w.Value)))
                    .ToList();

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
