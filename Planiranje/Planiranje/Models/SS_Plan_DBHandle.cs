﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using Planiranje.Controllers;
using PagedList;

namespace Planiranje.Models
{
    public class SS_Plan_DBHandle
    {
        private MySqlConnection connection;

        private void Connect()
        {
            string connection_string = ConfigurationManager.ConnectionStrings["BazaPodataka"].ConnectionString;
            connection = new MySqlConnection(connection_string);
        }

        public List<SS_Plan> ReadSSPlanove()
        {
            List<SS_Plan> Ss_planovi = new List<SS_Plan>();
            this.Connect();
            using (MySqlCommand command = new MySqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT id_plan, ak_godina, naziv, opis " +
                    "FROM Ss_plan " +
                    "WHERE id_pedagog = @id_pedagog " +
                    "ORDER BY id_plan ASC";
                command.Parameters.AddWithValue("@id_pedagog", PlaniranjeSession.Trenutni.PedagogId);
                connection.Open();
                using (MySqlDataReader sdr = command.ExecuteReader())
                {
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            SS_Plan plan = new SS_Plan()
                            {
                                Id_plan = Convert.ToInt32(sdr["id_plan"]),
                                Naziv = sdr["naziv"].ToString(),
                                Ak_godina = sdr["ak_godina"].ToString(),
                                Opis = sdr["opis"].ToString(),
                            };
                            Ss_planovi.Add(plan);
                        }
                    }
                }
                connection.Close();
            }
            return Ss_planovi;
        }

        public List<SS_Plan> ReadSSPlanove(string search_string)
        {
            List<SS_Plan>Ss_planovi = new List<SS_Plan>();
            this.Connect();
            using (MySqlCommand command = new MySqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT id_plan, ak_godina, naziv, opis " +
                    "FROM Ss_plan " +
                    "WHERE id_pedagog = @id_pedagog " +
                    "AND (ak_godina like '%" + search_string + "%' " +
                    "OR naziv like '%" + search_string + "%' " +
                    "OR opis like '%" + search_string + "%') " +
                    "ORDER BY id_plan ASC";
                command.Parameters.AddWithValue("@id_pedagog", PlaniranjeSession.Trenutni.PedagogId);
                connection.Open();
                using (MySqlDataReader sdr = command.ExecuteReader())
                {
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                            SS_Plan plan = new SS_Plan()
                            {
                                Id_plan = Convert.ToInt32(sdr["id_plan"]),
                                Naziv = sdr["naziv"].ToString(),
                                Ak_godina = sdr["ak_godina"].ToString(),
                                Opis = sdr["opis"].ToString(),
                            };
                            Ss_planovi.Add(plan);
                        }
                    }
                }
                connection.Close();
            }
            return Ss_planovi;
        }

        public SS_Plan ReadSSPlan(int _id)
        {
            SS_Plan Ss_plan = new SS_Plan();
            this.Connect();
            using (MySqlCommand command = new MySqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT id_plan, ak_godina, naziv, opis " +
                    "FROM Ss_plan " +
                    "WHERE id_plan = @id " +
                    "AND id_pedagog = @id_pedagog";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@id", _id);
                command.Parameters.AddWithValue("@id_pedagog", PlaniranjeSession.Trenutni.PedagogId);
                connection.Open();
                using (MySqlDataReader sdr = command.ExecuteReader())
                {
                    if (sdr.HasRows)
                    {
                        while (sdr.Read())
                        {
                           Ss_plan = new SS_Plan()
                            {
                                Id_plan = Convert.ToInt32(sdr["id_plan"]),
                                Ak_godina = sdr["ak_godina"].ToString(),
                                Naziv = sdr["naziv"].ToString(),
                                Opis = sdr["opis"].ToString()
                            };
                        }
                    }
                }
                connection.Close();
            }
            return Ss_plan;
        }

        public bool CreateSSPlan(SS_Plan Ss_plan)
        {
            try
            {
                this.Connect();
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO Ss_plan " +
                        "(id_pedagog, ak_godina, naziv, opis) " +
                        " VALUES (@id_pedagog, @ak_godina, @naziv, @opis)";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id_pedagog", PlaniranjeSession.Trenutni.PedagogId);
                    command.Parameters.AddWithValue("@ak_godina", Ss_plan.Ak_godina);
                    command.Parameters.AddWithValue("@naziv", Ss_plan.Naziv);
                    command.Parameters.AddWithValue("@opis",Ss_plan.Opis);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                connection.Close();
                return false;
            }
            finally
            {
                connection.Close();
            }
            return true;
        }

        public bool UpdateSSPlan(SS_Plan Ss_plan)
        {
            try
            {
                this.Connect();
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "UPDATE Ss_plan " +
                        "SET " +
                        "ak_godina = @ak_godina, " +
                        "naziv = @naziv, " +
                        "opis = @opis " +
                        "WHERE id_plan = @id_plan " +
                        "AND id_pedagog = @id_pedagog";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id_plan", Ss_plan.Id_plan);
                    command.Parameters.AddWithValue("@id_pedagog", PlaniranjeSession.Trenutni.PedagogId);
                    command.Parameters.AddWithValue("@ak_godina", Ss_plan.Ak_godina);
                    command.Parameters.AddWithValue("@naziv", Ss_plan.Naziv);
                    command.Parameters.AddWithValue("@opis", Ss_plan.Opis);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                connection.Close();
                return false;
            }
            finally
            {
                connection.Close();
            }
            return true;
        }

        public bool DeleteSSPlan(int id)
        {
            try
            {
                this.Connect();
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    connection.Open();
                    command.CommandText = "DELETE FROM Ss_plan " +
                        "WHERE id_plan = @id_plan " +
                        "AND id_pedagog = @id_pedagog";
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id_plan", id);
                    command.Parameters.AddWithValue("@id_pedagog", PlaniranjeSession.Trenutni.PedagogId);
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                connection.Close();
                return false;
            }
            finally
            {
                connection.Close();
            }
            return true;
        }
    }
}



