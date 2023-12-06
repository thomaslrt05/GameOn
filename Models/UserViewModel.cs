using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOn.Models
{
    class UserViewModel
    {
        public User Utilisateur { get; set; }
        public int Points { get; set; }

        public UserViewModel(User utilisateur, int points)
        {
            Utilisateur = utilisateur;
            Points = points;
        }
    }
}
