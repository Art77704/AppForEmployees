using AppForEmployees.DataBase;
using SharpVectors.Dom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Логика взаимодействия для MainMenuPage.xaml
    /// </summary>
    public partial class MainMenuPage : Page
    {
        public static int IdEmployee;
        public static object _currentpage;
        public static int IdRequest;
        public static string _RoleName;

        public MainMenuPage()
        {
            InitializeComponent();
            MainWindow._MenuRCC.Visibility = Visibility.Visible;
            
            MainWindow.PageText.Text = "Список активных заявок";
            MainWindow.GoBackBTN.Visibility = Visibility.Visible;
            DataBaseClass.ShowOrUpdateDT("Select req.IdRequest as НомерЗаявки, c.ClientSurname as ФамилияКлиента, c.ClientFirstName as ИмяКлиента, c.ClientPatronymic as ОтчествоКлиента, r.RoleName as ЗаявкаДля from Request req, Role r, Client c where c.IdClient=req.IdClient and r.IdRole=req.IdRole", ListOfRequestsDT);
            
            ShowIdEmployee();
            _currentpage = Manager.MainFrame.Content;


           
           

        }

        void ShowIdEmployee()
        {

            var cmd = DataBaseClass.connectionOpen($"select e.IdEmployee from Employee e, AuthorizationAcc a where a.IdAuth={AuthorizationPage.UserAuthId} and e.IdAuth=a.IdAuth");
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                IdEmployee = int.Parse(reader.GetValue(0).ToString());
            }

        }

        private void ArchiveBTN_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ArchivePage());

        }

        private void AddRequestBTN_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddRequestPage());
        }

        private void AddClient_BTN_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AllClientsPage());
        }

        private void PersonalAccount_BTN_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new PersonalAccountPage());

        }

        private void EditRequest_BTN_Click(object sender, RoutedEventArgs e)
        {
            string City = "";
            string Street = "";
            string House = "";
            string Flat = "";
            
            DataRowView selectedRow = (DataRowView)ListOfRequestsDT.SelectedItem;
            IdRequest = Convert.ToInt32(selectedRow["НомерЗаявки"]);
            var cmd10 = DataBaseClass.connectionOpen($"(Select distinct req.IdRequest as НомерЗаявки, c.ClientSurname as ФамилияКлиента, c.ClientFirstName as ИмяКлиента, c.ClientPatronymic as ОтчествоКлиента, r.RoleName as ЗаявкаДля, req.WorkDescription as ОписаниеРаботы, req.CadastralNumber as КадастровыйНомер, req.NumberCapitalConstruction as НомерОКС, City.NameCity as Город, EstateAddress.EstateStreet as Улица,  EstateAddress.EstateHouse as Дом,  EstateAddress.EstateFlat as Квартира from Request req inner join Client c on req.IdClient = c.IdClient inner join Role r on req.IdRole = r.IdRole left join EstateAddress on req.IdAddress = EstateAddress.IdAddress left join City on EstateAddress.IdCity=City.IdCity where req.IdAddress is not null and req.IdRequest={IdRequest}) union (Select distinct req.IdRequest as НомерЗаявки, c.ClientSurname as ФамилияКлиента, c.ClientFirstName as ИмяКлиента, c.ClientPatronymic as ОтчествоКлиента, r.RoleName as ЗаявкаДля, req.WorkDescription as ОписаниеРаботы, req.CadastralNumber as КадастровыйНомер, req.NumberCapitalConstruction as НомерОКС, City.NameCity as Город, null as Улица, null as Дом, null as Квартира from Request req inner join Client c on req.IdClient = c.IdClient inner join Role r on req.IdRole = r.IdRole left join EstateAddress on req.IdAddress = EstateAddress.IdAddress left join City on EstateAddress.IdCity = City.IdCity where req.IdAddress is null and req.IdRequest={IdRequest})");
            SqlDataReader reader10 = cmd10.ExecuteReader();
            while (reader10.Read())
            {
                AddRequestPage.WorkDescription = reader10.GetValue(5).ToString();
                AddRequestPage.CadastralNumber = reader10.GetValue(6).ToString();
                AddRequestPage.NumberOKS = reader10.GetValue(7).ToString();
                City = reader10.GetValue(8).ToString();
                Street = reader10.GetValue(9).ToString();
                House = reader10.GetValue(10).ToString();
                Flat = reader10.GetValue(11).ToString();
            }
            AddRequestPage.NumberRequest = IdRequest;

            string[] indexcl = DataBaseClass.GetData($"select cl.IdClient from Client cl where cl.ClientFirstName='{Convert.ToString(selectedRow["ИмяКлиента"])}' and cl.ClientSurname='{Convert.ToString(selectedRow["ФамилияКлиента"])}' and cl.ClientPatronymic='{Convert.ToString(selectedRow["ОтчествоКлиента"])}'");
            AddRequestPage.ClientIndex = int.Parse(indexcl[0]);
           
            int IdCity = 0;
            
            var cmd2 = DataBaseClass.connectionOpen($"select IdCity from City where NameCity='{City}'");
            SqlDataReader reader2 = cmd2.ExecuteReader();
            while (reader2.Read())
            {
                IdCity = int.Parse(reader2.GetValue(0).ToString());

            }

            var cmd3 = DataBaseClass.connectionOpen($"select ea.IdAddress from EstateAddress ea, City c where ea.EstateStreet='{Street}' and ea.EstateHouse='{House}' and ea.EstateFlat='{Flat}' and ea.IdCity={IdCity} and ea.IdCity=c.IdCity");
            SqlDataReader reader3 = cmd3.ExecuteReader();
            while (reader3.Read())
            {
                AddRequestPage.AddressIndex = int.Parse(reader3.GetValue(0).ToString());

            }

             var cmd4 = DataBaseClass.connectionOpen($"select IdRole from Role where RoleName='{Convert.ToString(selectedRow["ЗаявкаДля"])}'");
            SqlDataReader reader4 = cmd4.ExecuteReader();
            while (reader4.Read())
            {
                AddRequestPage.RoleIndex = int.Parse(reader4.GetValue(0).ToString());

            }


            if (ListOfRequestsDT.SelectedItems.Count > 1) { MessageBox.Show("Нельзя изменить несколько заявок сразу!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information); return; }
           
            Manager.MainFrame.Navigate(new AddRequestPage("Edit"));
        }



        public static bool CheckDataToEmpty(string[] textToCheck)
        {
            try
            {
                for (int i = 0; i < textToCheck.Length; i++)
                {
                    if (string.IsNullOrEmpty(textToCheck[i]))
                    {
                        MessageBox.Show("Вы не заполнили поля обязательного ввода!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return true;
                    }
                }
                return false;
            }
            catch 
            {
                MessageBox.Show("Вы не заполнили поля обязательного ввода!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return true;
            }
            
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)ListOfRequestsDT.SelectedItem;
            IdRequest = Convert.ToInt32(selectedRow["НомерЗаявки"]);

            DataGridRow row = sender as DataGridRow;
            Manager.MainFrame.Navigate(new RequestPage());
        }
        private void EditRequestButton_Loaded(object sender, RoutedEventArgs e)
        {
            string RoleName = "";
            Button editRequestButton = (Button)sender;
            var cmd2 = DataBaseClass.connectionOpen($"select a.IdRole, r.RoleName from Role r, AuthorizationAcc a where r.IdRole=a.IdRole and a.IdAuth={AuthorizationPage.UserAuthId}");
            SqlDataReader reader2 = cmd2.ExecuteReader();
            while (reader2.Read())
            {
                RoleName = reader2.GetValue(1).ToString();
            }
            _RoleName= RoleName;
            if (RoleName == "Администратор" || RoleName == "Менеджер")
            {
                editRequestButton.Visibility = Visibility.Visible;
                AddRequestBTN.Visibility = Visibility.Visible;
                AddClient_BTN.Visibility = Visibility.Visible;
                
            }
            else
                MainWindow.HideButton(MainWindow.menuItems);
        }

        private void Search_TXB_TextChanged(object sender, TextChangedEventArgs e)
        {
            var con = DataBaseClass.Connection();
            SqlDataAdapter dataAdapter = new SqlDataAdapter("Select req.IdRequest as НомерЗаявки, c.ClientSurname as ФамилияКлиента, c.ClientFirstName as ИмяКлиента, c.ClientPatronymic as ОтчествоКлиента, r.RoleName as ЗаявкаДля from Request req, Role r, Client c where c.IdClient=req.IdClient and r.IdRole=req.IdRole", con);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            ListOfRequestsDT.ItemsSource = dataTable.DefaultView;
            string searchText = Search_TXB.Text;

            if (string.IsNullOrEmpty(searchText))
            {
                // Если строка поиска пустая, то отобразить все записи
                ListOfRequestsDT.ItemsSource = dataTable.DefaultView;
            }
            else
            {
                // Иначе выполняется фильрация записей в DataTable, используя метод Select()
                DataRow[] rows = dataTable.Select($"ФамилияКлиента LIKE '%{searchText}%'");
                DataTable filteredTable = dataTable.Clone(); // Клонирование структуры таблицы
                foreach (DataRow row in rows)
                {
                    filteredTable.ImportRow(row); // Копирование найденных записей в новую таблицу
                }
                ListOfRequestsDT.ItemsSource = filteredTable.DefaultView;
            }
        }

        
    }
}
