using System;
using System.Data.SQLite;

namespace Bot
{
    class DataBaseInteract
    {
        SQLiteConnection connection;
        SQLiteDataReader reader;
        public DataBaseInteract(string path)
        {
            try
            {
                connection = new SQLiteConnection(@"Data Source =" + path);
            }
            catch { Console.WriteLine("Error"); }
            connection.Open();
        }
        public DataBaseInteract():this("botUsers.db") { }

        public void Insert(string insert)
        {
            
            SQLiteCommand command = new SQLiteCommand(insert, connection);

            try { command.ExecuteNonQuery();
            Console.WriteLine("user was added to database");
            }
            catch {}

               
        }

        public void Update(string update)
        {
            
            SQLiteCommand command = new SQLiteCommand(update, connection);
            command.ExecuteNonQuery();
            Console.WriteLine("user status updated");
        }

        public SQLiteDataReader Select(string select)
        {
            SQLiteCommand command = new SQLiteCommand(select, connection);
            reader = command.ExecuteReader();
            return reader;
        }

    }
}
