using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using GameOn.Models.Game;
using GameOn.ViewModels;


namespace GameOn.Views.Pages
{
    /// <summary>
    /// Logique d'interaction pour SudokuRankedPage.xaml
    /// </summary>
    public partial class SudokuRankedPage : Page
    {
        public SudokuRankedPage()
        {
            InitializeComponent();
            this.DataContext = new SudokuRankedPageVM();
            LoadSudokuAsync();
        }

        private async void LoadSudokuAsync()
        {
            try
            {
                SudokuLogic sudoku = await InitSudoku();

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        TextBox textBox = (TextBox)this.FindName($"text{i}{j}");

                        textBox.Text = int.Parse(sudoku.Grid[i, j].ToString()) != 0 ? sudoku.Grid[i, j].ToString() : "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite lors du chargement du Sudoku : " + ex.Message);
            }
        }

        public async Task<SudokuLogic> InitSudoku()
        {
            string sudoku = await GetSudoku();
            return SudokuLogic.JsonToGame(sudoku);
        }

        static async Task<string> GetSudoku()
        {
            string url = "https://sudoku-api.vercel.app/api/dosuku";

            HttpClient client = new HttpClient();
            string content = await client.GetStringAsync(url);

            return content;
        }
    }
}
