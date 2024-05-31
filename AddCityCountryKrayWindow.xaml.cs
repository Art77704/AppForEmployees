using AppForEmployees.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace AppForEmployees
{
    /// <summary>
    /// Логика взаимодействия для AddCityCountryKrayWindow.xaml
    /// </summary>
    public partial class AddCityCountryKrayWindow : Window
    {
        string _CityCountryKray;
        string _text;
        string _textpage;
        public AddCityCountryKrayWindow(string text, string textpage="")
        {
            InitializeComponent();
            AppConnect.modelOdb = new RCCEntities();
            _text = text;
            _textpage = textpage;

            if (text == "Kray")
                _Kray();
            else if (text == "Country")
                _Country();
            else if (text == "City")
                _City();
        }

        SqlCommand _City()
        {
            CityCountryKray_TB.Text = "Введите наименование города:";
            this.Title = "Добавление города";
            _CityCountryKray = "Город добавлен!";


            var connection = MainWindow.connectionOpen();
            string sql = $"INSERT INTO City (NameCity) VALUES ('{CityCountryKray_TXB.Text}')";

            SqlCommand command = new SqlCommand(sql, connection);
            return command;

        }

        SqlCommand _Country()
        {
            CityCountryKray_TB.Text = "Введите наименование страны:";
            this.Title = "Добавление страны";
            _CityCountryKray = "Страна добавлена!";


            var connection = MainWindow.connectionOpen();
            string sql = $"INSERT INTO Country (NameCountry) VALUES ('{CityCountryKray_TXB.Text}')";

            SqlCommand command = new SqlCommand(sql, connection);
            return command;

        }

        SqlCommand _Kray()
        {
            CityCountryKray_TB.Text = "Введите наименование края:";
            this.Title = "Добавление края";
            _CityCountryKray = "Край добавлен!";

            var connection = MainWindow.connectionOpen();
            string sql = $"INSERT INTO Kray (NameKray) VALUES ('{CityCountryKray_TXB.Text}')";

            SqlCommand command = new SqlCommand(sql, connection);
            return command;

        }

        bool CheckRepeat()
        {
            if (_text == "Kray")
            {
                SqlConnection con = MainWindow.connectionOpen();
                SqlCommand cmd = new SqlCommand($"select NameKray from Kray where NameKray='{CityCountryKray_TXB.Text}'", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    MessageBox.Show("Такой край уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return true;
                }
                return false;
            }
            else if (_text == "Country")
            {
                SqlConnection con = MainWindow.connectionOpen();
                SqlCommand cmd = new SqlCommand($"select NameCountry from Country where NameCountry='{CityCountryKray_TXB.Text}'", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    MessageBox.Show("Такая страна уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return true;
                }
                return false;
            }
            else
            {
                SqlConnection con = MainWindow.connectionOpen();
                SqlCommand cmd = new SqlCommand($"select NameCity from City where NameCity='{CityCountryKray_TXB.Text}'", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    MessageBox.Show("Такой город уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return true;
                }
                return false;
            }
        }

        private void SaveData_BTN_Click(object sender, RoutedEventArgs e)
        {
            string[] textToCheck = { CityCountryKray_TXB.Text };
            if (MainMenuPage.CheckDataToEmpty(textToCheck))
                return;
            else if (CheckRepeat())
                return;

            SqlCommand command;

            if (_text == "Kray")
            {
                _Kray();
                command = _Kray();
            }
            else if (_text == "Country")
            {
                _Country();
                command = _Country();
            }
            else
            {
                _City();
                command = _City();
            }
          
             command.ExecuteNonQuery();

            //MainWindow.SaveToBD();
            MessageBox.Show($"{_CityCountryKray}", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
            Manager.MainFrame.Navigate(new AddClientPage(AddClientPage._cl));

        }
    }
}
