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
    /// Логика взаимодействия для PlaceOfBirthPage.xaml
    /// </summary>
    public partial class PlaceOfBirthPage : Page
    {
        public PlaceOfBirthPage()
        {
            InitializeComponent();
            MainWindow.PageText.Text = "Добавление места рождения";

            AddCountryToCMB();
            AddCityToCMB();
            AddKrayToCMB();
        }

        private void SaveData_BTN_Click(object sender, RoutedEventArgs e)
        {
            AppConnect.modelOdb = new RCCEntities();

            PlaceOfBirth pb = new PlaceOfBirth();
            {
                pb.IdCity = City_CMB.SelectedIndex + 1;
                pb.IdCountry = Country_CMB.SelectedIndex + 1;
                pb.IdKray = Kray_CMB.SelectedIndex + 1;

            }; // Добавляет в БД  данные
            AppConnect.modelOdb.PlaceOfBirth.Add(pb);
            MainWindow.SaveToBD();
            MessageBox.Show("Место рождения добавлено!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);

            Manager.MainFrame.GoBack();
        }

        void AddCountryToCMB()
        {
            var connection = MainWindow.connectionOpen();
            SqlCommand cmd = new SqlCommand("select NameCountry from Country", connection);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var NameCountry = reader.GetValue(0).ToString();

                Country_CMB.Items.Add($"{NameCountry}");
            }
        }

         void AddCityToCMB()
         {
             var connection = MainWindow.connectionOpen();
             SqlCommand cmd = new SqlCommand("select NameCity from City", connection);
             SqlDataReader reader = cmd.ExecuteReader();
             while (reader.Read())
             {
                 var NameCity = reader.GetValue(0).ToString();

                 City_CMB.Items.Add($"{NameCity}");
             }
         }

         void AddKrayToCMB()
         {
             var connection = MainWindow.connectionOpen();
             SqlCommand cmd = new SqlCommand("select NameKray from Kray", connection);
             SqlDataReader reader = cmd.ExecuteReader();
             while (reader.Read())
             {
                 var NameKray = reader.GetValue(0).ToString();

                 Kray_CMB.Items.Add($"{NameKray}");
             }
         }


        private void AddKray_BTN_Click(object sender, RoutedEventArgs e)
        {
            AddCityCountryKrayWindow w = new AddCityCountryKrayWindow("Kray");
            w.ShowDialog();
        }

        private void AddCity_BTN_Click(object sender, RoutedEventArgs e)
        {
            AddCityCountryKrayWindow w = new AddCityCountryKrayWindow("City");
            w.ShowDialog();
        }

        private void AddCountry_Click(object sender, RoutedEventArgs e)
        {
            AddCityCountryKrayWindow w = new AddCityCountryKrayWindow("Country");
            w.ShowDialog();
        }
    }
}
