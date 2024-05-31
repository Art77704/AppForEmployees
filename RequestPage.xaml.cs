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
using Microsoft.Win32;

namespace AppForEmployees
{
    /// <summary>
    /// Логика взаимодействия для RequestPage.xaml
    /// </summary>
    public partial class RequestPage : Page
    {
        int IdRequest;
        public RequestPage()
        {
            InitializeComponent();
            MainWindow._MenuRCC.Visibility = Visibility.Collapsed;
            MainWindow.PageText.Text = "Информация о заявке";

            IdRequest = MainMenuPage.IdRequest;
            ShowData();
            CheckNameBTN();
        }

        void CheckNameBTN()
        {
            SqlConnection con = MainWindow.connectionOpen();
            SqlCommand cmd = new SqlCommand($"select ew.WorkInProcess, ew.WorkFinished from EmployeeWorking ew, Employee e where ew.IdRequest={IdRequest} and e.IdEmployee=ew.IdEmployee", con);
            //SqlCommand cmd = new SqlCommand($"select ew.WorkInProcess, ew.WorkFinished from EmployeeWorking ew, Employee e where ew.IdEmployee={IdEmployee} and e.IdEmployee=ew.IdEmployee", con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (Convert.ToBoolean(reader.GetValue(0).ToString()) == false)
                    GetWork_BTN.Content = "Начать работу";
                else if (Convert.ToBoolean(reader.GetValue(0).ToString()) == true)
                {
                    GetWork_BTN.Content = "Завершить работу";
                }
                else if (Convert.ToBoolean(reader.GetValue(1).ToString()) == true)
                    GetWork_BTN.Visibility = Visibility.Hidden;


            }
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

            SqlConnection con = MainWindow.connectionOpen();
            SqlCommand cmd = new SqlCommand($"(Select distinct req.IdRequest as НомерЗаявки, c.ClientSurname as ФамилияКлиента, c.ClientFirstName as ИмяКлиента, c.ClientPatronymic as ОтчествоКлиента, r.RoleName as ЗаявкаДля, req.WorkDescription as ОписаниеРаботы, req.CadastralNumber as КадастровыйНомер, req.NumberCapitalConstruction as НомерОКС, City.NameCity as Город, EstateAddress.EstateStreet as Улица,  EstateAddress.EstateHouse as Дом,  EstateAddress.EstateFlat as Квартира from Request req inner join Client c on req.IdClient = c.IdClient inner join Role r on req.IdRole = r.IdRole left join EstateAddress on req.IdAddress = EstateAddress.IdAddress left join City on EstateAddress.IdCity=City.IdCity where req.IdAddress is not null and req.IdRequest={IdRequest}) union (Select distinct req.IdRequest as НомерЗаявки, c.ClientSurname as ФамилияКлиента, c.ClientFirstName as ИмяКлиента, c.ClientPatronymic as ОтчествоКлиента, r.RoleName as ЗаявкаДля, req.WorkDescription as ОписаниеРаботы, req.CadastralNumber as КадастровыйНомер, req.NumberCapitalConstruction as НомерОКС, City.NameCity as Город, null as Улица, null as Дом, null as Квартира from Request req inner join Client c on req.IdClient = c.IdClient inner join Role r on req.IdRole = r.IdRole left join EstateAddress on req.IdAddress = EstateAddress.IdAddress left join City on EstateAddress.IdCity = City.IdCity where req.IdAddress is null and req.IdRequest={IdRequest})",con);
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
            string tempRoleName = "";
            SqlConnection con7 = MainWindow.connectionOpen();
            SqlCommand cmd7 = new SqlCommand($"select r.RoleName from Role r, AuthorizationAcc aa where aa.IdRole = r.IdRole and aa.IdAuth = {AuthorizationPage.UserAuthId}", con7);
            //SqlCommand cmd = new SqlCommand($"select ew.WorkInProcess, ew.WorkFinished from EmployeeWorking ew, Employee e where ew.IdEmployee={IdEmployee} and e.IdEmployee=ew.IdEmployee", con);
            SqlDataReader reader7 = cmd7.ExecuteReader();
            while (reader7.Read())
            {
                tempRoleName = reader7.GetValue(0).ToString();
            }
            
