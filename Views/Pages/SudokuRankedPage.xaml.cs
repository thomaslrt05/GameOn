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
using Google.Protobuf.WellKnownTypes;
using System.ComponentModel;

namespace GameOn.Views.Pages
{
    /// <summary>
    /// Logique d'interaction pour SudokuRankedPage.xaml
    /// </summary>
    public partial class SudokuRankedPage : Page
    {
        public SudokuParticipation? SudokuParticipation { get; set; }
        public Sudoku? _SudokuModel { get; set; }
        public SudokuLogic _SudokuLogic { get; set; }
        private DispatcherTimer gameTimer;
        private TimeSpan gameTimeRemaining;
        public bool NoteEnabled { get; set; }


        public SudokuRankedPage()
        {
            NoteEnabled = false;
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

            DateTime startTimer = SudokuParticipation.StartDate;
            DateTime dateEndTimer = startTimer.AddDays(1);
            DateTime currentTime = DateTime.Now;
            gameTimeRemaining = dateEndTimer - currentTime;
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
                    //creé le sudokuParaticipation pour ce user et ce sudoku
                    SudokuParticipation = new SudokuParticipation()
                    {
                        StartDate = DateTime.Now,
                        Id = 0,
                        SudokuId = _SudokuModel.Id,
                        PointWon = 0,
                        UserId = ConnectionSingleton.UserConnected.Id,
                        //crée la grille a partir du model
                        ActualGrid = SudokuParticipation.ModelGridToParticipationGrid(_SudokuModel.Grid)
                    };
                    dal.SudokuParticipationFact.Save(SudokuParticipation);
                }

                //transformer la participation en gameLogic

                if(_SudokuModel == null)
                {
                    _SudokuModel = dal.SudokuFactory.GetGameOfTheDay();
                }
                _SudokuLogic = SudokuLogic.SudokuParticipationToLogic(SudokuParticipation);

                //ouvrire la fenetre avec les données du gameParticipation
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        TextBox textBox = (TextBox)this.FindName($"text{i}{j}");
                        SudokuCell cellValue = _SudokuLogic.Grid[i, j];

                        textBox.Text = cellValue.Value != 0 ? cellValue.Value.ToString() : "";
                        textBox.IsReadOnly = !cellValue.IsEditable;
                        textBox.Tag = cellValue.IsNote.ToString();
                        if(cellValue.IsNote)
                        {
                            textBox.Background = Brushes.Red;
                        }
                        
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

                    _SudokuLogic.Grid[i, j] = new SudokuCell { 
                        IsEditable = !textBox.IsReadOnly, 
                        IsNote = bool.Parse(textBox.Tag as string), 
                        Value = int.Parse(textBox.Text != "" ? textBox.Text : "0")
                    };

                }
            }
            string jsonGrid = JsonConvert.SerializeObject(_SudokuLogic.Grid);
            SudokuParticipation.ActualGrid = jsonGrid;
            new DAL().SudokuParticipationFact.Save(SudokuParticipation);

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs args)
        {
            // Code à exécuter lorsque le texte dans la TextBox change
            TextBox textBox = (TextBox)sender;
            string newText = textBox.Text;
            textBox.Tag = NoteEnabled.ToString();

            Grid parentGrid = (Grid)textBox.Parent;

            if (NoteEnabled)
            {
                textBox.Background = Brushes.Red;
            }
            else
            {
                if (ShouldSetBackgroundWhite(parentGrid))
                {
                    textBox.Background = Brushes.White;
                }
                else
                {
                    textBox.Background = Brushes.LightBlue;
                }
            }

            if (!IsValidInteger(newText, out int newValue) || !IsInRange(newValue, 0, 9))
            {
                if (!string.IsNullOrEmpty(newText))
                {
                    MessageBox.Show("La valeur doit être un entier entre 0 et 9.");
                }
                textBox.Text = "";
            }
        }
        private bool IsValidInteger(string text, out int value)
        {
            return int.TryParse(text, out value);
        }

        private bool IsInRange(int value, int min, int max)
        {
            return value >= min && value <= max;
        }
        private void CheckNotes(object sender, RoutedEventArgs e)
        {
            NoteEnabled = true;
        }
        private void UnCheckNotes(object sender, RoutedEventArgs e)
        {
            NoteEnabled = false;
        }
        private bool ShouldSetBackgroundWhite(Grid parentGrid)
        {
            string[] whiteGrids = { "grid1", "grid3", "grid5", "grid7" };
            return whiteGrids.Contains(parentGrid.Name);
        }

    }

}
