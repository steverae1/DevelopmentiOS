﻿using System;
using System.Data.SQLite;
using CardGame.GameElements;

namespace CardGame.Server {
    class Database {
        private static Database instance;
        public static Database getInstance() {
            if (instance == null) {
                instance = new Database();
            }
            return instance;
        }

        private string dataSource;

        private Database() {
            this.dataSource = "Data Source=C:\\cygwin\\home\\Taylor\\pubnub\\DevelopmentiOS\\CardGame\\server\\CardGameServer\\database.sqlite";
            Console.WriteLine("Database created");
        }

        private int execNonQuery(string sql) {
            SQLiteConnection conn = new SQLiteConnection(this.dataSource);
            SQLiteCommand cmd = new SQLiteCommand(conn);
            int rowsAffected = 0;
            try {
                cmd.CommandText = sql;
                cmd.Prepare();
                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e) {
                Console.WriteLine("SQLite exception: {0}", e);
            }
            finally {
                conn.Close();
            }

            return rowsAffected;
        }

        private string execScalar(string sql) {
            SQLiteConnection conn = new SQLiteConnection(this.dataSource);
            SQLiteCommand cmd = new SQLiteCommand(conn);
            object result = null;
            try {
                cmd.CommandText = sql;
                cmd.Prepare();
                conn.Open();
                result = cmd.ExecuteScalar();
            }
            catch (SQLiteException e) {
                Console.WriteLine("SQLite exception: {0}", e);
            }
            finally {
                conn.Close();
            }

            if (result != null) {
                return result.ToString();
            }

            return "";
        }

        public int clear() {
            return this.execNonQuery("DELETE FROM users;");
        }

        public Player addUser(string username, string uuid) {
            return (this.execNonQuery(String.Format("INSERT INTO users(name, uuid, wins, funds) VALUES ('{0}', '{1}', 0, 100);", username, uuid)) == 1)
                ? new Player(username, uuid) : null;
        }

        public bool userExists(string username) {
            return "" != this.execScalar(String.Format("SELECT name FROM users WHERE name='{0}';", username));
        }

        public bool authenticateUser(string username, string uuid) {
            return uuid == this.execScalar(String.Format("SELECT uuid FROM users WHERE name='{0}';", username));
        }

        public int loadFunds(string uuid) {
            return int.Parse(this.execScalar(String.Format("SELECT funds FROM users WHERE uuid='{0}';", uuid)));
        }

        public int loadWins(string uuid) {
            return int.Parse(this.execScalar(String.Format("SELECT wins FROM users WHERE uuid='{0}';", uuid)));
        }

        public bool savePlayer(Player p) {
            return (this.execNonQuery(String.Format("UPDATE users SET funds={0}, wins={1} WHERE uuid='{2}';", p.Funds, p.Wins, p.Uuid)) == 1);
        }

    }
}