            if (Role_TXB.Text != tempRoleName) { MessageBox.Show("Данная работа предназначена не для Вас!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }

            int _idemp = 0;

            AppConnect.modelOdb = new RCCEntities();

            bool WorkStatus = false;
            bool WorkFinished = false;
            SqlConnection con = MainWindow.connectionOpen();
            SqlCommand cmd = new SqlCommand($"select ew.WorkInProcess, ew.WorkFinished from EmployeeWorking ew, Employee e where ew.IdRequest={IdRequest} and e.IdEmployee=ew.IdEmployee", con);
            //SqlCommand cmd = new SqlCommand($"select ew.WorkInProcess, ew.WorkFinished from EmployeeWorking ew, Employee e where ew.IdEmployee={IdEmployee} and e.IdEmployee=ew.IdEmployee", con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                WorkStatus = Convert.ToBoolean(reader.GetValue(0).ToString());
                WorkFinished = Convert.ToBoolean(reader.GetValue(1).ToString());

            }
            SqlConnection con2 = MainWindow.connectionOpen();
            SqlCommand cmd2 = new SqlCommand($"select ew.IdEmployee from EmployeeWorking ew, Employee e where ew.IdRequest={IdRequest} and e.IdEmployee=ew.IdEmployee", con2);
            SqlDataReader reader2 = cmd2.ExecuteReader();
            while (reader2.Read())
            {
                _idemp = Convert.ToInt32(reader2.GetValue(0).ToString());
            }

            if (!WorkStatus && !WorkFinished)
            {


                EmployeeWorking ew = new EmployeeWorking();
                {
                    ew.WorkInProcess = true;
                    ew.WorkFinished = false;
                    ew.IdEmployee = MainMenuPage.IdEmployee;
                    ew.IdRequest = IdRequest;

                }; // Добавляет в БД  данные
                AppConnect.modelOdb.EmployeeWorking.Add(ew);
                AppConnect.modelOdb.SaveChanges();
                //MainWindow.SaveToBD();
                MessageBox.Show("Вы успешно взялись за работу по данной заявке!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                Manager.MainFrame.Navigate(new RequestPage());
            }
            else if (WorkStatus && _idemp != MainMenuPage.IdEmployee)
            {
                MessageBox.Show("По данной заявке уже работает другой сотрудник!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else if (WorkFinished && MainMenuPage.IdEmployee != _idemp)
            {
                MessageBox.Show("Работу по данной заявке уже завершил другой сотрудник!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            if (WorkStatus && MainMenuPage.IdEmployee == _idemp)
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
                        SqlConnection con5 = MainWindow.connectionOpen();

                        string sql5 = $"update EmployeeWorking set WorkInProcess=1, WorkFinished=1 where IdRequest={IdRequest}";

                        SqlCommand command5 = new SqlCommand(sql5, con5);
                        command5.ExecuteNonQuery();


                        var constr = MainWindow.connectionOpen();
                        var cmd5 = new SqlCommand($"UPDATE EmployeeWorking SET Report = BulkColumn FROM OPENROWSET(BULK '{filePath}', SINGLE_BLOB) AS Document WHERE IdRequest={IdRequest}", constr);

                        cmd5.ExecuteNonQuery();
                        MessageBox.Show("Вы успешно сдали работу! Ожидайте, пожалуйста, подтверждение выполненной работы от Менеджера!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);

                    }

                }
                catch
                {
                    
                }
                
            }
            else if (WorkFinished && MainMenuPage.IdEmployee == _idemp)
            {
                MessageBox.Show("Вы уже выполнили данную работу! Пожалуйста, ожидайте проверку вашей работы от Менеджера!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
    }
}
