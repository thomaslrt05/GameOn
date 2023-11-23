using GameOn.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOn.DataAccesLayer.Factories
{
    public class NotificationFactory
    {
        public Notification CreateFromReader(MySqlDataReader mySqlDataReader)
        {
            int id = (int)mySqlDataReader["Id"];
            int user_id = (int)mySqlDataReader["user_id"];
            bool isSeen = mySqlDataReader.GetBoolean("isSeen");
            string content = mySqlDataReader.GetString("content");
            return new Notification(id, content, isSeen,user_id);
        }

        public void Save(Notification notif)
        {
            MySqlConnection? mySqlCnn = null;
            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                if (notif.Id == 0)
                {

                    mySqlCmd.CommandText = "INSERT INTO notification(isSeen,content, user_id) " +
                                           "VALUES (@isSeen, @content, @user_id)";
                }
                else
                {
                    mySqlCmd.CommandText = "UPDATE sudoku " +
                                           "SET isSeen=@isSeen, content=@content, user_id=@user_id" +
                                           "WHERE Id=@id";

                    mySqlCmd.Parameters.AddWithValue("@Id", notif.Id);
                }
                mySqlCmd.Parameters.AddWithValue("@isSeen", notif.IsSeen);
                mySqlCmd.Parameters.AddWithValue("@content", notif.Content.Trim());
                mySqlCmd.Parameters.AddWithValue("@user_id", notif.User_id);



                mySqlCmd.ExecuteNonQuery();

                if (notif.Id == 0)
                {
                    notif.Id = (int)mySqlCmd.LastInsertedId;
                }
            }
            finally
            {
                mySqlCnn?.Close();
            }
        }

        public List<Notification> GetNotificationsOfUser(int userid) {
            throw new NotImplementedException();
        }
        public bool HasUnseenNotification(int userid) { 
            throw new NotImplementedException();
        }

    }
}
