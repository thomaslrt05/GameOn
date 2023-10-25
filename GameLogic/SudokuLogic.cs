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
        public int[,] Grid { get; private set; }
        public int[,] SolutionGrid { get; private set; }
        public SudokuLogic(int[,] grid, int[,] solutionGrid)
        {
            Grid = grid;
            SolutionGrid = solutionGrid;
        }

        public void PrintGrid()
        {
            StringBuilder stringBuilder = new StringBuilder("Grille:\n");
            for (int i = 0; i < GRIDSIZE; i++)
            {
                for (int j = 0; j < GRIDSIZE; j++)
                {
                    stringBuilder.Append(" " + Grid[i, j]);
                }

                stringBuilder.Append("\n");
            }
            stringBuilder.Append("Solution : \n");
            for (int i = 0; i < GRIDSIZE; i++)
            {
                for (int j = 0; j < GRIDSIZE; j++)
                {
                    stringBuilder.Append(" " + SolutionGrid[i, j]);
                }
                stringBuilder.Append("\n");
            }
            Console.WriteLine(stringBuilder.ToString());
        }
        public static SudokuLogic JsonToGame(string json)
        {
            JObject jsonObject = JObject.Parse(json);
            JArray grid = (JArray)jsonObject["newboard"]["grids"][0]["value"];
            JArray gridSolution = (JArray)jsonObject["newboard"]["grids"][0]["solution"];
            int[,] gridInt = new int[9, 9];
            int[,] gridSolutionInt = new int[9, 9];
            int iRow = 0;
            foreach (JArray row in grid)
            {
                for (int iCol = 0; iCol < 9; iCol++)
                {
                    gridInt[iRow, iCol] = int.Parse(row[iCol].ToString());

                }
                iRow++;
            }

            iRow = 0;
            foreach (JArray row in gridSolution)
            {
                for (int iCol = 0; iCol < 9; iCol++)
                {
                    gridSolutionInt[iRow, iCol] = int.Parse(row[iCol].ToString());

                }
                iRow++;
            }

            return new SudokuLogic(gridInt, gridSolutionInt);


        }


        //permet de crée un object SudokuLogic a partir d'un sudoku dans la bd
        public static SudokuLogic SudokuParticipationToLogic(SudokuParticipation participation)
        {
            //obtenir la solution grid grace a la foreign key vers le sudoku en question
            DAL dal = new DAL();
            Sudoku? sudoku = dal.SudokuFactory.Get(participation.SudokuId);
            if (sudoku == null) { throw new Exception("sudoku n'éxiste pas"); }

            int[,] grid = JsonConvert.DeserializeObject<int[,]>(participation.ActualGrid);
            int[,] gridSolution = JsonConvert.DeserializeObject<int[,]>(sudoku.Grid);
            return new SudokuLogic(grid, gridSolution);

        }


    }
}
