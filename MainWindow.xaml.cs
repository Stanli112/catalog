using catalog.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        const string file_db_name = "catalog.db";
        public SqliteConnection connection;

        private ObservableCollection<Device> g_devices;
        private ObservableCollection<Category> g_category;
        private ObservableCollection<Sector> g_sector;

        public ObservableCollection<Device> G_devices { get => g_devices; set { g_devices = value; OnPropertyChanged(); } }
        public ObservableCollection<Category> G_category { get => g_category; set { g_category = value; OnPropertyChanged(); } }
        public ObservableCollection<Sector> G_sector { get => g_sector; set { g_sector = value; OnPropertyChanged(); } }


        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            
            InitLists();
            InitDataBase();
        }

        void InitLists()
        {
            G_devices = new ObservableCollection<Device>(); 
            G_category = new ObservableCollection<Category>(); 
            G_sector = new ObservableCollection<Sector>(); 
        }

        void InitDataBase()
        {
            string connStr = "Data Source=" + file_db_name;
            connection = new SqliteConnection(connStr);
            connection.Open();
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
                    //AddTestValues(connection);
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
            g_category.Clear();
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = "SELECT * FROM Category";
            using (SqliteDataReader reader = comm.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        G_category.Add(new Category(Convert.ToInt32(reader.GetValue(0)),
                                reader.GetValue(1).ToString(),
                                reader.GetValue(2).ToString()));
                    }
                }
            }
        }

        void GetSectorFromDB(SqliteConnection conn)
        {
            g_sector.Clear();
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = "SELECT * FROM Sector";
            using (SqliteDataReader reader = comm.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        G_sector.Add(new Sector(Convert.ToInt32(reader.GetValue(0)),
                                reader.GetValue(1).ToString(),
                                reader.GetValue(2).ToString()));
                    }
                }
            }
        }

        void GetDevicesFromDB(SqliteConnection conn)
        {
            g_devices.Clear();
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = "SELECT * FROM Devices";
            using (SqliteDataReader reader = comm.ExecuteReader())
            {
                if (reader.HasRows) 
                {
                    while (reader.Read())  
                    {
                        Category category = new Category();
                        if(reader.GetValue(4) != DBNull.Value)
                        {
                            category = g_category.FirstOrDefault(t => t.ID_category == Convert.ToInt32(reader.GetValue(4)));
                        }
                        Sector sector = new Sector();
                        if (reader.GetValue(5) != DBNull.Value)
                        {
                            sector = g_sector.FirstOrDefault(t => t.ID_sector == Convert.ToInt32(reader.GetValue(5)));
                        }

                        G_devices.Add( new Device( Convert.ToInt32(reader.GetValue(0)), 
                                reader.GetValue(1).ToString(),
                                reader.GetValue(2).ToString(),
                                reader.GetValue(3).ToString(),
                                //reader.GetValue(4),
                                //reader.GetValue(5),
                                category,
                                sector)
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
            g_category.Add(category);
        }

        void AddSectorToDB(SqliteConnection conn, Sector sector)
        {
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = $"INSERT INTO Sector (Name, Description) VALUES ('{sector.Name}', '{sector.Description}')";
            comm.ExecuteNonQuery();
            g_sector.Add(sector);
        }

        void AddDeviceToDB(SqliteConnection conn, Device device)
        {
            var _category = device.Category == null ? "NULL" : device.Category.ID_category.ToString();
            var _sector = device.Sector == null ? "NULL" : device.Sector.ID_sector.ToString();

            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = "INSERT INTO Devices (Name, Model, Description, ID_category, ID_sector) VALUES " +
                $"('{device.Name}', '{device.Model}', '{device.Description}', {_category }, {_sector})";
            comm.ExecuteNonQuery();
            g_devices.Add(device);
        }

        void DeleteCategoryFromDB(SqliteConnection conn, Category category)
        {
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = $"DELETE FROM Category WHERE ID_category={category.ID_category}";
            comm.ExecuteNonQuery();
            g_category.Remove(category);
        }

        void DeleteSectorFromDB(SqliteConnection conn, Sector sector)
        {
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = $"DELETE FROM Sector WHERE ID_sector={sector.ID_sector}";
            comm.ExecuteNonQuery();
            g_sector.Remove(sector);
        }

        void DeleteDeviceFromDB(SqliteConnection conn, Device device)
        {
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = $"DELETE FROM Devices WHERE ID_device={device.ID_device}";
            comm.ExecuteNonQuery();
            g_devices.Remove(device);
        }

        void UpdateDeviceFromDB(SqliteConnection conn, Device device)
        {
            var _category = device.Category == null ? "NULL" : device.Category.ID_category.ToString();
            var _sector = device.Sector == null ? "NULL" : device.Sector.ID_sector.ToString();

            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = $"UPDATE Devices " +
                $"SET Name='{device.Name}', Model='{device.Model}', Description='{device.Description}', ID_category={_category}, ID_sector={_sector} " +
                $"WHERE ID_device={device.ID_device}";
            comm.ExecuteNonQuery();

            CancelButton_Click(null, null);
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
            SetControlsData("", "", "", null, null);


            Grid_AddAndUpdateDevice.Visibility = Visibility.Visible;
        }

        private void btnUpdateDevice_Click(object sender, RoutedEventArgs e)
        {
            if(dgAll_Devices.SelectedItem != null)
            {
                Device device = (Device)dgAll_Devices.SelectedItems[0];
                lblTopPage.Content = "Редактировать оборудование";
                Grid_AllDevice.Visibility = Visibility.Hidden;
                btnAdd_Device.Visibility = Visibility.Hidden;
                btnUpdate_Device.Visibility = Visibility.Visible;
                SetControlsData(device.Name, device.Model, device.Description, device.Category, device.Sector);


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
                DeleteDeviceFromDB(connection, (Device)dgAll_Devices.SelectedItems[0]);
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
            if (VerifyDataControls())
            {
                AddDeviceToDB(connection, new Device(0, tbAddAndUpdate_DeviceName.Text, tbAddAndUpdate_DeviceModel.Text,
                    tbAddAndUpdate_DeviceDescription.Text, 
                    (Category)cbAddAndUpdate_DeviceCategory.SelectedItem,
                    (Sector) cbAddAndUpdate_DeviceSector.SelectedItem));
            }
        }

        #region Controls
        bool VerifyDataControls()
        {
            if(tbAddAndUpdate_DeviceName.Text.Length == 0)
            {
                tbAddAndUpdate_DeviceName.Focus();
                MessageBox.Show("Задайте имя оборудованию!");
                return false;
            }

            if (tbAddAndUpdate_DeviceModel.Text.Length == 0)
            {
                tbAddAndUpdate_DeviceModel.Focus();
                MessageBox.Show("Задайте марку оборудования!");
                return false;
            }

            return true;
        }

        void SetControlsData(string name, string model, string description, Category category, Sector sector)
        {
            tbAddAndUpdate_DeviceName.Text = name;
            tbAddAndUpdate_DeviceModel.Text = model;
            tbAddAndUpdate_DeviceDescription.Text = description;
            cbAddAndUpdate_DeviceCategory.SelectedItem = category;
            cbAddAndUpdate_DeviceSector.SelectedItem = sector;
        }
        #endregion
        private void btnAll_DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if(dgAll_Category.SelectedItem != null)
            {
                try
                {
                    DeleteCategoryFromDB(connection, (Category)dgAll_Category.SelectedItems[0]);
                }
                catch (Microsoft.Data.Sqlite.SqliteException exp)
                {
                    LocalDebug.Log(exp.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите категорию.");
            }
        }

        private void btnUpdate_Device_Click(object sender, RoutedEventArgs e)
        {
            if (VerifyDataControls())
            {
                UpdateDeviceFromDB(connection, new Device(0, tbAddAndUpdate_DeviceName.Text, tbAddAndUpdate_DeviceModel.Text,
                    tbAddAndUpdate_DeviceDescription.Text,
                    (Category)cbAddAndUpdate_DeviceCategory.SelectedItem,
                    (Sector)cbAddAndUpdate_DeviceSector.SelectedItem));
                GetDevicesFromDB(connection);
            }            
        }

        private void btnAll_DeleteSector_Click(object sender, RoutedEventArgs e)
        {
            if (dgAll_Sector.SelectedItem != null)
            {
                try
                {
                    DeleteSectorFromDB(connection, (Sector)dgAll_Category.SelectedItems[0]);
                }
                catch (Microsoft.Data.Sqlite.SqliteException exp)
                {
                    LocalDebug.Log(exp.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите отрасль.");
            }
        }
    }
}
