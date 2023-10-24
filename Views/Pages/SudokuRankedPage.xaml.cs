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
using GameOn.Models;
using GameOn.ViewModels;
using GameOn.DataAccesLayer;

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
                DAL dal = new DAL();

                //obtenir la participation au sudoku du jour
                SudokuParticipation? sudokuParticipation = dal.SudokuParticipationFact.GetTodayParticipationOfUser(ConnectionSingleton.UserConnected.Id);
                if(sudokuParticipation == null)
                {
                    //si il n'y a pas de participation, obtenir le sudoku du jour
                    Sudoku? sudokuModel = dal.SudokuFactory.GetGameOfTheDay();

                    // si le sudoku du jour n'existe pas le crée
                    if (sudokuModel == null)
                    {
                        sudokuModel = await Sudoku.CreateSudoku();
                        dal.SudokuFactory.Save(sudokuModel);
                    }
                    //creé le sudokuParaticipation pour ce sudoku
                    sudokuParticipation = new SudokuParticipation()
                    {
                        StartDate = DateTime.Now,
                        Id = 0,
                        SudokuId = sudokuModel.Id,
                        PointWon = 0,
                        UserId = ConnectionSingleton.UserConnected.Id,
                        ActualGrid = sudokuModel.Grid
                    };
                    dal.SudokuParticipationFact.Save(sudokuParticipation);
                }

                //transformer la participation en gameLogic
                SudokuLogic sudokuLogic = SudokuLogic.SudokuParticipationToLogic(sudokuParticipation);

                //ouvrire la fenetre avec les données du gameParticipation
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        TextBox textBox = (TextBox)this.FindName($"text{i}{j}");

                        textBox.Text = int.Parse(sudokuLogic.Grid[i, j].ToString()) != 0 ? sudokuLogic.Grid[i, j].ToString() : "";
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite lors du chargement du Sudoku : " + ex.Message);
            }
        }

       
    }
}
