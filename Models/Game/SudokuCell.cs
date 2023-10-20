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

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
