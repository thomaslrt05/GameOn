using System;
using System.Collections.Generic;
using System.Linq;
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

        
    }

}
