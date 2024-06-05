using AppForEmployees.DataBase;
using System;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AppForEmployees
{
    /// <summary>
    /// Логика взаимодействия для AddClientPage.xaml
    /// </summary>
    public partial class AddClientPage : Page
    {
        public static Client _cl;
        bool edit=false;
        string _text;
        
        public AddClientPage(Client cl=null)
        {
            InitializeComponent();
            MainWindow.PageText.Text = "Добавление клиента";
            MainWindow._previousPage = Manager.MainFrame.Content;
           
            MainWindow._previousPage = Manager.MainFrame.Content;
            SelectCity_CMB.ItemsSource = AppConnect.modelOdb.City.ToList();
            SelectCountry_CMB.ItemsSource = AppConnect.modelOdb.Country.ToList();
            SelectKray_CMB.ItemsSource = AppConnect.modelOdb.Kray.ToList();

            if (cl != null)
            {
                DataContext = cl;
                _cl = cl;
                edit = true;
                MainWindow.PageText.Text = "Изменение данных о клиенте";
                var alldata = AppConnect.modelOdb.PlaceOfBirth.FirstOrDefault(c => c.IdPlaceOfBirth == cl.IdPlaceOfBirth);
                var cityy = AppConnect.modelOdb.City.FirstOrDefault(c => c.NameCity == alldata.City.NameCity);
                var country = AppConnect.modelOdb.Country.FirstOrDefault(c => c.NameCountry == alldata.Country.NameCountry);
                Kray kray = null;
                try
                {
                    kray = AppConnect.modelOdb.Kray.FirstOrDefault(c => c.NameKray == alldata.Kray.NameKray);
                }
                catch
                {
                    SelectKray_CMB.SelectedItem = kray;
                }
                if (alldata != null)
                {
                    SelectCity_CMB.UpdateLayout();
                    SelectCountry_CMB.UpdateLayout();
                    SelectKray_CMB.UpdateLayout();
                    SelectCity_CMB.SelectedItem = cityy;
                    SelectCountry_CMB.SelectedItem = country;
                    SelectKray_CMB.SelectedItem = kray;
                }
            }
        }

        void EditData(int IdPlaceOfBirth)//Доработать метод (не изменяет страну город край если их нет в бд)
        {
            _cl.IdPlaceOfBirth= IdPlaceOfBirth;
            AppConnect.modelOdb.Client.AddOrUpdate(_cl);
            AppConnect.modelOdb.SaveChanges();

            MessageBox.Show("Данные изменены!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);

            Manager.MainFrame.Navigate(new MainMenuPage());
            Manager.MainFrame.Navigate(new ArchivePage());
            Manager.MainFrame.Navigate(new MainMenuPage());
            Manager.MainFrame.Navigate(new AllClientsPage());

        }

        private void AddAddress_BTN_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddAddressPage());
        }

        int ShowIdCity()
        {
            int IdCity = 3;
            if (SelectCity_CMB.SelectedItem is City ci)
                IdCity = ci.IdCity;

            return IdCity;
           
        }
        int ShowIdCountry()
        {
            int IdCountry = 3;
            if (SelectCountry_CMB.SelectedItem is Country country)
                IdCountry = country.IdCountry;

            return IdCountry;
        }
        int ShowIdKray()
        {
            int IdKray = 3;
            if (SelectKray_CMB.SelectedItem is Kray kray)
                IdKray = kray.IdKray;

            return IdKray;
        }
            


        private void AddClient_BTN_Click(object sender, RoutedEventArgs e)
        {
            string[] textToCheck = { FirstName_TXB.Text, Surname_TXB.Text, DateOfBirth_DP.Text, SeriyaNumber_TXB.Text, WhoIssued_TXB.Text, DepartmentCode_TXB.Text, SelectCountry_CMB.Text, SelectCity_CMB.Text };

            if (MainMenuPage.CheckDataToEmpty(textToCheck))
                return;

            AppConnect.modelOdb = new RCCEntities();
           

           
            int idCity = ShowIdCity();
            int idCountry = ShowIdCountry();
            int idKray = ShowIdKray();
           var cmd = DataBaseClass.connectionOpen($"select * from PlaceOfBirth where IdCountry={idCountry} and IdCity={idCity} and IdKray={idKray}");
            int IdPlaceOfBirth = -1;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                IdPlaceOfBirth=int.Parse(reader.GetValue(0).ToString());
            }
            if (IdPlaceOfBirth == -1)
            {
                PlaceOfBirth pl = new PlaceOfBirth();
                {
                    pl.IdCity = idCity;
                    pl.IdCountry = idCountry;
                    if (SelectKray_CMB.SelectedIndex == -1)
                        pl.IdKray = null;
                    else
                        pl.IdKray = idKray;
                }

                AppConnect.modelOdb.PlaceOfBirth.Add(pl);
                MainWindow.SaveToBD();
               var cmd2 = DataBaseClass.connectionOpen($"select * from PlaceOfBirth");
               
                SqlDataReader reader2 = cmd2.ExecuteReader();
                while (reader2.Read())
                {
                    IdPlaceOfBirth = int.Parse(reader2.GetValue(0).ToString());
                }
            }

            if (edit)
            {
                EditData(IdPlaceOfBirth);
                return;
            }

            Client cl = new Client();
            {
                cl.ClientSurname = Surname_TXB.Text;
                cl.ClientFirstName = FirstName_TXB.Text;
                cl.ClientPatronymic= Patronymic_TXB.Text;
                cl.DateOfBirth = (DateTime)DateOfBirth_DP.SelectedDate;
                cl.WhoIssued = WhoIssued_TXB.Text;
                cl.SeriyaNumber =SeriyaNumber_TXB.Text;
                cl.DepartmentCode = int.Parse(DepartmentCode_TXB.Text);
                cl.DateOfIssue=(DateTime)DateOfIssue_DP.SelectedDate;
                cl.IdPlaceOfBirth = IdPlaceOfBirth;

            }; // Добавляет в БД  данные
            AppConnect.modelOdb.Client.Add(cl);
            MainWindow.SaveToBD();

           
            MessageBox.Show("Клиент добавлен в систему!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);

            if (AddRequestPage.CheckPage)
            {
                AddRequestPage.CheckPage = false;
            }
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

        private void AddCountry_BTN_Click(object sender, RoutedEventArgs e)
        {
         
            AddCityCountryKrayWindow w = new AddCityCountryKrayWindow("Country");
            w.ShowDialog();
        }


        private void CheckText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char t = Convert.ToChar(e.Text.ToLower());
            if (!(t >= '0' && t <= '9'))
            {
                e.Handled = true;
                MessageBox.Show("Разрешен ввод только цифр!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckSpace(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;//Если пробел, то ввод недопустим
        }

        

    }
}
