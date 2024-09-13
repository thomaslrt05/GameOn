using GameOn.DataAccesLayer;
using GameOn.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameOn.DataAccesLayer.Factories
{
    public class DepartementFactory
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private Departement CreateFromReader(MySqlDataReader mySqlDataReader)
        {
            int id = (int)mySqlDataReader["Id"];
            string name = mySqlDataReader["Name"].ToString() ?? string.Empty;
            return new Departement(id, name);
        }

        public Departement? Get(int id)
        {
            Departement? departement = null;
            MySqlConnection? mySqlCnn = null;
            MySqlDataReader? mySqlDataReader = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT * FROM departement WHERE Id = @Id";
                mySqlCmd.Parameters.AddWithValue("@Id", id);

                mySqlDataReader = mySqlCmd.ExecuteReader();
                if (mySqlDataReader.Read())
                {
                    departement = CreateFromReader(mySqlDataReader);
                }
            }
            catch
            {
                string ErrorMessage = "Error 1.1: could not get all departements";
                Logger.Debug(ErrorMessage);
                MessageBox.Show(ErrorMessage);
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return departement;

        }

        public List<Departement>? GetAll()
        {
            List<Departement>? departements = null;
            MySqlConnection? mySqlCnn = null;
            MySqlDataReader? mySqlDataReader = null;

            try
            {
                departements = new List<Departement>();
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT * FROM departement";

                mySqlDataReader = mySqlCmd.ExecuteReader();
                while (mySqlDataReader.Read())
                {
                    Departement departement = CreateFromReader(mySqlDataReader);
                    departements.Add(departement);
                }
            }
            catch
            {
                string ErrorMessage = "Error 1.2: could not get all departements";
                Logger.Debug(ErrorMessage);
                MessageBox.Show(ErrorMessage);
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return departements;

        }
    }
}
