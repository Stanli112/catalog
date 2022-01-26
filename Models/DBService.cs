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
            Close();
        }

        public void Close()
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
                    "Image BLOB, " +
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

        #region Sector

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

        #endregion

        #region Categoty
        public bool AddCategoryToDB(Category category)
        {
            if (connection.State == ConnectionState.Open)
            {
                SqliteCommand comm = connection.CreateCommand();
                comm.CommandText = $"INSERT INTO Category (Name, Description) VALUES ('{category.Name}', '{category.Description}')";
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

        public ObservableCollection<Category> GetCategoryFromDB()
        {
            ObservableCollection<Category> result_collection = new ObservableCollection<Category>();
            if (connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("ExecuteReader can only be called when the connection is open.");
            }

            SqliteCommand comm = connection.CreateCommand();
            comm.CommandText = "SELECT * FROM Category";
            using (SqliteDataReader reader = comm.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result_collection.Add(new Category(Convert.ToInt32(reader.GetValue(0)),
                                reader.GetValue(1).ToString(),
                                reader.GetValue(2).ToString()));
                    }
                }
            }
            return result_collection;
        }

        public bool DeleteCategoryFromDB(Category category)
        {
            if (connection.State == ConnectionState.Open)
            {
                SqliteCommand comm = connection.CreateCommand();
                comm.CommandText = $"DELETE FROM Category WHERE ID_category={category.ID_category}";
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

        #endregion

        #region Device

        public bool AddDeviceToDB(Device device)
        {
            if (connection.State == ConnectionState.Open)
            {
                var _category = device.Category == null ? "NULL" : device.Category.ID_category.ToString();
                var _sector = device.Sector == null ? "NULL" : device.Sector.ID_sector.ToString();

                SqliteCommand comm = connection.CreateCommand();
                comm.CommandText = "INSERT INTO Devices (Name, Model, Description, ID_category, ID_sector) VALUES " +
                                    $"('{device.Name}', '{device.Model}', '{device.Description}', {_category }, {_sector})";
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

        public bool UpdateDeviceInDB(Device device)
        {
            if (connection.State == ConnectionState.Open)
            {
                var _category = device.Category.ID_category == 0 ? "NULL" : device.Category.ID_category.ToString();
                var _sector = device.Sector.ID_sector == 0 ? "NULL" : device.Sector.ID_sector.ToString();
                var _img_data = device.Image == null ? new byte[0] : device.Image;

SqliteCommand comm = connection.CreateCommand();
                comm.Parameters.Add(new SqliteParameter("@ImageData", _img_data));
                comm.CommandText = $"UPDATE Devices " +
                    $"SET Name='{device.Name}', Model='{device.Model}', Description='{device.Description}', ID_category={_category}, ID_sector={_sector}, Image=@ImageData  " +
                    $"WHERE ID_device={device.ID_device}";
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

        public ObservableCollection<Device> GetDevicesFromDB()
        {
            ObservableCollection<Device> result_collection = new ObservableCollection<Device>();
            if (connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("ExecuteReader can only be called when the connection is open.");
            }

            SqliteCommand comm = connection.CreateCommand();
            comm.CommandText = "SELECT * FROM Devices";
            using (SqliteDataReader reader = comm.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Category category = new Category();
                        if (reader.GetValue(4) != DBNull.Value)
                        {
                            category = GetCategoryFromDB().FirstOrDefault(t => t.ID_category == Convert.ToInt32(reader.GetValue(4)));
                        }
                        Sector sector = new Sector();
                        if (reader.GetValue(5) != DBNull.Value)
                        {
                            sector = GetSectorFromDB().FirstOrDefault(t => t.ID_sector == Convert.ToInt32(reader.GetValue(5)));
                        }
                        byte[] img = null;
                        if (reader.GetValue(6) != DBNull.Value)
                        {
                            img = (byte[])reader.GetValue(6);
                        }

                        result_collection.Add(new Device(Convert.ToInt32(reader.GetValue(0)),
                                reader.GetValue(1).ToString(),
                                reader.GetValue(2).ToString(),
                                reader.GetValue(3).ToString(),
                                category,
                                sector,
                                img)
                            );
                    }
                }
            }
            return result_collection;
        }

        public bool DeleteDeviceFromDB(Device device)
        {
            if (connection.State == ConnectionState.Open)
            {
                SqliteCommand comm = connection.CreateCommand();
                comm.CommandText = $"DELETE FROM Devices WHERE ID_device={device.ID_device}";
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

        #endregion
    }
}
