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
            MainWindow._previousPage = Manager.MainFrame.Content;

            DataBaseClass.AddToCMB("select NameCountry from Country", Country_CMB);
            DataBaseClass.AddToCMB("select NameCity from City", City_CMB);
            DataBaseClass.AddToCMB("select NameKray from Kray", Kray_CMB);
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
