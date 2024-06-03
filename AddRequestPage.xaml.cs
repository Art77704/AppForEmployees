using AppForEmployees.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
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
    /// Логика взаимодействия для AddRequestPage.xaml
    /// </summary>
    public partial class AddRequestPage : Page
    {
        public static object _currentpage;
        public static int ClientIndex;
        public static int RoleIndex;
        public static string WorkDescription;
        public static string CadastralNumber;
        public static int AddressIndex;
        public static int NumberRequest;
        public static string NumberOKS;
        public static bool CheckPage = false;
        string _AddOrEdit;

        public AddRequestPage(string AddOrEdit="")
        {
            InitializeComponent();
            MainWindow.PageText.Text = "Добавление заявки";
            SearchClient_TXB.Text = "Введите фамилию";
            SearchAddress_TXB.Text = "Введите улицу";

            SelectClient_DT.ItemsSource = AppConnect.modelOdb.Client.ToList();
            SelectAddress_DT.ItemsSource = AppConnect.modelOdb.EstateAddress.ToList();
           
            _AddOrEdit = AddOrEdit;
            _currentpage = Manager.MainFrame.Content;

            if (AddOrEdit == "Edit")
            {
                EditData();
                MainWindow.PageText.Text = "Изменение заявки";
                AddRequest_BTN.Content = "Изменить";
                AddClient_BTN.Visibility = Visibility.Collapsed;
                AddAddress_BTN.Visibility = Visibility.Collapsed;
            }
        }

        private void AddClient_BTN_Click(object sender, RoutedEventArgs e)
        {
            CheckPage = true;
            Manager.MainFrame.Navigate(new AllClientsPage(true));
        }

        private void AddAddress_BTN_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddAddressPage());
        }


        private void SelectRole_CMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectRole_CMB.SelectedIndex == 1)
            {
                NumberOKS_TB.Visibility = Visibility.Collapsed;
                NumberOKS_TXB.Visibility = Visibility.Collapsed;
                SelectAddress_TB.Visibility = Visibility.Visible;
                SelectAddress_DT.Visibility = Visibility.Visible;
                SearchAddress_TXB.Visibility = Visibility.Visible;
                AddAddress_BTN.Visibility = Visibility.Visible;
            }
            else
            {
                SelectAddress_TB.Visibility = Visibility.Collapsed;
                SelectAddress_DT.Visibility = Visibility.Collapsed;
                SearchAddress_TXB.Visibility = Visibility.Collapsed;
                AddAddress_BTN.Visibility= Visibility.Collapsed;
                NumberOKS_TB.Visibility = Visibility.Visible;
                NumberOKS_TXB.Visibility = Visibility.Visible;
            }
        }


        void EditData()
        {
           
            var seladr = (from EstateAddress in AppConnect.modelOdb.EstateAddress
                       where EstateAddress.IdAddress== MainMenuPage.IdAddress
                       select EstateAddress).FirstOrDefault();

            var selcl = (from Client in AppConnect.modelOdb.Client
                          where Client.IdClient== MainMenuPage.IdClient
                          select Client).FirstOrDefault();

            SelectClient_DT.SelectedItem = selcl;
            SelectRole_CMB.SelectedIndex  = RoleIndex - 3;
            WorkDescription_TXB.Text = WorkDescription;
            CadastralNumber_TXB.Text = CadastralNumber;
            if (SelectRole_CMB.SelectedIndex == 1)
                SelectAddress_DT.SelectedItem = seladr;
            else
                NumberOKS_TXB.Text = NumberOKS;
        }

        int ShowIdRole()
        {
            int IdRole = 3;
            string RoleName = SelectRole_CMB.Text;
            var cmd = DataBaseClass.connectionOpen($"select IdRole from Role where RoleName='{RoleName}'");
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                IdRole = int.Parse(reader.GetValue(0).ToString());
            }
            return IdRole;
        }

        private void AddRequest_BTN_Click(object sender, RoutedEventArgs e)
        {
            string checkToNull = "";
            string checkToNull2 = "";
            Client cl = null;
            EstateAddress ea = new EstateAddress();
            if (SelectClient_DT.SelectedItem != null)
                cl = SelectClient_DT.SelectedItem as Client;
            if (SelectAddress_DT.SelectedItem != null)
                ea = SelectAddress_DT.SelectedItem as EstateAddress;
            if (cl != null)
                checkToNull = cl.ClientFirstName;
            if (ea != null)
                checkToNull2 = ea.EstateHouse;

            if (SelectRole_CMB.SelectedIndex == 1)
            {
                string[] textToCheck = { WorkDescription_TXB.Text, CadastralNumber_TXB.Text, checkToNull, SelectRole_CMB.Text, checkToNull2 };
                if (MainMenuPage.CheckDataToEmpty(textToCheck))
                    return;
            }
            else
            {
                string[] textToCheck = { WorkDescription_TXB.Text, CadastralNumber_TXB.Text, checkToNull, SelectRole_CMB.Text, NumberOKS_TXB.Text };
                if (MainMenuPage.CheckDataToEmpty(textToCheck))
                    return;
            }

            int IdClient = cl.IdClient;
            int IdAddress = ea.IdAddress;
            int IdRole = ShowIdRole();

            if (_AddOrEdit == "Edit")
            {
                string sql;
                if (SelectRole_CMB.SelectedIndex == 1)
                    sql = $"update Request set IdClient={IdClient}, IdRole={SelectRole_CMB.SelectedIndex + 3}, IdAddress={IdAddress}, WorkDescription='{WorkDescription_TXB.Text}', CadastralNumber='{CadastralNumber_TXB.Text}', NumberCapitalConstruction=NULL where IdRequest={NumberRequest}";
                else
                    sql = $"update Request set IdClient={IdClient}, IdRole={SelectRole_CMB.SelectedIndex + 3}, IdAddress=NULL,  WorkDescription='{WorkDescription_TXB.Text}', CadastralNumber='{CadastralNumber_TXB.Text}', NumberCapitalConstruction='{NumberOKS_TXB.Text}' where IdRequest={NumberRequest}";

                DataBaseClass.AddEditDel(sql);
                MessageBox.Show("Заявка изменена!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                Manager.MainFrame.Navigate(new ArchivePage());
                Manager.MainFrame.Navigate(new MainMenuPage());
            }
            else
            { // 0 - без адреса, 1 - с адресом
                string sql7;
                if (SelectRole_CMB.SelectedIndex == 0)
                    sql7 = $"insert into Request (IdClient, IdRole, WorkDescription, CadastralNumber, NumberCapitalConstruction) Values ({IdClient}, {IdRole}, '{WorkDescription_TXB.Text}', '{CadastralNumber_TXB.Text}', '{NumberOKS_TXB.Text}')";
                else
                    sql7 = $"insert into Request (IdClient, IdRole, WorkDescription, CadastralNumber, IdAddress) Values ({IdClient}, {IdRole}, '{WorkDescription_TXB.Text}', '{CadastralNumber_TXB.Text}', {IdAddress})";

                DataBaseClass.AddEditDel(sql7);
                MessageBox.Show("Заявка создана!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                Manager.MainFrame.Navigate(new ArchivePage());
                Manager.MainFrame.Navigate(new MainMenuPage());
            }
        }
       

        private void SearchAddress_TXB_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == string.Empty )
                (sender as TextBox).Text = "Введите улицу";
        }

        private void SearchClient_TXB_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Text = string.Empty;
        }

        private void SearchAddress_TXB_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Text = string.Empty;
        }


        private void SearchClient_TXB_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == string.Empty)
                (sender as TextBox).Text = "Введите фамилию";
        }

        private void SearchClient_TXB_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Получаем текст из TextBox
            string searchText = SearchClient_TXB.Text;
            if (searchText != "Введите фамилию")
            {
                var filteredData = AppConnect.modelOdb.Client.Where(x => x.ClientSurname.Contains(searchText)).ToList();
                SelectClient_DT.ItemsSource = filteredData;
            }
        }

        private void SearchAddress_TXB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchAddress_TXB.Text;
            if (searchText != "Введите улицу")
            {
                var filteredData = AppConnect.modelOdb.EstateAddress.Where(x => x.EstateStreet.Contains(searchText)).ToList();
                SelectAddress_DT.ItemsSource = filteredData;
            }
        }
      
    }
}
