using GameOn.DataAccesLayer;
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
        //permet de crée un object SudokuLogic a partir d'un sudoku dans la bd
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

    }
}
