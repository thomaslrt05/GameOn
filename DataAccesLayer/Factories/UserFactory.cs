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
            

        }
        public UserFactory() { }

    }
}
