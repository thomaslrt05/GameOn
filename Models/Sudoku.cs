using GameOn.DataAccesLayer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GameOn.Models
{
    public class Sudoku
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Grid { get; set; }
        public string SolutionGrid { get; set; }
        public bool IsRanked { get; set; }
        public int Difficulty { get; set; }

        
        public Sudoku(int id, DateTime creationDate, string grid, string solutionGrid, bool isRanked, int difficulty)
        {
            Id = id;
            CreationDate = creationDate;
            Grid = grid;
            SolutionGrid = solutionGrid;
            IsRanked = isRanked;
            Difficulty = difficulty;
        }
        //test
        public static async Task<Sudoku> CreateSudoku(bool isRanked = true)
        {
            String content = await GetNewSudoku();

            JObject jsonObject = JObject.Parse(content);
            JArray grid = (JArray)jsonObject["newboard"]["grids"][0]["value"];
            string difficulty = jsonObject["newboard"]["grids"][0]["difficulty"].ToString(); // Accédez à l'élément 0 pour obtenir la difficulté
            
            JArray gridSolution = (JArray)jsonObject["newboard"]["grids"][0 ]["solution"];

            Sudoku sudoku = new Sudoku(0, DateTime.Today, grid.ToString(), gridSolution.ToString(), isRanked, CheckDifficulty(difficulty));
            return sudoku;
        }
        public static async Task<Sudoku> CreateSudoku(bool isRanked, int difficulty)
        {
            Sudoku s;
            do
            {
                s = await CreateSudoku(isRanked);
            }while (s.Difficulty != difficulty);
            return s;
        }

        static async Task<string> GetNewSudoku()
        {
            string url = "https://sudoku-api.vercel.app/api/dosuku";

            HttpClient client = new HttpClient();
            string content = await client.GetStringAsync(url);

            return content;
        }

        private static int CheckDifficulty(string difficulty)
        {
            switch (difficulty)
            {
                case "Easy":
                    return 1;
                case "Medium":
                    return 2;
                default: 
                    return 3;
            }
        } 

    }

}
