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
        EstateAddress _ea = new EstateAddress();
        public AddAddressPage()
        {
            InitializeComponent();
            MainWindow.PageText.Text = "Добавление адреса";
            EstateAddress ea = new EstateAddress();
            DataContext = ea;
            _ea = ea;
            //DataBaseClass.AddToCMB("select NameCity from City", City_CMB);
            City_CMB.ItemsSource = AppConnect.modelOdb.City.ToList();
        }

        private void AddCity_BTN_Click(object sender, RoutedEventArgs e)
        {
            AddCityCountryKrayWindow w = new AddCityCountryKrayWindow("City", "AddAddressPage");
            w.ShowDialog();
        }

        private void SaveDataAddress_BTN_Click(object sender, RoutedEventArgs e)
        {
            string[] textToCheck = { Street_TXB.Text, House_TXB.Text };
            if (MainMenuPage.CheckDataToEmpty(textToCheck))
                return;

            try
            {
                var ci = City_CMB.SelectedItem as City;
                _ea.IdCity = ci.IdCity;
                AppConnect.modelOdb.EstateAddress.Add(_ea);
                AppConnect.modelOdb.SaveChanges();
                MessageBox.Show("Адрес добавлен!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                Manager.MainFrame.Navigate(new AddRequestPage());
            }
            catch
            {
                MessageBox.Show("Ошибка ввода данных!");
            }
            
        }
    }
}
