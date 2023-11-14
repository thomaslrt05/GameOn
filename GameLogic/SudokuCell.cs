using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOn.Models.Game
{
    public class SudokuCell
    {
        public int? Value { get; set; }
        public bool IsEditable { get; set; }
        public bool IsNote { get; set; }

        public int[,] Notes { get; set; }

        public SudokuCell() {
            Notes = new int[3, 3];
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
