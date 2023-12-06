using GameOn.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Windows.Controls.Primitives;

namespace GameOn.DataAccesLayer.Factories
{
    public class UserFactory
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private User CreateFromReader(MySqlDataReader mySqlDataReader)
        {
            int id = (int)mySqlDataReader["Id"];
            string name = mySqlDataReader["Name"].ToString() ?? string.Empty;
            string lastName = mySqlDataReader["LastName"].ToString() ?? string.Empty;
            string mail = mySqlDataReader["Mail"].ToString() ?? string.Empty;
            string password = mySqlDataReader["Password"].ToString() ?? string.Empty;
            int departementId = (int)mySqlDataReader["departement"];
            DAL dal = new DAL();
            Departement? departement = dal.DepartementFact.Get(departementId);
            return new User(id, name, lastName, mail, password, departement);

        }

        public void Save(User user)
        {
            MySqlConnection? mySqlCnn = null;
            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                if (user.Id == 0)
                {

                    mySqlCmd.CommandText = "INSERT INTO user(name, lastName, mail, password, departement) " +
                                           "VALUES (@name, @lastName, @mail, @password, @departement)";
                }
                else
                {
                    mySqlCmd.CommandText = "UPDATE user " +
                                           "SET name=@name, lastName=@lastName, mail=@mail, password=@password, departement=@departement" +
                                           "WHERE Id=@id";

                    mySqlCmd.Parameters.AddWithValue("@Id", user.Id);
                }
                mySqlCmd.Parameters.AddWithValue("@name", user.Name?.Trim());
                mySqlCmd.Parameters.AddWithValue("@lastName", user.LastName?.Trim());
                mySqlCmd.Parameters.AddWithValue("@password", User.Hash(user.Password));
                mySqlCmd.Parameters.AddWithValue("@mail", user.Mail?.Trim());
                mySqlCmd.Parameters.AddWithValue("@departement", user.Departement.Id);


                mySqlCmd.ExecuteNonQuery();

                if (user.Id == 0)
                {
                    user.Id = (int)mySqlCmd.LastInsertedId;
                }
            }
            catch
            {
                string ErrorMessage = "Error 5.1: could not save User";
                Logger.Debug(ErrorMessage);
                MessageBox.Show(ErrorMessage);
            }

