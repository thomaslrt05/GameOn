using GameOnUnlimited.DataAccesLayer;
using GameOnUnlimited.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameOn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DAL.ConnectionString = "Server=sql.decinfo-cchic.ca;Port=33306;Database=a23_e80_hourglass_gameon;Uid=dev-2334653;Pwd=Sayer123";
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            string pwd = txtPassword.Text;
            string mail = Email.Text;
            string hash = User.Hash(pwd);
            User user = new DAL().UserFact.GetByMail(mail);
            if (user != null)
            {
                MessageBox.Show(user.Password == hash ? "logged in":"error");
            }
            MessageBox.Show("error");
        }
    }
}
