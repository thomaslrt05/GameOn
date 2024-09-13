using GameOn.Views.Pages;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using GameOn.DataAccesLayer;
using GameOn.Models;

namespace GameOn.ViewModels
{
    internal class SudokuMainPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string nomPropriete)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
            }
        }

        public static MainWindowVM Instance = MainWindowVM.Instance;

        public SudokuMainPageVM()
        {
            this.ButtonPracticeGameCommand = new RelayCommand(ExecuteButtonPracticeGame, CanExecuteButton);
            this.ButtonRankedGameCommand = new RelayCommand(ExecuteButtonRankedGame, CanExecuteButton);
            this.ButtonScoreboardCommand = new RelayCommand(ExecuteButtonScoreboard, CanExecuteButton);
            
        }

       
        public ICommand ButtonPracticeGameCommand { get; private set; }

        private bool CanExecuteButton(object parameter)
        {
            return true;
        }

        private void ExecuteButtonPracticeGame(object parameter)
        {
            MainWindowVM mainWindowVM = MainWindowVM.Instance;
            mainWindowVM.CurrentPage = new SudokuPracticePage();
        }

        public ICommand ButtonRankedGameCommand { get; private set; }

        private void ExecuteButtonRankedGame(object parameter)
        {
            var sudokuParticipation = new DAL().SudokuParticipationFact.GetTodayParticipationOfUser(ConnectionSingleton.UserConnected.Id);
            MainWindowVM mainWindowVM = MainWindowVM.Instance;
            if(sudokuParticipation is null || sudokuParticipation.EndDate is null)
            {
                mainWindowVM.CurrentPage = new SudokuRankedPage();
            }
            else
            {
                MessageBox.Show("Vous avez déja fini le sudoku du jour");
            }
        }

      

        public ICommand ButtonScoreboardCommand { get; private set; }

        private void ExecuteButtonScoreboard(object parameter)
        {
            MainWindowVM mainWindowVM = MainWindowVM.Instance;
            mainWindowVM.CurrentPage = new SudokuScoreboardPage();
        }

        


    }
}
