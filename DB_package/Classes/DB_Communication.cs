using System;
using System.Collections.Generic;
using Npgsql;
using Microsoft.Data.Analysis;

namespace DB_package {
    public class DB_Communication : IDB_Communication {
        public void insert_to(string insert, string tablename)
        {
            var cs = new Connections().cs_remote;
            var dataSource = NpgsqlDataSource.Create(cs);
            var countrows = 0;
            using (var cmd = dataSource.CreateCommand("select count(text) from " + tablename))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    countrows = reader.GetInt32(0);
                }
            }

            string inside = "insert into " + tablename + " (id,text) values (" + (countrows + 1) + ",'" + insert + "')";
            using (var cmd = dataSource.CreateCommand(inside))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public string get_token(string select) {
            var cs = new Connections().cs_remote;
            var dataSource = NpgsqlDataSource.Create(cs);
            string token = string.Empty;
            using (var cmd = dataSource.CreateCommand(select))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    token = reader.GetString(0);
                }
            }

            return token;
        }

        public Dictionary<int, string> get_data(string select) {
            var cs = new Connections().cs_remote;
            var dataSource = NpgsqlDataSource.Create(cs);
            var command = dataSource.CreateCommand(select);
            var reader = command.ExecuteReader();
            int i = 0;

            Dictionary<int, string> data = new Dictionary<int, string>();

            while (reader.Read())
            {
                data.Add(i, reader.GetString(0));
                i++;
            }
            return data;
        }

        public bool checkcommands(string input_string) {
            var cs = new Connections().cs_remote;
            var conn = new NpgsqlConnection(cs);
            conn.Open();
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT text FROM assistant_sets.commands";
            cmd.ExecuteNonQuery();
            NpgsqlDataReader dr = cmd.ExecuteReader();

            PrimitiveDataFrameColumn<int> idx = new PrimitiveDataFrameColumn<int>("id", 0);
            StringDataFrameColumn text = new StringDataFrameColumn("text", 0);

            int id = 1;
            while (dr.Read())
            {
                idx.Append(id);
                text.Append(dr[0].ToString());
                id++;
            }

            DataFrame df = new DataFrame(idx, text);
            Dictionary<int, string> txt = new Dictionary<int, string>();

            for (int i = 0; i < df["text"].Length; i++)
            {
                txt.Add(i, df["text"][i].ToString());
            }
            foreach (var txtvalue in txt.Values)
            {
                if (input_string.Contains(txtvalue))
                {
                    return true;
                }

            }
            return false;
        }
    }
}