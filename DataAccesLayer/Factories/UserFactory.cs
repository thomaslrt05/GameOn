using GameOnUnlimited.Models;
using MySql.Data.MySqlClient;

namespace GameOnUnlimited.DataAccesLayer.Factories
{
    public class UserFactory
    {
        private User CreateFromReader(MySqlDataReader mySqlDataReader)
        {
            int id = (int)mySqlDataReader["Id"];
            string name = mySqlDataReader["Name"].ToString() ?? string.Empty;
            string lastName = mySqlDataReader["LastName"].ToString() ?? string.Empty;
            string mail = mySqlDataReader["Mail"].ToString() ?? string.Empty;
            string password = mySqlDataReader["Password"].ToString() ?? string.Empty;
            int departementId = (int)mySqlDataReader["Id"];
            DAL dal = new DAL();
            Departement? departement = dal.DepartementFact.Get(departementId);
            return new User(id, name, lastName, mail, password, departement);

        }
        public User CreateEmpty()
        {
            return new User(0, string.Empty, string.Empty, string.Empty, string.Empty, null);
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
        public UserFactory() { }


    }

}
