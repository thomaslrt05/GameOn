
using GameOn.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameOn.DataAccesLayer.Factories
{
    public class SudokuParticipationFactory
    {
        public static SudokuParticipation createEmpty()
        {
            return new SudokuParticipation
            {
                Id = 0, // Valeurs par défaut ou appropriées pour les autres propriétés
                EndDate = null,
                StartDate = DateTime.Now, // Vous pouvez modifier la valeur par défaut si nécessaire
                PointWon = 0,
                SudokuId = 0,
                UserId = 0,
                ActualGrid = string.Empty
            };
        }

        public static SudokuParticipation CreateFromReader(IDataReader reader)
        {
            SudokuParticipation participation = new SudokuParticipation();

            participation.Id = Convert.ToInt32(reader["Id"]);
            participation.StartDate = Convert.ToDateTime(reader["StartDate"]);
            participation.EndDate = reader["EndDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EndDate"]);
            participation.PointWon = Convert.ToInt32(reader["PointWon"]);
            participation.SudokuId = Convert.ToInt32(reader["Sudoku"]);
            participation.UserId = Convert.ToInt32(reader["User_Id"]);
            participation.ActualGrid = reader["ActualGrid"].ToString();

            return participation;
        }

        public SudokuParticipation? GetTodayParticipationOfUser(int userId)
        {
            DateTime date = DateTime.Today;
            MySqlConnection? mySqlCnn = null;
            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT sp.* " +
                                        "FROM sudokuparticipation sp " +
                                        "INNER JOIN sudoku s ON sp.sudoku = s.id " +
                                        "WHERE sp.user_id = @UserId " +
                                        "AND DATE(sp.startDate) = @Date " +
                                        "AND s.isRanked = 1";
                mySqlCmd.Parameters.AddWithValue("@UserId", userId);
                mySqlCmd.Parameters.AddWithValue("@Date", date.Date);

                using (MySqlDataReader reader = mySqlCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return CreateFromReader(reader);
                    }
                }
            }
            finally
            {
                mySqlCnn?.Close();
            }

            // Si aucune participation n'est trouvée, retournez null ou une valeur par défaut.
            return null;
        }

        public SudokuParticipation? GetLastUnrankedSudokuOfUser(int userId, int difficulty)
        {
            MySqlConnection? mySqlCnn = null;
            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT sp.* " +
                                        "FROM sudokuparticipation sp " +
                                        "INNER JOIN sudoku s ON sp.sudoku = s.id " +
                                        "WHERE sp.user_id = @UserId " +
                                        "AND s.isRanked = 0 " +
                                        "AND s.difficulty = @Difficulty " +
                                        "ORDER BY sp.startDate DESC " +
                                        "LIMIT 1";
                mySqlCmd.Parameters.AddWithValue("@UserId", userId);
                mySqlCmd.Parameters.AddWithValue("@Difficulty", difficulty);

                using (MySqlDataReader reader = mySqlCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return CreateFromReader(reader);
                    }
                }
            }
            finally
            {
                mySqlCnn?.Close();
            }

            // Si aucun sudoku non classé n'est trouvé, retournez null ou une valeur par défaut.
            return null;
        }
        public void Save(SudokuParticipation participation)
        {
            MySqlConnection? mySqlCnn = null;
            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                if (participation.Id == 0)
                {
                    mySqlCmd.CommandText = "INSERT INTO sudokuparticipation(EndDate, StartDate, PointWon, Sudoku, User_Id, ActualGrid) " +
                                           "VALUES (@EndDate, @StartDate, @PointWon, @SudokuId, @UserId, @ActualGrid)";
                }
                else
                {
                    mySqlCmd.CommandText = "UPDATE sudokuparticipation " +
                                           "SET EndDate=@EndDate, StartDate=@StartDate, PointWon=@PointWon, Sudoku=@SudokuId, User_Id=@UserId, ActualGrid=@ActualGrid " +
                                           "WHERE Id=@Id";

                    mySqlCmd.Parameters.AddWithValue("@Id", participation.Id);
                }
                mySqlCmd.Parameters.AddWithValue("@EndDate", participation.EndDate);
                mySqlCmd.Parameters.AddWithValue("@StartDate", participation.StartDate);
                mySqlCmd.Parameters.AddWithValue("@PointWon", participation.PointWon);
                mySqlCmd.Parameters.AddWithValue("@SudokuId", participation.SudokuId);
                mySqlCmd.Parameters.AddWithValue("@UserId", participation.UserId);
                mySqlCmd.Parameters.AddWithValue("@ActualGrid", participation.ActualGrid);

                mySqlCmd.ExecuteNonQuery();

                if (participation.Id == 0)
                {
                    participation.Id = (int)mySqlCmd.LastInsertedId;
                }
            }
            finally
            {
                mySqlCnn?.Close();
            }
        }

        public int GetNbWins(int sudokuId)
        {
            int nbWins = 0;
            MySqlConnection? mySqlCnn = null;
            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT COUNT(*) AS NumberOfWins\r\nFROM sudokuParticipation\r\nWHERE sudoku = @Id\r\n      AND EndDate IS NOT NULL;\r\n";
                mySqlCmd.Parameters.AddWithValue("@Id", sudokuId);

                using (MySqlDataReader reader = mySqlCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        nbWins = reader.GetInt32("NumberOfWins");
                    }
                }
            }
            finally
            {
                mySqlCnn?.Close();
            }
            return nbWins;

        }
    }
}

