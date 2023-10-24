using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
