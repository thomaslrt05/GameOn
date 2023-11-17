using GameOn.DataAccesLayer;
using GameOn.Models.Game;
using GameOn.Models;
using GameOn.ViewModels;
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
using System.Windows.Threading;

namespace GameOn.Views.Pages
{
    /// <summary>
    /// Logique d'interaction pour SudokuPracticePage.xaml
    /// </summary>
    public partial class SudokuPracticePage : Page
    {
        public SudokuParticipation? _SudokuParticipation { get; set; }
        public Sudoku? _SudokuModel { get; set; }
        public SudokuLogic _SudokuLogic { get; set; }
        private DispatcherTimer gameTimer;
        private TimeSpan gameTime;

        public SudokuPracticePage()
        {
            InitializeComponent();
            this.DataContext = new SudokuRankedPageVM();
            InitPage();
        }
        private async Task InitPage()
        {
            await LoadSudokuAsync();
            await LoadTimerGame();
        }

        private async Task LoadTimerGame()
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Tick += GameTimer_TickGame;
            gameTimer.Interval = TimeSpan.FromSeconds(1);

            DateTime startTimer = _SudokuParticipation.StartDate;
            DateTime currentTime = DateTime.Now;
            gameTime = currentTime - startTimer;
            gameTimer.Start();
        }
        private void GameTimer_TickGame(object sender, EventArgs e)
        {
            gameTime = gameTime.Add(TimeSpan.FromSeconds(1));
            TimerGame.Content = gameTime.ToString(@"hh\:mm\:ss");

        }
        private async Task LoadSudokuAsync()
        {
            try
            {
                DAL dal = new DAL();
                int difficulty = 1;

                //Check si il y'a déja une participation du user dans cette difficulté
                _SudokuParticipation = dal.SudokuParticipationFact.GetLastUnrankedSudokuOfUser(ConnectionSingleton.UserConnected.Id,difficulty);

                if (_SudokuParticipation == null)
                {

                    _SudokuModel = await Sudoku.CreateSudoku(isRanked:false, difficulty); ;
                    dal.SudokuFactory.Save(_SudokuModel);
                    
                    //creé le sudokuParaticipation pour ce user et ce sudoku
                    _SudokuParticipation = new SudokuParticipation()
                    {
                        StartDate = DateTime.Now,
                        Id = 0,
                        SudokuId = _SudokuModel.Id,
                        PointWon = 0,
                        UserId = ConnectionSingleton.UserConnected.Id,
                        //crée la grille a partir du model
                        ActualGrid = SudokuParticipation.ModelGridToParticipationGrid(_SudokuModel.Grid)
                    };
                    dal.SudokuParticipationFact.Save(_SudokuParticipation);
                }

                if (_SudokuModel == null)
                {
                    _SudokuModel = dal.SudokuFactory.Get(_SudokuParticipation.SudokuId);
                }

                //transformer la participation en gameLogic
                _SudokuLogic = SudokuLogic.SudokuParticipationToLogic(_SudokuParticipation, this);

                //ouvrire la fenetre avec les données du gameParticipation
                for (int iLogicRow = 0; iLogicRow < 9; iLogicRow++)
                {
                    for (int iLogicCol = 0; iLogicCol < 9; iLogicCol++)
                    {
                        TextBox textBox = (TextBox)this.FindName($"text{iLogicRow}{iLogicCol}");
                        SudokuCell cellValue = _SudokuLogic.Grid[iLogicRow, iLogicCol];

                        textBox.Text = cellValue.Value != 0 ? cellValue.Value.ToString() : "";
                        textBox.IsReadOnly = !cellValue.IsEditable;
                        textBox.Tag = cellValue.IsNote.ToString();
                        // si c'est une note crée la grille pour les notes
                        if (cellValue.IsNote)
                        {
                            CreateNote(textBox, iLogicRow, iLogicCol);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite lors du chargement du Sudoku : " + ex.Message);
            }
        }
        private void CreateNote(TextBox textBox, int iLogicRow, int iLogicCol)
        {
            _SudokuLogic.CreateNote(textBox, iLogicRow, iLogicCol);
        }
        public void SaveGame(object sender, RoutedEventArgs e)
        {
            _SudokuLogic.SaveGame(this, _SudokuParticipation);
        }
        //transforme un text box en grid pour les notes multiples
        private void Notes_Click(object sender, RoutedEventArgs e)
        {
            _SudokuLogic.Notes_Click(sender, e);
        }
        private void TextBox_Click(object sender, RoutedEventArgs e)
        {
            _SudokuLogic.TextBox_Click(sender, e);
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs args)
        {
            _SudokuLogic.TextBox_TextChanged(sender, args);
        }
        private void CheckNotes(object sender, RoutedEventArgs e)
        {
            _SudokuLogic.CheckNotes();
        }
        private void UnCheckNotes(object sender, RoutedEventArgs e)
        {
            _SudokuLogic.UnCheckNotes();
        }
    }
}
