using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameOn.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public Departement Departement { get; set; }
        public List<SudokuParticipation>? Participations { get; set; }
        public List<Notification>? Notifications { get; set; }
        public User() { }

        public User(int id, string name, string lastName, string mail, string password, Departement departement)
        {
            Id = id;
            Name = name;
            LastName = lastName;
            Mail = mail;
            Password = password;
            Departement = departement;
        }
        public static string Hash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return hash;
            }
        }
    }
}
