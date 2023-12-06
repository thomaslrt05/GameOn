using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOn.Models
{
    public class DepartementViewModel
    {
        public Departement Departement { get; set; }
        public int Points { get; set; }

        public DepartementViewModel(Departement departement, int points)
        {
            Departement = departement;
            Points = points;
        }
    }


}
