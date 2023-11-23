using GameOn.DataAccesLayer;
using GameOn.Models.Game;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GameOn.Models
{
    public class SudokuParticipation
    {
        public int Id { get; set; }
        public DateTime? EndDate{ get; set; }
        public DateTime StartDate{ get; set; }
        public int PointWon { get; set; } = 0;
        public int SudokuId { get; set;}
        public int UserId { get; set;}
        public string ActualGrid { get; set; }

        public static string ModelGridToParticipationGrid(string modelGrid)
        {
            //on re crée le tableau a partir du string qui représente la grille en BD
            int[][] intArray = JsonConvert.DeserializeObject<int[][]>(modelGrid);
            SudokuCell[,] sudokuGrid = new SudokuCell[intArray.Length, intArray[0].Length];

            for (int i = 0; i < intArray.Length; i++)
            {
                for (int j = 0; j < intArray[i].Length; j++)
                {
                    int value = intArray[i][j];
                    SudokuCell cell = new SudokuCell
                    {
                        Value = value,
                        IsEditable = (value == 0),
                        IsNote = false
                    };
                    sudokuGrid[i, j] = cell;
                }
            }

            string jsonString = JsonConvert.SerializeObject(sudokuGrid);
            return jsonString;

        }

        public SudokuLogic ToLogic(Page sudokuPage, bool isRanked)
        {
            //obtenir la solution grid grace a la foreign key vers le sudoku en question
            DAL dal = new DAL();
            Sudoku? sudoku = dal.SudokuFactory.Get(SudokuId);
            if (sudoku == null) { throw new Exception("sudoku n'éxiste pas"); }

            SudokuCell[,] sudokuGrid = JsonConvert.DeserializeObject<SudokuCell[,]>(ActualGrid);

            int[,] gridSolution = JsonConvert.DeserializeObject<int[,]>(sudoku.SolutionGrid);
            return new SudokuLogic(sudokuGrid, gridSolution, sudokuPage, isRanked);

        }

    }


}
