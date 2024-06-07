using AppForEmployees.DataBase;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
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
    /// Логика взаимодействия для ArchivePage.xaml
    /// </summary>
    public partial class ArchivePage : Page
    {
        Archive selectedItem = new Archive();
        public ArchivePage()
        {
            InitializeComponent();
            MainWindow.PageText.Text = "Архив заявок";
            Search_TXB.Text = "Поиск по любым параметрам";
            MainWindow._previousPage = Manager.MainFrame.Content;
           

            AppConnect.modelOdb = new RCCEntities();
            ArchiveRequests_DT.ItemsSource = AppConnect.modelOdb.Archive.ToList();
        }
        
        private void Search_TXB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = Search_TXB.Text;

            if (searchText != "Поиск по любым параметрам")
            {
                var filteredData = AppConnect.modelOdb.Archive.Where(x => x.WorkAllData.Contains(searchText)).ToList();
                ArchiveRequests_DT.ItemsSource = filteredData;
            }
        }

        private void ArchiveRequests_DT_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            selectedItem = ArchiveRequests_DT.SelectedItem as Archive;
            if (selectedItem != null)
            {
                if (AuthorizationPage.UserRoleName[0] == "Менеджер" || AuthorizationPage.UserRoleName[0] == "Администратор")
                    DownloadReport_BTN.Visibility = Visibility.Visible;

                ArchiveRequest_TXB.Visibility = Visibility.Visible;
                ArchiveRequest_TXB.Text = selectedItem.WorkAllData;
                ArchiveRequests_DT.Visibility = Visibility.Collapsed;
                Back_BTN.Visibility = Visibility.Visible;
            }
        }
        private void Back_BTN_Click(object sender, RoutedEventArgs e)
        {
            Back_BTN.Visibility = Visibility.Collapsed;
            ArchiveRequest_TXB.Visibility = Visibility.Collapsed;
            ArchiveRequests_DT.Visibility = Visibility.Visible;
            DownloadReport_BTN.Visibility = Visibility.Collapsed;
        }

        private void Search_TXB_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == string.Empty)
                (sender as TextBox).Text = "Поиск по любым параметрам";
        }

        private void Search_TXB_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Text = string.Empty;
        }

        void SaveReport()
        {
            var res2 = MessageBox.Show("Выберите путь для сохранения файла, а также задайте название сохраняемому файлу", "Выберите путь", MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (res2 == MessageBoxResult.Cancel)
                return;
            string filePath = "";
            try
            {

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Word Documents (*.docx, *.doc)|*.docx;*.doc";
                if (saveFileDialog.ShowDialog() == true)
                {
                    filePath = saveFileDialog.FileName;
                }

                var command = DataBaseClass.connectionOpen($"select Report from Archive where IdArchive={selectedItem.IdArchive}");
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    byte[] documentBytes = (byte[])reader["Report"];

                    File.WriteAllBytes($"{filePath}", documentBytes);
                }
                var res = MessageBox.Show("Файл успешно сохранён по заданному пути! Хотите открыть его?", "Файл сохранён", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    Process.Start($"{filePath}");
                }
            }
            catch
            {
                MessageBox.Show("Вы не выбрали путь для сохранения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DownloadReport_BTN_Click(object sender, RoutedEventArgs e)
        {
            SaveReport();
        }
    }
}
