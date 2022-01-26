using catalog.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
        private const string _file_db_name = "catalog.db";

        private DBService _service;

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
            _service = new DBService(_file_db_name);
            _service.CreateTables();

            GetDataFromDB();
        }

        void GetDataFromDB()
        {
            G_category = _service.GetCategoryFromDB();
            G_sector = _service.GetSectorFromDB();
            G_devices = _service.GetDevicesFromDB();
        }

        
        #region Main page device controls handlers

        private void btnAll_ChangeImgDevice_Click(object sender, RoutedEventArgs e)
        {
            if(dgAll_Devices.SelectedItem != null)
            {
                OpenFileDialog file_dialog = new OpenFileDialog();
                file_dialog.Filter = "Image Files(*.bmp;*.jpg;*.gif)|*.bmp;*.jpg;*.gif";
                if (file_dialog.ShowDialog() == true)
                {
                    ((Device)dgAll_Devices.SelectedItem).Image = File.ReadAllBytes(file_dialog.FileName);
                    _service.UpdateDeviceInDB((Device)dgAll_Devices.SelectedItem);
                    dgAll_Devices_SelectionChanged(null, null);
                }
            }            
        }

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

        private void dgAll_Devices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAll_Devices.SelectedItem != null)
            {
                var dataImage = ((Device)dgAll_Devices.SelectedItem).Image;
                if (dataImage == null || dataImage.Length == 0)
                {
                    imgAll_Device.Source = null;
                }
                if (dataImage != null)
                {
                    var image = new BitmapImage();
                    using (var mem = new MemoryStream(dataImage))
                    {
                        mem.Position = 0;
                        image.BeginInit();
                        image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.UriSource = null;
                        image.StreamSource = mem;
                        image.EndInit();
                    }
                    image.Freeze();
                    imgAll_Device.Source = image;
                }
            }
        } 

        private void btnDeleteDevice_Click(object sender, RoutedEventArgs e)
        {
            if (dgAll_Devices.SelectedItem != null)
            {
                if (_service.DeleteDeviceFromDB((Device)dgAll_Devices.SelectedItems[0]))
                {
                    G_devices.Remove((Device)dgAll_Devices.SelectedItems[0]);
                }
            }
            else
            {
                MessageBox.Show("Выберите оборудование");
            }
        }

        #endregion

        #region Add/Update device page controls handlers
        private void btnAdd_Device_Click(object sender, RoutedEventArgs e)
        {
            if (VerifyDataControls())
            {
                _service.AddDeviceToDB(new Device(0, tbAddAndUpdate_DeviceName.Text, tbAddAndUpdate_DeviceModel.Text,
                    tbAddAndUpdate_DeviceDescription.Text,
                    (Category)cbAddAndUpdate_DeviceCategory.SelectedItem,
                    (Sector)cbAddAndUpdate_DeviceSector.SelectedItem));
                G_devices = _service.GetDevicesFromDB();
            }
        }

        private void btnUpdate_Device_Click(object sender, RoutedEventArgs e)
        {
            if (VerifyDataControls())
            {
                _service.UpdateDeviceInDB( new Device(
                    ((Device)dgAll_Devices.SelectedItem).ID_device, 
                    tbAddAndUpdate_DeviceName.Text, 
                    tbAddAndUpdate_DeviceModel.Text,
                    tbAddAndUpdate_DeviceDescription.Text,
                    (Category)cbAddAndUpdate_DeviceCategory.SelectedItem,
                    (Sector)cbAddAndUpdate_DeviceSector.SelectedItem));
                G_devices = _service.GetDevicesFromDB();
            }            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Grid_AddAndUpdateDevice.Visibility = Visibility.Hidden;
            Grid_AllDevice.Visibility = Visibility.Visible;
        }

        #endregion

        #region Main page category and sector controls handlers
        private void btnAll_DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (dgAll_Category.SelectedItem != null)
            {
                try
                {
                    if (_service.DeleteCategoryFromDB((Category)dgAll_Category.SelectedItems[0]))
                    {
                        G_category.Remove((Category)dgAll_Category.SelectedItems[0]);
                    }
                }
                catch (SqliteException exp)
                {
                    LocalDebug.Log(exp.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите категорию.");
            }
        }

        private void btnAll_DeleteSector_Click(object sender, RoutedEventArgs e)
        {
            if (dgAll_Sector.SelectedItem != null)
            {
                try
                {
                    if (_service.DeleteSectorFromDB((Sector)dgAll_Sector.SelectedItems[0]))
                    {
                        G_sector.Remove((Sector)dgAll_Sector.SelectedItems[0]);
                    }
                }
                catch (SqliteException exp)
                {
                    LocalDebug.Log(exp.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите отрасль.");
            }
        }
        #endregion


        #region Controls
        bool VerifyDataControls()
        {
            if (tbAddAndUpdate_DeviceName.Text.Length == 0)
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

    }
}
