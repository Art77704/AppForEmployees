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
using System.Threading;
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
        public static int _IdEmployee;
        public static object _currentpage;
        public static int IdRequest;
        public static string _RoleName;
        public static int IdAddress ;
        public static int IdClient ;

        public MainMenuPage()
        {
            InitializeComponent();
            MainWindow._MenuRCC.Visibility = Visibility.Visible;
            MainWindow._previousPage = Manager.MainFrame.Content;

            ListOfRequestsDT.ItemsSource = AppConnect.modelOdb.Request.ToList();
            MainWindow.PageText.Text = "Список активных заявок";
            Search_TXB.Text = "Поиск по описанию работы";
            MainWindow.GoBackBTN.Visibility = Visibility.Visible;
            
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

       

        private void EditRequest_BTN_Click(object sender, RoutedEventArgs e)
        {
            var selitem = ListOfRequestsDT.SelectedItem as Request;
            IdRequest = selitem.IdRequest;

            if (selitem.IdEmployee != null)
                _IdEmployee = (int)selitem.IdEmployee;

            if (selitem.IdAddress !=null)
                IdAddress = (int)selitem.IdAddress;
            IdClient = selitem.IdClient;

            var cmd10 = DataBaseClass.connectionOpen($"(Select distinct req.IdRequest as НомерЗаявки, c.ClientSurname as ФамилияКлиента, c.ClientFirstName as ИмяКлиента, c.ClientPatronymic as ОтчествоКлиента, r.RoleName as ЗаявкаДля, req.WorkDescription as ОписаниеРаботы, req.CadastralNumber as КадастровыйНомер, req.NumberCapitalConstruction as НомерОКС, City.NameCity as Город, EstateAddress.EstateStreet as Улица,  EstateAddress.EstateHouse as Дом,  EstateAddress.EstateFlat as Квартира from Request req inner join Client c on req.IdClient = c.IdClient inner join Role r on req.IdRole = r.IdRole left join EstateAddress on req.IdAddress = EstateAddress.IdAddress left join City on EstateAddress.IdCity=City.IdCity where req.IdAddress is not null and req.IdRequest={IdRequest}) union (Select distinct req.IdRequest as НомерЗаявки, c.ClientSurname as ФамилияКлиента, c.ClientFirstName as ИмяКлиента, c.ClientPatronymic as ОтчествоКлиента, r.RoleName as ЗаявкаДля, req.WorkDescription as ОписаниеРаботы, req.CadastralNumber as КадастровыйНомер, req.NumberCapitalConstruction as НомерОКС, City.NameCity as Город, null as Улица, null as Дом, null as Квартира from Request req inner join Client c on req.IdClient = c.IdClient inner join Role r on req.IdRole = r.IdRole left join EstateAddress on req.IdAddress = EstateAddress.IdAddress left join City on EstateAddress.IdCity = City.IdCity where req.IdAddress is null and req.IdRequest={IdRequest})");
            SqlDataReader reader10 = cmd10.ExecuteReader();
            while (reader10.Read())
            {
                AddRequestPage.WorkDescription = reader10.GetValue(5).ToString();
                AddRequestPage.CadastralNumber = reader10.GetValue(6).ToString();
                AddRequestPage.NumberOKS = reader10.GetValue(7).ToString();
            }
            AddRequestPage.NumberRequest = IdRequest;
            AddRequestPage.ClientIndex = selitem.IdClient;
            if (selitem.IdAddress != null)
            AddRequestPage.AddressIndex= (int)selitem.IdAddress;
            AddRequestPage.RoleIndex = selitem.IdRole;
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
            if (ListOfRequestsDT.SelectedItem != null)
            {
                var selItem = ListOfRequestsDT.SelectedItem as Request;
                IdRequest = selItem.IdRequest;
                Manager.MainFrame.Navigate(new RequestPage(selItem.IdRequest, selItem));
            }
            
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
            string searchText = Search_TXB.Text;
            if (searchText != "Поиск по описанию работы")
            {
                var filteredData = AppConnect.modelOdb.Request.Where(x => x.WorkDescription.Contains(searchText)).ToList();
                ListOfRequestsDT.ItemsSource = filteredData;
            }
        }

        private void Search_TXB_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Text = string.Empty;
        }

        private void Search_TXB_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == string.Empty)
                (sender as TextBox).Text = "Поиск по описанию работы";
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

    }
}
