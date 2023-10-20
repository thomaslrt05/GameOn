using GameOn.Models;
using System;

namespace GameOn.DataAccesLayer
{
    public static class ConnectionSingleton
    {
        private static User? _user;

        public static User UserConnected
        {
            get
            {
                return _user;
            }
            set { _user = value; }
        }

    }
}