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
using AppForEmployees.DataBase;
using System.Runtime.Remoting.Messaging;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;

namespace AppForEmployees
{
    /// <summary>
    /// Логика взаимодействия для PersonalAccountPage.xaml
    /// </summary>
    public partial class PersonalAccountPage : Page
    {
        public static int IdNewUser;
        bool ShowOrHide = true;
        string _HidePass;
        string Pasw;
        public static string RoleName;
        
        public PersonalAccountPage()
        {
            InitializeComponent();
            MainWindow.PageText.Text = "Личный кабинет";
            MainWindow._previousPage = Manager.MainFrame.Content;
            RefreshDT();
           
            GetDataAcc();
            GetUserRole(ShowForManagAdm2_STP, ShowForManagAdm_STP, ForEmployee_STP);
        }

        void GetDataAcc()
        {
            var cmd = DataBaseClass.connectionOpen($"select AA.AuthPassword, AA.AuthLogin, emp.FirstName, emp.Surname, emp.Patronymic, r.RoleName from AuthorizationAcc AA, Employee emp, Role r where AA.IdAuth={AuthorizationPage.UserAuthId} and AA.IdAuth=emp.IdAuth and r.IdRole=aa.IdRole");
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Pasw = reader.GetValue(0).ToString() + " ";
                Login_TB.Text = reader.GetValue(1).ToString() + " ";
                FirstName_TB.Text = reader.GetValue(2).ToString();
                Surname_TB.Text = reader.GetValue(3).ToString();
                Patronymic_TB.Text = reader.GetValue(4).ToString();
                Role_TB.Text=reader.GetValue(5).ToString();
            }
            _HidePass = String.Concat(Enumerable.Repeat("*", Pasw.Length - 1));
            Password_TB.Text = _HidePass;
            //Отображение списка выполненных работ у работника
            DataBaseClass.ShowOrUpdateDT($"select WorkAllData as ВыполненнаяРабота from Archive where WorkAllData like '%{Surname_TB.Text} {FirstName_TB.Text} {Patronymic_TB.Text}%'", Archive_DT);
        }

        
        
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)Archive_DT.SelectedItem;
            if (selectedRow != null)
            {
                var text = Convert.ToString(selectedRow["ВыполненнаяРабота"]);

                WorkInfo_TXB.Visibility = Visibility.Visible;
                WorkInfo_TXB.Text = text;
                Back_BTN.Visibility = Visibility.Visible;
                Archive_DT.Visibility = Visibility.Collapsed;
            }
        }

        private void Back_BTN_Click(object sender, RoutedEventArgs e)
        {
            Back_BTN.Visibility=Visibility.Collapsed;
            WorkInfo_TXB.Visibility=Visibility.Collapsed;
            Archive_DT.Visibility=Visibility.Visible;
            GetDataAcc();
        }


        public static void GetUserRole(StackPanel stp, StackPanel stp2, StackPanel stp3)
        {
            var cmd2 = DataBaseClass.connectionOpen($"select a.IdRole, r.RoleName from Role r, AuthorizationAcc a where r.IdRole=a.IdRole and a.IdAuth={AuthorizationPage.UserAuthId}");
            SqlDataReader reader2 = cmd2.ExecuteReader();
            while (reader2.Read())
            {
                RoleName = reader2.GetValue(1).ToString();
            }

            if (RoleName == "Менеджер" || RoleName == "Администратор")
            {
                stp.Visibility = Visibility.Visible;
                stp2.Visibility = Visibility.Visible;
                stp3.Visibility = Visibility.Collapsed;
            }
            else
            {
                stp.Visibility = Visibility.Collapsed;
                stp2.Visibility = Visibility.Collapsed;
                stp3.Visibility = Visibility.Visible;
            }
        }

        public static bool CheckInput(TextBox Login_TXB, PasswordBox Password_TXB)
        {
            if (Login_TXB.Text.Length > 3 || Password_TXB.Password.Length > 7)
            {
                return true;
            }
            else
            {
                MessageBox.Show("Минимальная длина логина: 4 символа.\nМинимальная длина пароля: 8 символов.", "Ненадеждый логин / пароль", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
        }

        private void ChangeLogin_BTN_Click(object sender, RoutedEventArgs e)
        {
            ChangeLoginPassWindow w = new ChangeLoginPassWindow("Login");
            w.ShowDialog();
        }

        private void ChangePassword_BTN_Click(object sender, RoutedEventArgs e)
        {
            ChangeLoginPassWindow w = new ChangeLoginPassWindow("Password");
            w.ShowDialog();
        }

        private void ShowPassword_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (ShowOrHide)
            {
                Password_TB.Text = Pasw;
                ShowOrHide = false;
            }
            else
            {
                Password_TB.Text = _HidePass;
                ShowOrHide = true;
            }
        }

        private void Confirm_BTN_Click(object sender, RoutedEventArgs e)
        {
            var seladm = forManagerAdm_DT.SelectedItem as AuthorizationAcc;

            IdNewUser = seladm.IdAuth;
            string NewUserRole = seladm.Role.RoleName;
            if (AuthorizationPage.UserRoleName[0] == "Менеджер" && NewUserRole == "Менеджер")
            {
                MessageBox.Show("Нет прав на добавление данного сотрудника в систему!\nСотрудника с данной должностью может добавить только Администратор.", "Нет прав", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string sql = $"update AuthorizationAcc set AccIsValid=1 where IdAuth={IdNewUser}";
            DataBaseClass.AddEditDel(sql);
            MessageBox.Show("Пользователь добавлен в систему!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
            Manager.MainFrame.Navigate(new PersonalAccountPage());
        }

        private void DeleteRequest_BTN_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Вы действительно хотите отклонить заявку на регистрацию аккаунта?", "Отклонение регистрации", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                var seladm=forManagerAdm_DT.SelectedItem as AuthorizationAcc;
                IdNewUser = seladm.IdAuth;

                DataBaseClass.AddEditDel($"DELETE AuthorizationAcc where IdAuth={IdNewUser}");
                MessageBox.Show("Запрос на регистрацию аккаунта был отклонён!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                Manager.MainFrame.Navigate(new PersonalAccountPage());
            }
        }
        void RefreshDT()
        {
            var querryNewUser = from AuthorizationAcc in AppConnect.modelOdb.AuthorizationAcc
                                where AuthorizationAcc.AccIsValid == false
                                select AuthorizationAcc;

            forManagerAdm_DT.ItemsSource= querryNewUser.ToList();


            var querryFinishWork = from EmployeeWorking in AppConnect.modelOdb.EmployeeWorking
                                   where (EmployeeWorking.WorkInProcess == true && EmployeeWorking.WorkFinished == true)
                                   select EmployeeWorking;
            FinishWork_DT.ItemsSource = querryFinishWork.ToList();
        }

        private void Refresh_BTN_Click(object sender, RoutedEventArgs e)
        {
            RefreshDT();
        }

        private void AddEmployee_BTN_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEmployeePage());
        }
        EmployeeWorking emplwork;

        private void ConfirmWork_BTN_Click(object sender, RoutedEventArgs e)
        {
            string Surname = "";
            string FirstName = "";
            string Patronymic = "";
            string RoleWork = "";
            string CadastrN = "";
            string NumberOKS = "";
            string Address = "";
            string WorkDescription = "";
            string WorkerFIO = "";
            AppConnect.modelOdb = new RCCEntities();

            if (FinishWork_DT.SelectedItem!= null)
                emplwork = FinishWork_DT.SelectedItem as EmployeeWorking;
            
            int IdRequest = emplwork.IdRequest;

            string sql = $"update EmployeeWorking set WorkFinished=1, WorkInProcess=0 where IdRequest={IdRequest}";
            DataBaseClass.AddEditDel(sql);
           
            var cm = DataBaseClass.connectionOpen($"select e.Surname, e.FirstName, E.Patronymic from EmployeeWorking ew, Employee e where e.IdEmployee=ew.IdEmployee and ew.IdRequest={IdRequest}");
            SqlDataReader readr = cm.ExecuteReader();
            while (readr.Read()) 
            {
                WorkerFIO = readr.GetValue(0).ToString() + " " + readr.GetValue(1).ToString() + " " + readr.GetValue(2).ToString();
            }


            var cmd2 = DataBaseClass.connectionOpen($"(Select distinct req.IdRequest as НомерЗаявки, c.ClientSurname as ФамилияКлиента, c.ClientFirstName as ИмяКлиента, c.ClientPatronymic as ОтчествоКлиента, r.RoleName as ЗаявкаДля, req.WorkDescription as ОписаниеРаботы, req.CadastralNumber as КадастровыйНомер, req.NumberCapitalConstruction as НомерОКС, City.NameCity as Город, EstateAddress.EstateStreet as Улица,  EstateAddress.EstateHouse as Дом,  EstateAddress.EstateFlat as Квартира from Request req inner join Client c on req.IdClient = c.IdClient inner join Role r on req.IdRole = r.IdRole left join EstateAddress on req.IdAddress = EstateAddress.IdAddress left join City on EstateAddress.IdCity=City.IdCity where req.IdAddress is not null and req.IdRequest={IdRequest}) union (Select distinct req.IdRequest as НомерЗаявки, c.ClientSurname as ФамилияКлиента, c.ClientFirstName as ИмяКлиента, c.ClientPatronymic as ОтчествоКлиента, r.RoleName as ЗаявкаДля, req.WorkDescription as ОписаниеРаботы, req.CadastralNumber as КадастровыйНомер, req.NumberCapitalConstruction as НомерОКС, City.NameCity as Город, null as Улица, null as Дом, null as Квартира from Request req inner join Client c on req.IdClient = c.IdClient inner join Role r on req.IdRole = r.IdRole left join EstateAddress on req.IdAddress = EstateAddress.IdAddress left join City on EstateAddress.IdCity = City.IdCity where req.IdAddress is null and req.IdRequest={IdRequest})");
            SqlDataReader reader2 = cmd2.ExecuteReader();
            while (reader2.Read())
            {
                Surname = reader2.GetValue(1).ToString();
                FirstName = reader2.GetValue(2).ToString();
                Patronymic = reader2.GetValue(3).ToString();
                RoleWork = reader2.GetValue(4).ToString();
                WorkDescription = reader2.GetValue(5).ToString();
                CadastrN = reader2.GetValue(6).ToString();
                NumberOKS = reader2.GetValue(7).ToString();
                if (reader2.GetValue(11).ToString() == string.Empty)
                    Address = "г. " + reader2.GetValue(8).ToString() + ", ул. " + reader2.GetValue(9).ToString() + ", д. " + reader2.GetValue(10).ToString();
                else
                    Address = "г. " + reader2.GetValue(8).ToString() + ", ул. " + reader2.GetValue(9).ToString() + ", д. " + reader2.GetValue(10).ToString() + ", кв. " + reader2.GetValue(11).ToString();
            }
            string RequestToArchiveResult = "";

            if (NumberOKS == string.Empty)
                RequestToArchiveResult = $"Номер заявки: {IdRequest}. ФИО заказчика: {Surname} {FirstName} {Patronymic}. Работу выполнил: {WorkerFIO}, с должностью: {RoleWork}. Кадастровый номер: {CadastrN}. Адрес: {Address}. Описание работы: {WorkDescription}";
            else
                RequestToArchiveResult = $"Номер заявки: {IdRequest}. ФИО заказчика: {Surname} {FirstName} {Patronymic}. Работу выполнил: {WorkerFIO}, с должностью: {RoleWork}. Кадастровый номер: {CadastrN}. Номер ОКС: {NumberOKS}. Описание работы: {WorkDescription}";

            Archive ar = new Archive();
            {
                ar.WorkAllData = RequestToArchiveResult;
            }; // Добавляет в БД  данные
            AppConnect.modelOdb.Archive.Add(ar);
            MainWindow.SaveToBD();
           /* int IdArch=0;
            var cmd = DataBaseClass.connectionOpen("select * from Archive");
            SqlDataReader sqlDataReader = cmd.ExecuteReader();
            while (sqlDataReader.Read())
            {
                 IdArch=int.Parse(sqlDataReader.GetValue(0).ToString());
            }

            
            DataBaseClass.AddEditDel($"UPDATE Archive SET Report = {emplwork.Report.ToArray()} WHERE IdArchive={IdArch}");*/


            string sql7 = $"delete EmployeeWorking where IdRequest={IdRequest}\r\ndelete Request where IdRequest={IdRequest}";
            DataBaseClass.AddEditDel(sql7);

            MessageBox.Show("Заявка изменена на статус 'Выполнена' и была перемещена в архив!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
            Manager.MainFrame.Navigate(new PersonalAccountPage());

        }

        private void DenyWork_BTN_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Вы действительно хотите отклонить данную работу?", "Отклонение работы", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {

                if (FinishWork_DT.SelectedItem != null)
                    emplwork = FinishWork_DT.SelectedItem as EmployeeWorking;

                string sql = $"delete EmployeeWorking where IdRequest={emplwork.IdRequest}";
                DataBaseClass.AddEditDel(sql);
                MessageBox.Show("Заявка по данной работе отклонена!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
                Manager.MainFrame.Navigate(new PersonalAccountPage());
            }
        }

        private void FinishWork_DT_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FinishWork_DT.SelectedItem != null)
            {
                emplwork = FinishWork_DT.SelectedItem as EmployeeWorking;

                Manager.MainFrame.Navigate(new RequestPage(emplwork.IdRequest));
            }
        }

        void SaveReport()
        {
            var res2 = MessageBox.Show("Выберите путь для сохранения файла, а также задайте название сохраняемому файлу", "Выберите путь", MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (res2 == MessageBoxResult.Cancel)
                return;
            string filePath = "";
            try
            {
                if (FinishWork_DT.SelectedItem != null)
                {
                    if (FinishWork_DT.SelectedItems.Count == 1)
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.Filter = "Word Documents (*.docx, *.doc)|*.docx;*.doc";
                        if (saveFileDialog.ShowDialog() == true)
                        {
                            filePath = saveFileDialog.FileName;

                        }
                        if (FinishWork_DT.SelectedItem != null)
                            emplwork = FinishWork_DT.SelectedItem as EmployeeWorking;

                        int IdRequest = emplwork.IdRequest;

                        var command = DataBaseClass.connectionOpen($"select Report from EmployeeWorking where IdRequest={IdRequest}");
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
                }
            }
            catch
            {
                MessageBox.Show("Вы не выбрали путь для сохранения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckReport_BTN_Click(object sender, RoutedEventArgs e)
        {
            SaveReport();
        }
    }
}
