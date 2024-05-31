﻿using AppForEmployees.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для ArchivePage.xaml
    /// </summary>
    public partial class ArchivePage : Page
    {
        public ArchivePage()
        {
            InitializeComponent();
            MainWindow.PageText.Text = "Архив заявок";

            AppConnect.modelOdb = new RCCEntities();
            ArchiveRequests_DT.ItemsSource = AppConnect.modelOdb.Archive.ToList();

        }


        

        private void Search_TXB_TextChanged(object sender, TextChangedEventArgs e)
        {
                // Получаем текст из TextBox
            string searchText = Search_TXB.Text;
            // Фильтруем данные в DataGrid
            var filteredData = AppConnect.modelOdb.Archive.Where(x => x.WorkAllData.Contains(searchText)).ToList();
            // Обновляем ItemsSource DataGrid
            ArchiveRequests_DT.ItemsSource = filteredData;
        }

        private void ArchiveRequests_DT_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = ArchiveRequests_DT.SelectedItem as Archive;
            //DataRowView row = ArchiveRequests_DT.SelectedItem as DataRowView;
            if (selectedItem != null)
            {
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
        }
    }
}
