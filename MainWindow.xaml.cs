using catalog.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace catalog
{

    public partial class MainWindow : Window
    {
        const string file_db_name = "catalog.db";
        public SqliteConnection connection;

        public List<Device> g_devices;
        public List<Category> g_category;
        public List<Sector> g_sector;


        public MainWindow()
        {
            InitializeComponent();
            InitLists();
            InitDataBase();
        }

        void InitLists()
        {
            g_devices = new List<Device>(); 
            g_category = new List<Category>(); 
            g_sector = new List<Sector>(); 
        }

        void InitDataBase()
        {
            string connStr = "Data Source=" + file_db_name;            
            using (connection = new SqliteConnection(connStr))
            {
                connection.Open();
                SqliteCommand comm = connection.CreateCommand();
                
                // create tables: device, category, sector
                comm.CommandText = 
                    // category
                    "CREATE TABLE IF NOT EXISTS Category(" +
                    "ID_category INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, " +
                    "Name TEXT NOT NULL, " +
                    "Description TEXT NOT NULL);" +
                    // sector
                    "CREATE TABLE IF NOT EXISTS Sector(" +
                    "ID_sector INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, " +
                    "Name TEXT NOT NULL, " +
                    "Description TEXT NOT NULL);" +
                    // devices
                    "CREATE TABLE IF NOT EXISTS Devices(" +
                    "ID_device INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, " +
                    "Name TEXT NOT NULL, " +
                    "Model INTEGER NOT NULL," +
                    "Description TEXT NOT NULL," +
                    "ID_category INTEGER, " +
                    "ID_sector INTEGER ," +
                    "FOREIGN KEY (ID_category)  REFERENCES Category (ID_category) ON DELETE SET NULL," +
                    "FOREIGN KEY (ID_sector)  REFERENCES Sector (ID_sector) ON DELETE SET NULL);";

                try
                {
                    comm.ExecuteReader();
                    //AddTestValues(conn);
                    GetDataFromDB(connection);

                }
                catch (Microsoft.Data.Sqlite.SqliteException exp)
                {
                    LocalDebug.Log(exp.Message);
                }                
            }
        }

        #region Get Data From DataBase
        void GetDataFromDB(SqliteConnection conn)
        {
            GetCategoryFromDB(conn);
            GetSectorFromDB(conn);
            GetDevicesFromDB(conn);
        }

        void GetCategoryFromDB(SqliteConnection conn)
        {
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = "SELECT * FROM Category";
            using (SqliteDataReader reader = comm.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        g_category.Add(new Category(Convert.ToInt32(reader.GetValue(0)),
                                reader.GetValue(1).ToString(),
                                reader.GetValue(2).ToString()));
                    }
                }
            }
        }

        void GetSectorFromDB(SqliteConnection conn)
        {
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = "SELECT * FROM Sector";
            using (SqliteDataReader reader = comm.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        g_sector.Add(new Sector(Convert.ToInt32(reader.GetValue(0)),
                                reader.GetValue(1).ToString(),
                                reader.GetValue(2).ToString()));
                    }
                }
            }
        }

        void GetDevicesFromDB(SqliteConnection conn)
        {
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = "SELECT * FROM Devices";
            using (SqliteDataReader reader = comm.ExecuteReader())
            {
                if (reader.HasRows) 
                {
                    while (reader.Read())  
                    {
                        g_devices.Add( new Device( Convert.ToInt32(reader.GetValue(0)), 
                                reader.GetValue(1).ToString(),
                                reader.GetValue(2).ToString(),
                                reader.GetValue(3).ToString(),
                                reader.GetValue(4),
                                reader.GetValue(5))
                            );
                    }
                }
            }
        }
        #endregion

        #region Add Data To DataBase

        void AddCategoryToDB(SqliteConnection conn, Category category)
        {
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = $"INSERT INTO Category (Name, Description) VALUES ('{category.Name}', '{category.Description}')";
            comm.ExecuteNonQuery();
        }

        void AddSectorToDB(SqliteConnection conn, Sector sector)
        {
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = $"INSERT INTO Sector (Name, Description) VALUES ('{sector.Name}', '{sector.Description}')";
            comm.ExecuteNonQuery();
        }

        void AddDeviceToDB(SqliteConnection conn, Device device)
        {
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = "INSERT INTO Devices (Name, Model, Description, ID_category, ID_sector) VALUES " +
                $"('{device.Name}', '{device.Model}', '{device.Description}', {device.ID_category}, {device.ID_sector})";
            comm.ExecuteNonQuery();
        }

        void DeleteCategoryFromDB(SqliteConnection conn, Category category)
        {
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = $"DELETE FROM Category WHERE ID_category={category.ID_category}";
            comm.ExecuteNonQuery();
        }

        void DeleteSectorFromDB(SqliteConnection conn, Sector sector)
        {
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = $"DELETE FROM Sector WHERE ID_sector={sector.ID_sector}";
            comm.ExecuteNonQuery();
        }

        void DeleteDeviceFromDB(SqliteConnection conn, Device device)
        {
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = $"DELETE FROM Devices WHERE ID_device={device.ID_device}";
            comm.ExecuteNonQuery();
        }

        void UpdateDeviceFromDB(SqliteConnection conn, Device device)
        {
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = $"UPDATE Devices " +
                $"SET Name='{device.Name}', Model='{device.Model}', Description='{device.Description}', ID_category={device.ID_category}, ID_sector={device.ID_sector} " +
                $"WHERE ID_device={device.ID_device}";
            comm.ExecuteNonQuery();
        }

        #endregion

        #region Test Values
        void AddTestValues(SqliteConnection conn)
        {
            AddValuesInCategory(conn);
            AddValuesInSector(conn);
            AddValuesInDevice(conn);
        }

        void AddValuesInCategory(SqliteConnection conn)
        {
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = "INSERT INTO Category (Name, Description) VALUES " +
                "('Category_1', 'Description_1')," +
                "('Category_2', 'Description_2')," +
                "('Category_3', 'Description_3')," +
                "('Category_4', 'Description_4')";
            comm.ExecuteNonQuery();
        }

        void AddValuesInSector(SqliteConnection conn)
        {
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = "INSERT INTO Sector (Name, Description) VALUES " +
                "('Sector_1', 'Description_1')," +
                "('Sector_2', 'Description_2')," +
                "('Sector_3', 'Description_3')," +
                "('Sector_4', 'Description_4')";
            comm.ExecuteNonQuery();
        }

        void AddValuesInDevice(SqliteConnection conn)
        {
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = "INSERT INTO Devices (Name, Model, Description, ID_category, ID_sector) VALUES " +
                "('Device_1', 'Model_1', 'Description_1', 1, 1)," +
                "('Device_2', 'Model_1', 'Description_2', 1, 2)," +
                "('Device_3', 'Model_1', 'Description_3', 2, 3)," +
                "('Device_4', 'Model_2', 'Description_4', 2, 1)," +
                "('Device_5', 'Model_2', 'Description_5', 3, 2)," +
                "('Device_6', 'Model_3', 'Description_6', 3, 1)," +
                "('Device_7', 'Model_3', 'Description_7', 2, 2)," +
                "('Device_8', 'Model_4', 'Description_8', 4, 4)," +
                "('Device_9', 'Model_5', 'Description_9', 4, 3)," +
                "('Device_10', 'Model_5', 'Description_10', 1, 4)";
            comm.ExecuteNonQuery();
        }
        #endregion



        private void btnAddDevice_Click(object sender, RoutedEventArgs e)
        {
            lblTopPage.Content = "Добавить оборудование";
            Grid_AllDevice.Visibility = Visibility.Hidden;
            btnAdd_Device.Visibility = Visibility.Visible;
            btnUpdate_Device.Visibility = Visibility.Hidden;

            Grid_AddAndUpdateDevice.Visibility = Visibility.Visible;
        }

        private void btnUpdateDevice_Click(object sender, RoutedEventArgs e)
        {
            if(dgAll_Devices.SelectedItem != null)
            {
                lblTopPage.Content = "Редактировать оборудование";
                Grid_AllDevice.Visibility = Visibility.Hidden;
                btnAdd_Device.Visibility = Visibility.Hidden;
                btnUpdate_Device.Visibility = Visibility.Visible;

                Grid_AddAndUpdateDevice.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Выберите оборудование");
            }
            
        }

        private void btnDeleteDevice_Click(object sender, RoutedEventArgs e)
        {
            if (dgAll_Devices.SelectedItem != null)
            {
                
            }
            else
            {
                MessageBox.Show("Выберите оборудование");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Grid_AddAndUpdateDevice.Visibility = Visibility.Hidden;
            Grid_AllDevice.Visibility = Visibility.Visible;
        }

        private void btnAdd_Device_Click(object sender, RoutedEventArgs e)
        {
            // 
        }
    }
}
