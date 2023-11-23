using GameOn.DataAccesLayer;
using GameOn.Views.Pages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using GameOn.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Org.BouncyCastle.Asn1.Ocsp;


namespace GameOn.Models.Game
{
    public class SudokuLogic
    {
        public const int GRIDSIZE = 9;
        public SudokuCell[,] Grid { get; private set; }
        public int[,] SolutionGrid { get; private set; }
        public bool NoteEnabled { get; set; }
        public Page SudokuPage {get;set;}
        public bool IsRanked { get;set;}
        
        public SudokuLogic(SudokuCell[,] grid, int[,] solutionGrid, Page sudokuPage, bool isRanked)
        {
            Grid = grid;
            SolutionGrid = solutionGrid;
            NoteEnabled = false;
            SudokuPage = sudokuPage;
            IsRanked = isRanked;
        }

        public void SaveGame(Page sudokuPage, SudokuParticipation sudokuParticipation)
        {
            //Actualiser la grille de gameLogic avec tout les chiffres de la vue
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    //on cherche la case au coordonée i j
                    TextBox textBox = (TextBox)sudokuPage.FindName($"text{i}{j}");
                    if (textBox != null)
                    {
                        Grid[i, j] = new SudokuCell
                        {
                            IsEditable = !textBox.IsReadOnly,
                            IsNote = bool.Parse(textBox.Tag as string),
                            Value = int.Parse(textBox.Text != "" ? textBox.Text : "0")
                        };
                    }
                    else
                    {
                        //si on ne trouve pas de textbox a ces coordonées c'est qu'il s'agit d'une note, et donc d'une grille
                        Grid grid = (Grid)sudokuPage.FindName($"notegrid{i}{j}");
                        Grid[i, j] = new SudokuCell
                        {
                            IsEditable = true,
                            IsNote = true,
                            Value = 0
                        };
                        foreach (TextBox element in grid.Children)
                        {
                            int noteRow = System.Windows.Controls.Grid.GetRow(element);
                            int noteCol = System.Windows.Controls.Grid.GetColumn(element);
                            Grid[i, j].Notes[noteRow, noteCol] = int.Parse(element.Text);
                        }
                    }

                }
            }
            string jsonGrid = JsonConvert.SerializeObject(Grid);
            sudokuParticipation.ActualGrid = jsonGrid;
            if(IsWin())
            {
                sudokuParticipation.EndDate = DateTime.Now;
                MessageBox.Show("Win !");
                if(IsRanked)
                {
                    GivePoints(sudokuParticipation);
                    Notification notification = new Notification(0, $"Félicitation vous avez gagné {sudokuParticipation.PointWon} points", false, sudokuParticipation.UserId);
                    new DAL().NotifFactory.Save(notification);
                }
            }
            new DAL().SudokuParticipationFact.Save(sudokuParticipation);

        }
        public void TextBox_TextChanged(object sender, TextChangedEventArgs args)
        {
            TextBox textBox = (TextBox)sender;
            string newText = textBox.Text;
            textBox.Tag = NoteEnabled.ToString();

            Grid parentGrid = (Grid)textBox.Parent;

            if (!NoteEnabled)
            {
                if (ShouldSetBackgroundWhite(parentGrid))
                {
                    textBox.Background = System.Windows.Media.Brushes.White;
                }
                else
                {
                    textBox.Background = System.Windows.Media.Brushes.LightBlue;
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
        public void CreateNote(TextBox textBox, int iLogicRow, int iLogicCol)
        {
            //Cherche sur la quel des 9 sous grilles la notes doit etre mise 
            Grid parentGrid = (Grid)textBox.Parent;

            int viewRow = System.Windows.Controls.Grid.GetRow(textBox);
            int viewCol = System.Windows.Controls.Grid.GetColumn(textBox);

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
            SudokuPage.RegisterName(newGrid.Name, newGrid);

            SudokuPage.UnregisterName(textBoxName);

            // Ajoutez la nouvelle Grid au parentGrid
            parentGrid.Children.Add(newGrid);

            // Ajoutez la nouvelle Grid au parentGrid en spécifiant la position de row et col
            System.Windows.Controls.Grid.SetRow(newGrid, viewRow);
            System.Windows.Controls.Grid.SetColumn(newGrid, viewCol);

            // Ajoutez des TextBox à la nouvelle Grid 
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    TextBox newTextBox = new TextBox();
                    newTextBox.Text = Grid[iLogicRow, iLogicCol].Notes[i, j].ToString();
                    System.Windows.Controls.Grid.SetRow(newTextBox, i);
                    System.Windows.Controls.Grid.SetColumn(newTextBox, j);
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

        //retourne a un textbox au lieu de la grille de note
        public void CreateTextBox(Grid noteGrid)
        {
            Grid parentGrid = (Grid)noteGrid.Parent;

            int viewRow = System.Windows.Controls.Grid.GetRow(noteGrid);
            int viewCol = System.Windows.Controls.Grid.GetColumn(noteGrid);
            int logicRow = int.Parse(noteGrid.Name[8].ToString());
            int logicCol = int.Parse(noteGrid.Name[9].ToString());
            TextBox newTextBox = new TextBox();

            string gridName = noteGrid.Name;
            newTextBox.Text = "";
            newTextBox.FontSize = 30;
            newTextBox.Name = $"text{logicRow}{logicCol}";

            SudokuPage.RegisterName(newTextBox.Name, newTextBox);

            SudokuPage.UnregisterName(gridName);
            // Ajoutez la nouvelle Grid au parentGrid
            parentGrid.Children.Add(newTextBox);

            // Ajoutez la nouvelle Grid au parentGrid en spécifiant la position de row et col
            System.Windows.Controls.Grid.SetRow(newTextBox, viewRow);
            System.Windows.Controls.Grid.SetColumn(newTextBox, viewCol);

            newTextBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            newTextBox.VerticalContentAlignment = VerticalAlignment.Center;
            newTextBox.PreviewMouseDown += TextBox_Click;
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
        public void Notes_Click(object sender, RoutedEventArgs e)
        {
            if (!NoteEnabled)
            {
                TextBox textBoxClicked = (TextBox)sender;
                Grid noteGrid = (Grid)textBoxClicked.Parent;
                CreateTextBox(noteGrid);

            }
        }
        public void TextBox_Click(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (NoteEnabled && !textBox.IsReadOnly)
            {
                int logicRow = int.Parse(textBox.Name[4].ToString());
                int logicCol = int.Parse(textBox.Name[5].ToString());
                CreateNote(textBox, logicRow, logicCol);

            }
        }
        public void CheckNotes()
        {
            NoteEnabled = true;
        }
        public void UnCheckNotes()
        {
            NoteEnabled = false;
        }
        public bool IsValidInteger(string text, out int value)
        {
            return int.TryParse(text, out value);
        }
        public bool IsInRange(int value, int min, int max)
        {
            return value >= min && value <= max;
        }
        public bool IsWin()
        {
            if (HasNote())
                return false;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int a = SolutionGrid[i, j];
                    int b =(int)Grid[i, j].Value;
                    if (SolutionGrid[i,j] != (int)Grid[i,j].Value)
                        return false;
                }
            }
            return true;

        }
        public bool HasNote()
        {
            for(int i = 0; i < 9;i++)
            {
                Grid g = (Grid)SudokuPage.FindName($"grid{i}");
                foreach(UIElement children in g.Children)
                {
                    if (children is FrameworkElement element && element.Name.StartsWith("notegrid"))
                        return true;

                }
            }
            return false;


        }
        public void GivePoints(SudokuParticipation sudokuParticipation)
        {
            //check si il y'a déja des game fini pour ce sudoku
            
            int nbWins = new DAL().SudokuParticipationFact.GetNbWins(sudokuParticipation.SudokuId);
            switch (nbWins)
            {
                case 0:
                    sudokuParticipation.PointWon = 10;
                    break;
                case 1:
                    sudokuParticipation.PointWon = 6;
                    break;
                case 2: case 3: case 4:
                    sudokuParticipation.PointWon = 3;
                    break;
                default:
                    sudokuParticipation.PointWon = 1;
                    break;
            }

        }
        public void ClearGrid()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    TextBox textBox = (TextBox)SudokuPage.FindName($"text{i}{j}");
                    if (textBox != null)
                    {
                        textBox.Text = "";
                    }
                    else
                    {
                        Grid grid = (Grid)SudokuPage.FindName($"notegrid{i}{j}");
                        this.CreateTextBox(grid);
                    }

                }
            }
        }


        public static bool ShouldSetBackgroundWhite(Grid parentGrid)
        {
            string[] whiteGrids = { "grid1", "grid3", "grid5", "grid7" };
            return whiteGrids.Contains(parentGrid.Name);
        }
    }
}
