using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOn.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsSeen { get; set; }
        public int User_id { get; set; }
    
        public Notification(int id, string content, bool isSeen, int user_id)
        {
            Id = id;
            Content = content;
            IsSeen = isSeen;
            User_id = user_id;
        }
    }

}
