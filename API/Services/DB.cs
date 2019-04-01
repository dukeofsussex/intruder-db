// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Services
{
    using System;
    using System.Collections.Generic;
    using MySql.Data.MySqlClient;

    public class DB
    {
        private const int ItemsPerPage = 25;

        public DB(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private static string ConnectionString { get; set; }

        public static void GetOrderingStatement(string column, int orderDirection, Dictionary<string, string> validColumns, ref string query)
        {
            string order = orderDirection == 0 ? "ASC" : "DESC";
            query += $" ORDER BY {validColumns[column]} {order}";
        }

        public static T Get<T>(string query, Dictionary<string, dynamic> parameters, Func<MySqlDataReader, T> createItem)
            where T : new()
        {
            T item = new T();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query + " LIMIT 1", conn);
                AssignQueryParameters(ref cmd, parameters);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return default(T);
                    }

                    while (reader.Read())
                    {
                        item = createItem(reader);
                    }
                }
            }

            return item;
        }

        public static List<T> GetList<T>(string query, Dictionary<string, dynamic> parameters, Func<MySqlDataReader, T> createItem)
            where T : new()
        {
            List<T> list = new List<T>();
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                AssignQueryParameters(ref cmd, parameters);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return list;
                    }

                    while (reader.Read())
                    {
                        list.Add(createItem(reader));
                    }
                }
            }

            return list;
        }

        public static Dictionary<string, dynamic> GetPage<T>(string query, Dictionary<string, dynamic> parameters, int page, string countLimitingQuery, string tableName, Func<MySqlDataReader, T> createItem)
            where T : new()
        {
            query += $" LIMIT {ItemsPerPage} OFFSET {(page - 1) * ItemsPerPage}";

            List<T> items = GetList(query, parameters, createItem);
            int count = Get($"SELECT COUNT(*) AS count FROM {tableName} {countLimitingQuery}", parameters, CreateCount);
            int totalPages = (count == 0) ? 1 : (int)Math.Ceiling(count / (double)ItemsPerPage);

            return new Dictionary<string, dynamic>
            {
                { "page", page },
                { "totalPages", totalPages },
                { "data", items }
            };
        }

        public static int Insert(string query, Dictionary<string, dynamic> parameters, bool returnLastInsertId = false)
        {
            return CUDOperation(query, parameters, returnLastInsertId);
        }

        public static int Update(string query, Dictionary<string, dynamic> parameters)
        {
            return CUDOperation(query, parameters);
        }

        public static int Delete(string query, Dictionary<string, dynamic> parameters)
        {
            return CUDOperation(query, parameters);
        }

        private static void AssignQueryParameters(ref MySqlCommand query, Dictionary<string, dynamic> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return;
            }

            foreach (KeyValuePair<string, dynamic> pair in parameters)
            {
                query.Parameters.Add(new MySqlParameter(pair.Key, pair.Value));
            }
        }

        private static int CUDOperation(string query, Dictionary<string, dynamic> parameters, bool returnLastInsertId = false)
        {
            int insertedID = 0;
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                AssignQueryParameters(ref cmd, parameters);
                insertedID = cmd.ExecuteNonQuery();

                if (returnLastInsertId)
                {
                    insertedID = (int)cmd.LastInsertedId;
                }
            }

            return insertedID;
        }

        private static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        private static int CreateCount(MySqlDataReader reader)
        {
            return reader.GetInt32("count");
        }
    }
}