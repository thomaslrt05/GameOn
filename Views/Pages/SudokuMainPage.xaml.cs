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
using GameOn.DataAccesLayer;
using GameOn.Models;
using GameOn.ViewModels;

namespace GameOn.Views.Pages
{
    /// <summary>
    /// Logique d'interaction pour SodukuMainPage.xaml
    /// </summary>
    public partial class SodukuMainPage : Page
    {
        public SodukuMainPage()
        {
            InitializeComponent();
            this.DataContext = new SudokuMainPageVM();
        }


    }
}