            finally
            {
                mySqlCnn?.Close();
            }
        }

        public User? Get(int id)
        {
            User? user = null;
            MySqlConnection? mySqlCnn = null;
            MySqlDataReader? mySqlDataReader = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT * FROM user WHERE id = @id";
                mySqlCmd.Parameters.AddWithValue("@id", id);

                mySqlDataReader = mySqlCmd.ExecuteReader();
                if (mySqlDataReader.Read())
                {
                    user = CreateFromReader(mySqlDataReader);
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return user;
        }

        public User? GetByMail(string mail)
        {
            User? user = null;
            MySqlConnection? mySqlCnn = null;
            MySqlDataReader? mySqlDataReader = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT * FROM user WHERE mail = @mail";
                mySqlCmd.Parameters.AddWithValue("@mail", mail);

                mySqlDataReader = mySqlCmd.ExecuteReader();
                if (mySqlDataReader.Read())
                {
                    user = CreateFromReader(mySqlDataReader);
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return user;
        }

        public void ChangePassword(User user,string newPassword)
        {
            string newHashedPassword = User.Hash(newPassword);
            MySqlConnection? mySqlCnn = null;
            MySqlDataReader? mySqlDataReader = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "UPDATE user SET Password = @Password WHERE id = @id";
                mySqlCmd.Parameters.AddWithValue("@Password", newHashedPassword);
                mySqlCmd.Parameters.AddWithValue("@id", user.Id);

                mySqlDataReader = mySqlCmd.ExecuteReader();
                if (mySqlDataReader.Read())
                {
                    user = CreateFromReader(mySqlDataReader);
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            
        }

        public int GetAllPointOfUser(User user)
        {
            MySqlConnection? mySqlCnn = null;
            MySqlDataReader? mySqlDataReader = null;
            int totalPoints = 0;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT SUM(PointWon) " +
                                        "FROM sudokuparticipation " +
                                        "WHERE User_Id = @Id ";

                mySqlCmd.Parameters.AddWithValue("@id", user.Id);
                mySqlDataReader = mySqlCmd.ExecuteReader();

                if (mySqlDataReader.Read())
                {
                    // Vérifier si la valeur retournée n'est pas DBNull
                    if (!mySqlDataReader.IsDBNull(0))
                    {
                        totalPoints = mySqlDataReader.GetInt32(0);
                    }
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return totalPoints;
        }

        public List<User> GetAllUsersOfDepartement(string departementName)
        {
            List<User> users = new List<User>();
            MySqlConnection mySqlCnn = null;
            MySqlDataReader mySqlDataReader = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT u.* " +
                                       "FROM user u " +
                                       "JOIN departement d ON u.departement = d.Id " +
                                       "WHERE d.name = @departementName";

                mySqlCmd.Parameters.AddWithValue("@departementName", departementName);

                mySqlDataReader = mySqlCmd.ExecuteReader();

                while (mySqlDataReader.Read())
                {
                    users.Add(CreateFromReader(mySqlDataReader));
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return users;
        }

        public int GetAllPointOfUserFiltredByDepartement(User user,string departementName)
        {
            MySqlConnection? mySqlCnn = null;
            MySqlDataReader? mySqlDataReader = null;
            int totalPoints = 0;
            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();
                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT SUM(PointWon)" +
                                       "FROM sudokuparticipation" + 
                                        "WHERE User_Id IN(SELECT @Id FROM user WHERE Departement_Name = @DepartementName)";


                mySqlCmd.Parameters.AddWithValue("@id", user.Id);
                mySqlCmd.Parameters.AddWithValue("@DepartementName", departementName);
                mySqlDataReader = mySqlCmd.ExecuteReader();
                if (mySqlDataReader.Read())
                {
                    totalPoints = mySqlDataReader.GetInt32(0);
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }
            return totalPoints;
        }

        public List<User>? GetAllUser()
        {
            List<User>? users = null;
            MySqlConnection? mySqlCnn = null;
            MySqlDataReader? mySqlDataReader = null;

            try
            {
                users = new List<User>(); // Initialisez la liste ici

                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT * FROM user";

                mySqlDataReader = mySqlCmd.ExecuteReader();
                while (mySqlDataReader.Read())
                {
                    users.Add(CreateFromReader(mySqlDataReader));
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return users;
        }

        public Dictionary<User, Dictionary<string, int>> GetPointsByWeek()
        {
            Dictionary<User, Dictionary<string, int>> pointsByUserAndWeek = new Dictionary<User, Dictionary<string, int>>();

            try
            {
                List<User> users = GetAllUser(); 

                foreach (User user in users)
                {
                    Dictionary<string, int> pointsByWeek = new Dictionary<string, int>();

                    DateTime? userFirstParticipationDate = GetParticipationDate(user.Id);

                  

                    // Boucle pour chaque semaine depuis la première participation de l'utilisateur
                    if (userFirstParticipationDate.HasValue)
                    {
                        // Obtenez la date de début (lundi) et de fin (dimanche) de la semaine de participation de l'utilisateur
                        DateTime startOfWeek = userFirstParticipationDate.Value.Date.AddDays(-(int)userFirstParticipationDate.Value.DayOfWeek + (int)DayOfWeek.Monday);
                        DateTime endOfWeek = startOfWeek.AddDays(6);

                        // Obtenez les points pour la semaine de participation de l'utilisateur
                        int totalPoints = GetAllPointOfUserForWeek(user, startOfWeek, endOfWeek);

                        // Ajoutez les points à la liste
                        pointsByWeek.Add($"{startOfWeek.ToShortDateString()} - {endOfWeek.ToShortDateString()}", totalPoints);
                    }

                    

                    // Ajoutez les points par semaine pour cet utilisateur à la liste principale
                    pointsByUserAndWeek.Add(user, pointsByWeek);
                }
            }
            catch (Exception ex)
            {
                // Gérez les exceptions ici
                Console.WriteLine($"Une erreur est survenue : {ex.Message}");
            }

            return pointsByUserAndWeek;
        }

        public int GetAllPointOfUserForWeek(User user, DateTime startOfWeek, DateTime endOfWeek)
        {
            MySqlConnection? mySqlCnn = null;
            MySqlDataReader? mySqlDataReader = null;
            int totalPoints = 0;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();
                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT SUM(PointWon) " +
                                        "FROM sudokuparticipation " +
                                        "WHERE User_Id = @Id " +
                                        "AND StartDate >= @StartOfWeek AND EndDate <= @EndOfWeek";

                mySqlCmd.Parameters.AddWithValue("@id", user.Id);
                mySqlCmd.Parameters.AddWithValue("@StartOfWeek", startOfWeek);
                mySqlCmd.Parameters.AddWithValue("@EndOfWeek", endOfWeek);

                mySqlDataReader = mySqlCmd.ExecuteReader();
                if (mySqlDataReader.Read())
                {
                    totalPoints = mySqlDataReader.GetInt32(0);
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return totalPoints;
        }


        //------------- Fonctionne pas -----------
        public Dictionary<User, Dictionary<string, int>> GetPointsByWeekAndDepartement(string departementName)
        {
            Dictionary<User, Dictionary<string, int>> pointsByUserAndWeek = new Dictionary<User, Dictionary<string, int>>();

            try
            {
                List<User> users = GetAllUser();

                foreach (User user in users)
                {
                    Dictionary<string, int> pointsByWeek = new Dictionary<string, int>();

                    DateTime? userFirstParticipationDate = GetParticipationDate(user.Id);



                    // Boucle pour chaque semaine depuis la première participation de l'utilisateur
                    if (userFirstParticipationDate.HasValue)
                    {
                        // Obtenez la date de début (lundi) et de fin (dimanche) de la semaine de participation de l'utilisateur
                        DateTime startOfWeek = userFirstParticipationDate.Value.Date.AddDays(-(int)userFirstParticipationDate.Value.DayOfWeek + (int)DayOfWeek.Monday);
                        DateTime endOfWeek = startOfWeek.AddDays(6);

                        // Obtenez les points pour la semaine de participation de l'utilisateur
                        int totalPoints = GetAllPointOfUserFiltredByDepartementAndDate(user,departementName ,startOfWeek, endOfWeek);

                        // Ajoutez les points à la liste
                        pointsByWeek.Add($"{startOfWeek.ToShortDateString()} - {endOfWeek.ToShortDateString()}", totalPoints);
                    }



                    // Ajoutez les points par semaine pour cet utilisateur à la liste principale
                    pointsByUserAndWeek.Add(user, pointsByWeek);
                }
            }
            catch (Exception ex)
            {
                // Gérez les exceptions ici
                Console.WriteLine($"Une erreur est survenue : {ex.Message}");
            }

            return pointsByUserAndWeek;
        }


        public int GetAllPointOfUserFiltredByDepartementAndDate(User user, string departementName, DateTime startOfWeek, DateTime endOfWeek)
        {
            MySqlConnection? mySqlCnn = null;
            MySqlDataReader? mySqlDataReader = null;
            int totalPoints = 0;
            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();
                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT SUM(PointWon)" +
                                       "FROM sudokuparticipation" +
                                        "WHERE User_Id IN(SELECT @Id FROM user WHERE Departement_Name = @DepartementName)" +
                                        "AND StartDate >= @StartOfWeek AND EndDate <= @EndOfWeek";


                mySqlCmd.Parameters.AddWithValue("@id", user.Id);
                mySqlCmd.Parameters.AddWithValue("@DepartementName", departementName);
                mySqlCmd.Parameters.AddWithValue("@StartOfWeek", startOfWeek);
                mySqlCmd.Parameters.AddWithValue("@EndOfWeek", endOfWeek);
                mySqlDataReader = mySqlCmd.ExecuteReader();
                if (mySqlDataReader.Read())
                {
                    totalPoints = mySqlDataReader.GetInt32(0);
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }
            return totalPoints;
        }
        ///////////////////////////////////////////////////////////////////////////

        public DateTime? GetParticipationDate(int userId)
        {
            MySqlConnection? mySqlCnn = null;
            MySqlDataReader? mySqlDataReader = null;
            DateTime? firstParticipationDate = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();
                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT MAX(StartDate) " +
                                        "FROM sudokuparticipation " +
                                        "WHERE User_Id = @Id";

                mySqlCmd.Parameters.AddWithValue("@id", userId);

                mySqlDataReader = mySqlCmd.ExecuteReader();
                if (mySqlDataReader.Read() && !mySqlDataReader.IsDBNull(0))
                {
                    firstParticipationDate = mySqlDataReader.GetDateTime(0);
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return firstParticipationDate;
        }

        



        public UserFactory() { }


    }

}
