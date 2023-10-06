using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOn.Models
{
    public class Departement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Departement() { }
        public Departement(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
