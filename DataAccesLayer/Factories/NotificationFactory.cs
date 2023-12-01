﻿using GameOn.Models;
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
                    mySqlCmd.CommandText = "UPDATE notification " +
                                           "SET isSeen=@isSeen, content=@content, user_id=@user_id " +
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

        public List<Notification> GetUnseenNotificationsOfUser(int userId)
        {
            List<Notification> notifications = new List<Notification>();
            MySqlConnection mySqlCnn = null;
            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT * FROM notification WHERE user_id = @userId AND isSeen = 0";
                mySqlCmd.Parameters.AddWithValue("@userId", userId);

                using (MySqlDataReader reader = mySqlCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Notification notification = CreateFromReader(reader);
                        notifications.Add(notification);
                    }
                }
            }
            finally
            {
                mySqlCnn?.Close();
            }

            return notifications;
        }

        public bool HasUnseenNotification(int userId)
        {
            MySqlConnection mySqlCnn = null;
            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT COUNT(*) FROM notification WHERE user_id = @userId AND isSeen = 0";
                mySqlCmd.Parameters.AddWithValue("@userId", userId);

                int count = Convert.ToInt32(mySqlCmd.ExecuteScalar());

                return count > 0;
            }
            finally
            {
                mySqlCnn?.Close();
            }
        }
        public void SetNotifAsSeen(Notification notif)
        {
            notif.IsSeen = true;
            new DAL().NotifFactory.Save(notif);
        }


    }
}
