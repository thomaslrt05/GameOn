using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GameOn.DataAccesLayer.Factories;
using GameOn.Models;

namespace GameOn.DataAccesLayer
{
    public class DAL
    {
        
        public static string? ConnectionString { get; set; }
        private UserFactory? _userFact = null;
        private DepartementFactory? _departementFact = null;
        private SudokuFactory? _sudokuFact = null;
        private SudokuParticipationFactory? _sudokuParticipationFact = null;
        private NotificationFactory? _notifFactory = null;

        public NotificationFactory NotifFactory
        {
            get
            {
                if (_notifFactory == null)
                {
                    _notifFactory = new NotificationFactory();
                }

                return _notifFactory;
            }
        }

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

        public SudokuFactory SudokuFactory 
        {
            get
            {
                if (_sudokuFact == null)
                {
                    _sudokuFact = new SudokuFactory();
                }

                return _sudokuFact;
            }
                
        }
        public SudokuParticipationFactory SudokuParticipationFact
        {
            get
            {
                if (_sudokuParticipationFact == null)
                {
                    _sudokuParticipationFact = new SudokuParticipationFactory();
                }

                return _sudokuParticipationFact;
            }
                
        }


    }
}
