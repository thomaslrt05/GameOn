using GameOn.DataAccesLayer;
using GameOn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; 


namespace GameOn.ViewModels
{
    internal class ProfilWindowVM
    {
            public ProfilWindowVM()
            {
                LoadDepartement();
                User userConntected = new DAL().userConnected;
                Console.WriteLine(userConntected);
            }


        private void LoadDepartement()
        {

            List<string> list = new List<string>();
            DAL dAL = new DAL();
            List<Departement> departs = new List<Departement>();
            departs = dAL.DepartementFact.GetAll();

            foreach (Departement nameDepartement in departs)
            {
                list.Add(nameDepartement.Name);
            }
        }
    }
}
