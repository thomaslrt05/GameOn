using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using GameOn.Models.Game;
using GameOn.Models;
using GameOn.ViewModels;
using GameOn.DataAccesLayer;
using System.Threading.Channels;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace GameOn.Views.Pages
{
    /// <summary>
    /// Logique d'interaction pour SudokuRankedPage.xaml
    /// </summary>
    public partial class SudokuRankedPage : Page
    {
        public SudokuParticipation? SudokuParticipation { get; set; }
        public Sudoku? SudokuModel { get; set; }
        public SudokuLogic _SudokuLogic { get; set; }
        private DispatcherTimer gameTimer;
        private TimeSpan gameTimeRemaining;


        public SudokuRankedPage()
        {
            InitializeComponent();
            this.DataContext = new SudokuRankedPageVM();
            LoadSudokuAsync();
            LoadTimer();
        }

        private void LoadTimer() 
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            DateTime dateTime = DateTime.Today.Add(new TimeSpan(24, 0, 0));
            gameTimeRemaining = dateTime - SudokuParticipation.StartDate;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            gameTimeRemaining = gameTimeRemaining.Subtract(TimeSpan.FromSeconds(1));
            Timer.Content = gameTimeRemaining.ToString(@"hh\:mm\:ss");

        }
        private async void LoadSudokuAsync()
        {
            try
            {
                DAL dal = new DAL();

                //obtenir la participation au sudoku du jour
                SudokuParticipation = dal.SudokuParticipationFact.GetTodayParticipationOfUser(ConnectionSingleton.UserConnected.Id);

                if(SudokuParticipation == null)
                {
                    //si il n'y a pas de participation, obtenir le sudoku du jour
                    _SudokuModel = dal.SudokuFactory.GetGameOfTheDay();

                    // si le sudoku du jour n'existe pas le crée
                    if (_SudokuModel == null)
                    {
                        _SudokuModel = await Sudoku.CreateSudoku();
                        dal.SudokuFactory.Save(_SudokuModel);
                    }
                    //creé le sudokuParaticipation pour ce sudoku
                    SudokuParticipation = new SudokuParticipation()
                    {
                        StartDate = DateTime.Now,
                        Id = 0,
                        SudokuId = _SudokuModel.Id,
                        PointWon = 0,
                        UserId = ConnectionSingleton.UserConnected.Id,
                        ActualGrid = _SudokuModel.Grid
                    };
                    dal.SudokuParticipationFact.Save(SudokuParticipation);
                }

                //transformer la participation en gameLogic

                if(_SudokuModel == null)
                {
                    _SudokuModel = dal.SudokuFactory.GetGameOfTheDay();
                }
                _SudokuLogic = SudokuLogic.SudokuParticipationToLogic(SudokuParticipation);

                //met en premier les cases qui ne peuvent pas etre edité
                int[,] modelgrid = JsonConvert.DeserializeObject<int[,]>(_SudokuModel.Grid);
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        TextBox textBox = (TextBox)this.FindName($"text{i}{j}");
                        textBox.IsReadOnly = modelgrid[i,j] != 0;
                    }
                }

                //ouvrire la fenetre avec les données du gameParticipation
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        TextBox textBox = (TextBox)this.FindName($"text{i}{j}");

                        textBox.Text = int.Parse(_SudokuLogic.Grid[i, j].ToString()) != 0 ? _SudokuLogic.Grid[i, j].ToString() : "";
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite lors du chargement du Sudoku : " + ex.Message);
            }
        }

        public void SaveGame(object sender, RoutedEventArgs e)
        {
            //Actualiser la grille de gameLogic avec tout les chiffres de la vue
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    TextBox textBox = (TextBox)this.FindName($"text{i}{j}");
                    _SudokuLogic.Grid[i, j] = int.Parse(textBox.Text == ""? "0": textBox.Text);

                }
            }
            string jsonGrid = JsonConvert.SerializeObject(_SudokuLogic.Grid);
            SudokuParticipation.ActualGrid = jsonGrid;
            new DAL().SudokuParticipationFact.Save(SudokuParticipation);

        }

    }

}
