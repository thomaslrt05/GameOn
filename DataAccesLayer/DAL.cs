using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameOn.DataAccesLayer.Factories;
using GameOn.Models;

namespace GameOn.DataAccesLayer
{
    public class DAL
    {
        
        public static string? ConnectionString { get; set; }
        private UserFactory? _userFact = null;
        private DepartementFactory? _departementFact = null;
        public User? userConnected { get; set; } 

        public DepartementFactory DepartementFact
        {
            get
            {
                if (_departementFact == null)
                {
                    _departementFact = new DepartementFactory();
                }

                return _departementFact;
            }
        }
        public UserFactory UserFact
        {
            get
            {
                if (_userFact == null)
                {
                    _userFact = new UserFactory();
                }

                return _userFact;
            }
        }





    }
}
