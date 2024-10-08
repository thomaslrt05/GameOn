﻿
using GameOn.Models;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameOn.DataAccesLayer.Factories
{
    public class SudokuParticipationFactory
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

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

            catch
            {
                string ErrorMessage = "Error 4.1: could get participation to sudoku";
                Logger.Debug(ErrorMessage);
                MessageBox.Show(ErrorMessage);
            }

            finally
            {
                mySqlCnn?.Close();
            }

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

            catch
            {
                string ErrorMessage = "Error 4.2: could not get participation to unranked game";
                Logger.Debug(ErrorMessage);
                MessageBox.Show(ErrorMessage);
            }

            finally
            {
                mySqlCnn?.Close();
            }

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
            catch
            {
                string ErrorMessage = "Error 4.3: could not save participation to sudoku";
                Logger.Debug(ErrorMessage);
                MessageBox.Show(ErrorMessage);
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
            catch
            {
                string ErrorMessage = "Error 4.4: could get nb wins of sudoku "+sudokuId;
                Logger.Debug(ErrorMessage);
                MessageBox.Show(ErrorMessage);
            }
            finally
            {
                mySqlCnn?.Close();
            }
            return nbWins;

        }

        public int GetPointsOfDepartement(int departementId)
        {
            int totalPoints = 0;
            MySqlConnection mySqlCnn = null;
            MySqlDataReader mySqlDataReader = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT SUM(sp.pointWon) " +
                                       "FROM sudokuParticipation sp " +
                                       "JOIN user u ON sp.user_id = u.Id " +
                                       "WHERE u.departement = @departmentId";

                mySqlCmd.Parameters.AddWithValue("@departmentId", departementId);

                mySqlDataReader = mySqlCmd.ExecuteReader();

                if (mySqlDataReader.Read())
                {
                    object result = mySqlDataReader.GetValue(0);

                    if (result != DBNull.Value)
                    {
                        totalPoints = Convert.ToInt32(result);
                    }
                }
            }

            catch
            {
                string ErrorMessage = "Error 4.5: could not get nb points of departement "+departementId;
                Logger.Debug(ErrorMessage);
                MessageBox.Show(ErrorMessage);
            }

            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return totalPoints;
        }

        public int GetPointsOfDepartementByWeek(int departementId)
        {
            int totalPoints = 0;
            MySqlConnection mySqlCnn = null;
            MySqlDataReader mySqlDataReader = null;
            DateTime today = DateTime.Now;
            DateTime startOfWeek = today.Date.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            DateTime endOfWeek = startOfWeek.AddDays(6);

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT SUM(sp.pointWon) " +
                                       "FROM sudokuParticipation sp " +
                                       "JOIN user u ON sp.user_id = u.Id " +
                                       "WHERE u.departement = @departmentId" +
                                       "AND StartDate >= @StartOfWeek AND EndDate <= @EndOfWeek";

                mySqlCmd.Parameters.AddWithValue("@departmentId", departementId);
                mySqlCmd.Parameters.AddWithValue("@StartOfWeek", startOfWeek);
                mySqlCmd.Parameters.AddWithValue("@EndOfWeek", endOfWeek);

                mySqlDataReader = mySqlCmd.ExecuteReader();

                if (mySqlDataReader.Read())
                {
                    object result = mySqlDataReader.GetValue(0);

                    if (result != DBNull.Value)
                    {
                        totalPoints = Convert.ToInt32(result);
                    }
                }
                return totalPoints;
            }catch(Exception e)
            {
                return 0;
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

           
        }
    }
}

