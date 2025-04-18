﻿using AppForEmployees.DataBase;
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
using Microsoft.Win32;

namespace AppForEmployees
{
    /// <summary>
    /// Логика взаимодействия для RequestPage.xaml
    /// </summary>
    public partial class RequestPage : Page
    {
        int IdRequest;
        int IdEmployeeInRequest;
        int _IdEmpNow;

        public RequestPage(int IdReq=-1, Request req=null)
        {
            InitializeComponent();
            MainWindow._MenuRCC.Visibility = Visibility.Collapsed;
            MainWindow.PageText.Text = "Информация о заявке";
            MainWindow._previousPage = Manager.MainFrame.Content;

          

            if (req != null)
            {
                if (req.Employee != null)
                {
                    Employee_TXB.Text = $"{req.Employee.Surname} {req.Employee.FirstName} {req.Employee.Patronymic}";
                    IdEmployeeInRequest = req.Employee.IdEmployee;
                }
            }

            IdRequest_TB.Text = "Заявка - №";
            IdRequest_TB.Text += IdReq.ToString();
            IdRequest = IdReq;
            //IdRequest = MainMenuPage.IdRequest;
            ShowData();
            CheckNameBTN();
        }

        void CheckNameBTN()
        {
            var cmd = DataBaseClass.connectionOpen($"select e.IdEmployee from Employee e, " +
                $"AuthorizationAcc aa where aa.IdAuth=e.IdAuth and e.IdAuth={AuthorizationPage.UserAuthId}");
            SqlDataReader readr = cmd.ExecuteReader();
            while (readr.Read())
            {
                _IdEmpNow = Convert.ToInt32(readr.GetValue(0).ToString());
            }

            if (IdEmployeeInRequest != _IdEmpNow)
                GetWork_BTN.Visibility = Visibility.Hidden;
        }

        void HideAndShow(string text="")
        {
            if (text == "HideAddress")
            {
                Address_TB.Visibility = Visibility.Collapsed;
                Address_TXB.Visibility = Visibility.Collapsed;

                NumberOKS_TB.Visibility = Visibility.Visible;
                NumberOKS_TXB.Visibility = Visibility.Visible;
            }
            else
            {
                NumberOKS_TB.Visibility = Visibility.Collapsed;
                NumberOKS_TXB.Visibility = Visibility.Collapsed;
                Address_TB.Visibility = Visibility.Visible;
                Address_TXB.Visibility = Visibility.Visible;
            }
        }


        void ShowData()
        {

            var cmd = DataBaseClass.connectionOpen($"(Select distinct req.IdRequest as НомерЗаявки, c.ClientSurname as ФамилияКлиента, c.ClientFirstName as ИмяКлиента, c.ClientPatronymic as ОтчествоКлиента, r.RoleName as ЗаявкаДля, req.WorkDescription as ОписаниеРаботы, req.CadastralNumber as КадастровыйНомер, req.NumberCapitalConstruction as НомерОКС, City.NameCity as Город, EstateAddress.EstateStreet as Улица,  EstateAddress.EstateHouse as Дом,  EstateAddress.EstateFlat as Квартира from Request req inner join Client c on req.IdClient = c.IdClient inner join Role r on req.IdRole = r.IdRole left join EstateAddress on req.IdAddress = EstateAddress.IdAddress left join City on EstateAddress.IdCity=City.IdCity where req.IdAddress is not null and req.IdRequest={IdRequest}) union (Select distinct req.IdRequest as НомерЗаявки, c.ClientSurname as ФамилияКлиента, c.ClientFirstName as ИмяКлиента, c.ClientPatronymic as ОтчествоКлиента, r.RoleName as ЗаявкаДля, req.WorkDescription as ОписаниеРаботы, req.CadastralNumber as КадастровыйНомер, req.NumberCapitalConstruction as НомерОКС, City.NameCity as Город, null as Улица, null as Дом, null as Квартира from Request req inner join Client c on req.IdClient = c.IdClient inner join Role r on req.IdRole = r.IdRole left join EstateAddress on req.IdAddress = EstateAddress.IdAddress left join City on EstateAddress.IdCity = City.IdCity where req.IdAddress is null and req.IdRequest={IdRequest})");
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Surname_TXB.Text = reader.GetValue(1).ToString();
                FirstName_TXB.Text = reader.GetValue(2).ToString();
                Patronymic_TXB.Text = reader.GetValue(3).ToString();
                Role_TXB.Text = reader.GetValue(4).ToString();
                WorkDescription_TXB.Text = reader.GetValue(5).ToString();
                CadastralNumber_TXB.Text = reader.GetValue(6).ToString();
                NumberOKS_TXB.Text = reader.GetValue(7).ToString();
                if (reader.GetValue(11).ToString() == string.Empty)
                    Address_TXB.Text = "г. " + reader.GetValue(8).ToString() + ", ул. " + reader.GetValue(9).ToString() + ", д. " + reader.GetValue(10).ToString();
                else
                    Address_TXB.Text = "г. " + reader.GetValue(8).ToString() + ", ул. " + reader.GetValue(9).ToString() + ", д. " + reader.GetValue(10).ToString() + ", кв. " + reader.GetValue(11).ToString();

            }
            if (NumberOKS_TXB.Text != string.Empty)
            {
                HideAndShow("HideAddress");
            }
            else if (NumberOKS_TXB.Text == string.Empty)
            {
                HideAndShow();

            }
        }


        private void GetWork_BTN_Click(object sender, RoutedEventArgs e)
        {
           
           
            if (IdEmployeeInRequest == _IdEmpNow)
            {
                try
                {
                    MessageBox.Show("Выберите файл с отчётом", "Выберите файл", MessageBoxButton.OK, MessageBoxImage.Information );
                    string filePath = null;
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "Word Documents (*.docx, *.doc)|*.docx;*.doc";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        filePath = openFileDialog.FileName;
                    }
                    if (filePath != null)
                    {
                        string sql5 = $"update EmployeeWorking set WorkInProcess=1, WorkFinished=1 where IdRequest={IdRequest}";
                        DataBaseClass.AddEditDel(sql5);

                        sql5 = $"UPDATE EmployeeWorking SET Report = BulkColumn FROM OPENROWSET(BULK '{filePath}', SINGLE_BLOB) AS Document WHERE IdRequest={IdRequest}";
                        DataBaseClass.AddEditDel(sql5);
                        MessageBox.Show("Вы успешно сдали работу! Ожидайте, пожалуйста, подтверждение выполненной работы от Менеджера!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                        MessageBox.Show("Работа не сдана. Вы не выбрали файл с отчётом.\nДля сдачи работы повторно нажмите \"Завершить работу\" и выберите файл с отчётом!", "Файл не выбран", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch
                {
                    MessageBox.Show("Работа не сдана. Вы не выбрали файл с отчётом!", "Файл не выбран", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
            }
        }
    }
}
