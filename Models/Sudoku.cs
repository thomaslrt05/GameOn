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

        public static async Task<Sudoku> CreateSudoku()
        {
            String content = await GetNewSudoku();

            JObject jsonObject = JObject.Parse(content);
            JArray grid = (JArray)jsonObject["newboard"]["grids"][0]["value"];
            JArray gridSolution = (JArray)jsonObject["newboard"]["grids"][0]["solution"];

            Sudoku sudoku = new Sudoku(0, DateTime.Today, grid.ToString(), gridSolution.ToString(), true, 1);
            return sudoku;
        }
        static async Task<string> GetNewSudoku()
        {
            string url = "https://sudoku-api.vercel.app/api/dosuku";

            HttpClient client = new HttpClient();
            string content = await client.GetStringAsync(url);

            return content;
        }

    }

}
