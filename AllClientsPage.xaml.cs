using AppForEmployees.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Drawing;
using System.Threading;

namespace AppForEmployees
{
    /// <summary>
    /// Логика взаимодействия для AllClientsPage.xaml
    /// </summary>
    public partial class AllClientsPage : Page
    {
        public static bool GoToAddRequestPage;
        public AllClientsPage(bool AddRequest=false)
        {
            InitializeComponent();
            MainWindow.PageText.Text = "Список клиентов";
            Clients_DT.ItemsSource = null;
            GoToAddRequestPage=AddRequest;
            Clients_DT.ItemsSource= AppConnect.modelOdb.Client.ToList();
        }

        private void AddClient_BTN_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddClientPage());
        }

        private void Edit_BTN_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = Clients_DT.SelectedItem as Client;
            Manager.MainFrame.Navigate(new AddClientPage(selectedRow));
        }

        private void Delete_BTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var res = MessageBox.Show("Вы действительно хотите удалить данные?", "Удаление клиента(ов)", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    if (Clients_DT.SelectedItem != null)
                    {
                            var selectedRow = Clients_DT.SelectedItems.Cast<Client>().ToList();

                        if (selectedRow != null)
                        {
                            AppConnect.modelOdb.Client.RemoveRange(selectedRow);
                            AppConnect.modelOdb.SaveChanges();
                            Clients_DT.ItemsSource = AppConnect.modelOdb.Client.ToList();
                        }
                        else
                            MessageBox.Show("Вы не выбрали клиента(ов) для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                    else
                        MessageBox.Show("Вы не выбрали клиента(ов) для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Для этого клиента есть активная заявка!", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
