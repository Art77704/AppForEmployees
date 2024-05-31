using AppForEmployees.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace AppForEmployees
{
    /// <summary>
    /// Логика взаимодействия для AddAddressPage.xaml
    /// </summary>
    public partial class AddAddressPage : Page
    {
        public AddAddressPage()
        {
            InitializeComponent();
            MainWindow.PageText.Text = "Добавление адреса";
            AddCityToCMB();
        }

        private void AddCity_BTN_Click(object sender, RoutedEventArgs e)
        {
            AddCityCountryKrayWindow w = new AddCityCountryKrayWindow("City", "AddAddressPage");
            w.ShowDialog();
        }

        void AddCityToCMB()
        {
            var cmd = ConToBD.connectionOpen("select NameCity from City");
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var City = reader.GetValue(0).ToString();

                City_CMB.Items.Add($"{City}");
            }
        }

        private void SaveDataAddress_BTN_Click(object sender, RoutedEventArgs e)
        {
            string[] textToCheck = { Street_TXB.Text, House_TXB.Text };
            if (MainMenuPage.CheckDataToEmpty(textToCheck))
                return;

            AppConnect.modelOdb = new RCCEntities();

            EstateAddress ea = new EstateAddress();
            {
                ea.IdCity = City_CMB.SelectedIndex + 1;
                ea.EstateStreet = Street_TXB.Text;
                ea.EstateHouse = House_TXB.Text;
                ea.EstateFlat = Flat_TXB.Text;

            }; // Добавляет в БД  данные
            AppConnect.modelOdb.EstateAddress.Add(ea);
            MainWindow.SaveToBD();
            MessageBox.Show("Адрес добавлен!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
            Manager.MainFrame.Navigate(new AddRequestPage());
        }
    }
}
