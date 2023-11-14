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
using System.Reflection;
using System.Windows.Controls.Primitives;

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

        private DispatcherTimer gameTimerRemain;
        private TimeSpan gameTimeRemaining;

        private DispatcherTimer gameTimer;
        private TimeSpan gameTime;
        public bool NoteEnabled { get; set; }


        public SudokuRankedPage()
        {
            NoteEnabled = false;
            InitializeComponent();
            this.DataContext = new SudokuRankedPageVM();
            InitPage();
        }
        private async Task InitPage()
        {
            await LoadSudokuAsync();
            await LoadTimerRemain();
            await LoadTimerGame();
        }

        private async Task LoadTimerGame()
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Tick += GameTimer_TickGame;
            gameTimer.Interval = TimeSpan.FromSeconds(1);

            DateTime startTimer = SudokuParticipation.StartDate;
            DateTime currentTime = DateTime.Now;
            gameTime = currentTime - startTimer;
            gameTimer.Start();
        }

        private void GameTimer_TickGame(object sender, EventArgs e)
        {
            gameTime = gameTime.Add(TimeSpan.FromSeconds(1));
            TimerGame.Content = gameTime.ToString(@"hh\:mm\:ss");

        }
        private async Task LoadTimerRemain() 
        {
            gameTimerRemain = new DispatcherTimer();
            gameTimerRemain.Tick += GameTimer_Tick;
            gameTimerRemain.Interval = TimeSpan.FromSeconds(1);

            DateTime startTimer = _SudokuModel.CreationDate;
            DateTime dateEndTimer = startTimer.AddDays(1);
            DateTime currentTime = DateTime.Now;
            gameTimeRemaining = dateEndTimer - currentTime;
            gameTimerRemain.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            gameTimeRemaining = gameTimeRemaining.Subtract(TimeSpan.FromSeconds(1));
            TimerRemain.Content = gameTimeRemaining.ToString(@"hh\:mm\:ss");

        }
        private async Task LoadSudokuAsync()
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

                if(_SudokuModel == null)
                {
                    _SudokuModel = dal.SudokuFactory.GetGameOfTheDay();
                }

                //transformer la participation en gameLogic
                _SudokuLogic = SudokuLogic.SudokuParticipationToLogic(SudokuParticipation);

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
                        if(cellValue.IsNote)
                        {
                            CreateNote(textBox,iLogicRow,iLogicCol);
                        }
                    }

                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite lors du chargement du Sudoku : " + ex.Message);
            }
        }

        private void CreateNote(TextBox textBox,int iLogicRow, int iLogicCol)
        {
            //Cherche sur la quel des 9 sous grilles la notes doit etre mise 
            Grid parentGrid = (Grid)textBox.Parent;
            
            int viewRow = Grid.GetRow(textBox);
            int viewCol = Grid.GetColumn(textBox);

            // Créez une nouvelle Grid de 3x3
            Grid newGrid = new Grid();

            for (int a = 0; a < 3; a++)
            {
                newGrid.RowDefinitions.Add(new RowDefinition());
                newGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            string textBoxName = textBox.Name;
            //on donne un nom a la grille pour pouvoir la retrouver quand on enregistera la game
            newGrid.Name = $"notegrid{iLogicRow}{iLogicCol}";
            this.RegisterName(newGrid.Name, newGrid);

            this.UnregisterName(textBoxName);

            // Ajoutez la nouvelle Grid au parentGrid
            parentGrid.Children.Add(newGrid);
            
            // Ajoutez la nouvelle Grid au parentGrid en spécifiant la position de row et col
            Grid.SetRow(newGrid, viewRow);
            Grid.SetColumn(newGrid, viewCol);

            // Ajoutez des TextBox à la nouvelle Grid 
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    TextBox newTextBox = new TextBox();
                    newTextBox.Text = _SudokuLogic.Grid[iLogicRow, iLogicCol].Notes[i, j].ToString();
                    Grid.SetRow(newTextBox, i);
                    Grid.SetColumn(newTextBox, j);
                    newTextBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                    newTextBox.VerticalContentAlignment = VerticalAlignment.Center;
                    newTextBox.PreviewMouseDown += Notes_Click;
                    newTextBox.TextChanged += TextBox_TextChanged;
                    newGrid.Children.Add(newTextBox);

                }
            }
            //on enleve la textbox de la grille
            parentGrid.Children.Remove(textBox);
        }
        public void SaveGame(object sender, RoutedEventArgs e)
        {
            //Actualiser la grille de gameLogic avec tout les chiffres de la vue
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++) 
                {
                    //on cherche la case au coordonée i j
                    TextBox textBox = (TextBox)this.FindName($"text{i}{j}");
                    if(textBox!=null)
                    {
                        _SudokuLogic.Grid[i, j] = new SudokuCell
                        {
                            IsEditable = !textBox.IsReadOnly,
                            IsNote = bool.Parse(textBox.Tag as string),
                            Value = int.Parse(textBox.Text != "" ? textBox.Text : "0")
                        };
                    }
                    else
                    {
                        //si on ne trouve pas de textbox a ces coordonées c'est qu'il s'agit d'une note, et donc d'une grille
                        Grid grid = (Grid)this.FindName($"notegrid{i}{j}");
                        _SudokuLogic.Grid[i, j] = new SudokuCell
                        {
                            IsEditable = true,
                            IsNote = true,
                            Value = 0
                        };
                        foreach (TextBox element in grid.Children)
                        {
                            int noteRow = Grid.GetRow(element);
                            int noteCol = Grid.GetColumn(element);
                            _SudokuLogic.Grid[i, j].Notes[noteRow, noteCol] = int.Parse(element.Text); 
                        }
                    }

                }
            }
            string jsonGrid = JsonConvert.SerializeObject(_SudokuLogic.Grid);
            SudokuParticipation.ActualGrid = jsonGrid;
            new DAL().SudokuParticipationFact.Save(SudokuParticipation);

        }

        //transforme un text box en grid pour les notes multiples
        private void Notes_Click(object sender, RoutedEventArgs e)
        {
            if(!NoteEnabled)
            {
                TextBox textBoxClicked = (TextBox)sender;
                Grid noteGrid = (Grid)textBoxClicked.Parent;
                Grid parentGrid = (Grid)noteGrid.Parent;

                int viewRow = Grid.GetRow(noteGrid);
                int viewCol = Grid.GetColumn(noteGrid);
                int logicRow = int.Parse(noteGrid.Name[8].ToString());
                int logicCol = int.Parse(noteGrid.Name[9].ToString());
                TextBox newTextBox = new TextBox();

                string gridName = noteGrid.Name;
                newTextBox.Text = "";
                newTextBox.FontSize = 30;
                newTextBox.Name = $"text{logicRow}{logicCol}";

                this.RegisterName(newTextBox.Name, newTextBox);

                this.UnregisterName(gridName);
                // Ajoutez la nouvelle Grid au parentGrid
                parentGrid.Children.Add(newTextBox);

                // Ajoutez la nouvelle Grid au parentGrid en spécifiant la position de row et col
                Grid.SetRow(newTextBox, viewRow);
                Grid.SetColumn(newTextBox, viewCol);

                newTextBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                newTextBox.VerticalContentAlignment = VerticalAlignment.Center;
                newTextBox.PreviewMouseDown += Notes_Click;
                newTextBox.TextChanged += TextBox_TextChanged;
                newTextBox.Tag = "false";
                newTextBox.IsReadOnly = false;

                if (ShouldSetBackgroundWhite(parentGrid))
                {
                    newTextBox.Background = Brushes.White;
                }
                else
                {
                    newTextBox.Background = Brushes.LightBlue;
                }

                parentGrid.Children.Remove(noteGrid);


            }
        }

        private void TextBox_Click(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (NoteEnabled && !textBox.IsReadOnly)
            {                
                int logicRow = int.Parse(textBox.Name[4].ToString());
                int logicCol = int.Parse(textBox.Name[5].ToString());
                CreateNote(textBox,logicRow,logicCol);

            }
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs args)
        {
            TextBox textBox = (TextBox)sender;
            string newText = textBox.Text;
            textBox.Tag = NoteEnabled.ToString();

            Grid parentGrid = (Grid)textBox.Parent;

            if(!NoteEnabled)
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
