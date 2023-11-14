using GameOn.DataAccesLayer;
using GameOn.Views.Pages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace GameOn.Models.Game
{
    public class SudokuLogic
    {
        public const int GRIDSIZE = 9;
        public SudokuCell[,] Grid { get; private set; }
        public int[,] SolutionGrid { get; private set; }
        public SudokuLogic(SudokuCell[,] grid, int[,] solutionGrid)
        {
            Grid = grid;
            SolutionGrid = solutionGrid;
        }
        public static SudokuLogic SudokuParticipationToLogic(SudokuParticipation participation)
        {
            //obtenir la solution grid grace a la foreign key vers le sudoku en question
            DAL dal = new DAL();
            Sudoku? sudoku = dal.SudokuFactory.Get(participation.SudokuId);
            if (sudoku == null) { throw new Exception("sudoku n'éxiste pas"); }

            SudokuCell[,] sudokuGrid = JsonConvert.DeserializeObject<SudokuCell[,]>(participation.ActualGrid);

            int[,] gridSolution = JsonConvert.DeserializeObject<int[,]>(sudoku.Grid);
            return new SudokuLogic(sudokuGrid, gridSolution);

        }

        //todo: Mettre toutes les fonctions de SudokuRankedPage dans cette classes pour pouvoir les réutiliser avec les autres modes de sudoku
        public void SaveGame(Page sudokuPage)
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
            //SudokuParticipation.ActualGrid = jsonGrid;
            //new DAL().SudokuParticipationFact.Save(SudokuParticipation);
        }
    }
}
