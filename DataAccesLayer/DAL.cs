using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameOnUnlimited.DataAccesLayer.Factories;
namespace GameOnUnlimited.DataAccesLayer
{
    public class DAL
    {
        public static string? ConnectionString { get; set; }
        private UserFactory? _userFact = null;
        private DepartementFactory? _departementFact = null;

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
