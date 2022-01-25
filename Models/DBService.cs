using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace catalog.Models
{
    public class DBService
    {
        private SqliteConnection connection { get; }

        public DBService(string db_name)
        {
            string connStr = "Data Source=" + db_name;
            connection = new SqliteConnection(connStr);
            connection.Open();
        }
        ~DBService()
        {
            connection.Close();
        }

        public ConnectionState GetServerState()
        {
            return connection.State;
        }

        public bool CreateTables()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                SqliteCommand comm = connection.CreateCommand();

                // create tables: device, category, sector
                comm.CommandText =
                    // category
                    "CREATE TABLE IF NOT EXISTS Category(" +
                    "ID_category INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, " +
                    "Name TEXT NOT NULL, " +
                    "Description TEXT );" +
                    // sector
                    "CREATE TABLE IF NOT EXISTS Sector(" +
                    "ID_sector INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, " +
                    "Name TEXT NOT NULL, " +
                    "Description TEXT );" +
                    // devices
                    "CREATE TABLE IF NOT EXISTS Devices(" +
                    "ID_device INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, " +
                    "Name TEXT NOT NULL, " +
                    "Model INTEGER NOT NULL," +
                    "Description TEXT," +
                    "ID_category INTEGER, " +
                    "ID_sector INTEGER, " +
                    "FOREIGN KEY (ID_category)  REFERENCES Category (ID_category) ON DELETE SET NULL," +
                    "FOREIGN KEY (ID_sector)  REFERENCES Sector (ID_sector) ON DELETE SET NULL);";
                try
                {
                    comm.ExecuteReader();
                    return true;
                }
                catch (Microsoft.Data.Sqlite.SqliteException exp)
                {
                    LocalDebug.Log(exp.Message);
                    return false;
                }
            }
            else
            {
                throw new InvalidOperationException("ExecuteReader can only be called when the connection is open.");
            }
        }

        public bool AddSectorToDB(Sector sector)
        {
            if (connection.State == ConnectionState.Open)
            {
                SqliteCommand comm = connection.CreateCommand();
                comm.CommandText = $"INSERT INTO Sector (Name, Description) VALUES ('{sector.Name}', '{sector.Description}')";
                try
                {
                    comm.ExecuteNonQuery();
                    return true;
                }
                catch (SqliteException exp)
                {
                    LocalDebug.Log(exp.Message);
                    return false;
                }
            }
            else
            {
                throw new InvalidOperationException("ExecuteReader can only be called when the connection is open.");
            }
        }

        public ObservableCollection<Sector> GetSectorFromDB()
        {
            ObservableCollection<Sector> result_collection = new ObservableCollection<Sector>();
            if (connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("ExecuteReader can only be called when the connection is open.");
            }
            
            SqliteCommand comm = connection.CreateCommand();
            comm.CommandText = "SELECT * FROM Sector";
            using (SqliteDataReader reader = comm.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result_collection.Add(new Sector(Convert.ToInt32(reader.GetValue(0)),
                                reader.GetValue(1).ToString(),
                                reader.GetValue(2).ToString()));
                    }
                }
            }
            return result_collection;
        }

        public bool DeleteSectorFromDB(Sector sector)
        {
            if (connection.State == ConnectionState.Open)
            {
                SqliteCommand comm = connection.CreateCommand();
                comm.CommandText = $"DELETE FROM Sector WHERE ID_sector={sector.ID_sector}";
                try
                {
                    comm.ExecuteNonQuery();
                    return true;
                }
                catch (SqliteException exp)
                {
                    LocalDebug.Log(exp.Message);
                    return false;
                }
            }
            else
            {
                throw new InvalidOperationException("ExecuteReader can only be called when the connection is open.");
            }
        }
    }
}
