using GameOn.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOn.DataAccesLayer.Factories
{
    public class SudokuFactory
    {
        public Sudoku CreateFromReader(MySqlDataReader mySqlDataReader)
        {
            int id = (int)mySqlDataReader["Id"];
            DateTime creationDate = mySqlDataReader.GetDateTime("CreationDate");
            string grid = mySqlDataReader.GetString("Grid"); 
            string solutionGrid = mySqlDataReader.GetString("SolutionGrid");
            bool isRanked = mySqlDataReader.GetBoolean("IsRanked");
            int difficulty = (int)mySqlDataReader["difficulty"];
            return new Sudoku(id, creationDate, grid, solutionGrid, isRanked, difficulty);
        }

        public void Save(Sudoku sudoku)
        {
            MySqlConnection? mySqlCnn = null;
            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                if (sudoku.Id == 0)
                {

                    mySqlCmd.CommandText = "INSERT INTO user(creationDate, grid, solutionGrid, isRanked, difficulty) " +
                                           "VALUES (@creationDate, @grid, @solutionGrid, @isRanked, @difficulty)";
                }
                else
                {
                    mySqlCmd.CommandText = "UPDATE user " +
                                           "SET creationDate=@creationDate, grid=@grid, solutionGrid=@solutionGrid, isRanked=@isRanked, difficulty=@difficulty" +
                                           "WHERE Id=@id";

                    mySqlCmd.Parameters.AddWithValue("@Id", sudoku.Id);
                }
                mySqlCmd.Parameters.AddWithValue("@creationDate", sudoku.CreationDate);
                mySqlCmd.Parameters.AddWithValue("@grid", sudoku.Grid.Trim());
                mySqlCmd.Parameters.AddWithValue("@solutionGrid", sudoku.SolutionGrid.Trim());
                mySqlCmd.Parameters.AddWithValue("@isRanked", sudoku.IsRanked);
                mySqlCmd.Parameters.AddWithValue("@difficulty", sudoku.Difficulty);


                mySqlCmd.ExecuteNonQuery();

                if (sudoku.Id == 0)
                {
                    sudoku.Id = (int)mySqlCmd.LastInsertedId;
                }
            }
            finally
            {
                mySqlCnn?.Close();
            }
        }
        public Sudoku GetGameOfTheDay()
        {
            MySqlConnection? mySqlCnn = null;
            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                DateTime today = DateTime.Today;
                string query = "SELECT * FROM game WHERE DATE(CreationDate) = @today";
                mySqlCmd.CommandText = query;
                mySqlCmd.Parameters.AddWithValue("@today", today);

                using (MySqlDataReader reader = mySqlCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return CreateFromReader(reader);
                    }
                }

                return null; 
            }
            finally
            {
                mySqlCnn?.Close();
            }
        }
    }
}
