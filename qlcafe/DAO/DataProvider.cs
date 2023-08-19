﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace qlcafe.DAO
{
    public class DataProvider
    {
        private static DataProvider instance;

        private string connstr = "Data Source=.\\sqlexpress;Initial Catalog=QuanLyQuanCafe;Integrated Security=True";

        public static DataProvider Instance { get => instance == null ? new DataProvider() : instance; private set => instance = value; }


        private DataProvider() { }
        public DataTable executeQuery(string query, object[] parameters = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connstr)) // đóng kết nối sau khi thực hiện khối lệnh dưới
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                if (parameters != null)
                {
                    int i = 0;
                    string[] listParams = query.Split(' ');
                    foreach(string item in listParams)
                    {
                        if (item.Contains('@'))
                        {
                            command.Parameters.AddWithValue(item, parameters[i]);
                            i++;
                        }
                    }
                }
                SqlDataAdapter sda = new SqlDataAdapter(command);
                sda.Fill(dt);
                conn.Close();
            }

            return dt;
        }
        public int executeNonQuery(string query, object[] parameters = null)
        {
            int rows_affected = 0;
            using (SqlConnection conn = new SqlConnection(connstr)) // đóng kết nối sau khi thực hiện khối lệnh dưới
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                if (parameters != null)
                {
                    int i = 0;
                    string[] listParams = query.Split(' ');
                    foreach (string item in listParams)
                    {
                        if (item.Contains('@'))
                        {
                            command.Parameters.AddWithValue(item, parameters[i]);
                            i++;
                        }
                    }
                }

                rows_affected = command.ExecuteNonQuery();
                conn.Close();
            }

            return rows_affected;
        }
        public object executeScalar(string query, object[] parameters = null)
        {
            object result = 0;
            using (SqlConnection conn = new SqlConnection(connstr)) // đóng kết nối sau khi thực hiện khối lệnh dưới
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                if (parameters != null)
                {
                    int i = 0;
                    string[] listParams = query.Split(' ');
                    foreach (string item in listParams)
                    {
                        if (item.Contains('@'))
                        {
                            command.Parameters.AddWithValue(item, parameters[i]);
                            i++;
                        }
                    }
                }

                result = command.ExecuteScalar();
                conn.Close();
            }

            return result;
        }
    }
}
