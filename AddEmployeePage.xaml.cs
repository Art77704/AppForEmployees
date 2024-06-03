﻿using AppForEmployees.DataBase;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
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
    /// Логика взаимодействия для AddEmployeePage.xaml
    /// </summary>
    public partial class AddEmployeePage : Page
    {
        public AddEmployeePage()
        {
            InitializeComponent();
            MainWindow.PageText.Text = "Добавление сотрудника";
            Employee_DT.ItemsSource = AppConnect.modelOdb.Employee.ToList();

            if (MainMenuPage._RoleName == "Администратор")
            {
                var cmd = DataBaseClass.connectionOpen("select RoleName from Role");

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var NameRole = reader.GetValue(0).ToString();

                    Role_CMB.Items.Add($"{NameRole}");
                }
            }
            else
            {
                var cmd = DataBaseClass.connectionOpen("select RoleName from Role where RoleName!='Администратор'");

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var NameRole = reader.GetValue(0).ToString();

                    Role_CMB.Items.Add($"{NameRole}");
                }
            }
        }

        private void SaveData_BTN_Click(object sender, RoutedEventArgs e)
        {
            string[] textToCheck = { Login_TXB.Text, Password_TXB.Text, Surname_TXB.Text, FirstName_TXB.Text, Role_CMB.Text };

            if (MainMenuPage.CheckDataToEmpty(textToCheck))
                return;

            int idnewauth = 0;
            AppConnect.modelOdb = new RCCEntities();
            AuthorizationAcc AA = new AuthorizationAcc();
            {
                AA.AuthLogin = Login_TXB.Text;
                AA.AuthPassword = Password_TXB.Text;
                AA.AccIsValid = true;
                AA.IdRole = Role_CMB.SelectedIndex + 2;
            }
            AppConnect.modelOdb.AuthorizationAcc.Add(AA);
            MainWindow.SaveToBD();
            var cmd = DataBaseClass.connectionOpen($"select IdAuth from AuthorizationAcc where AuthLogin='{Login_TXB.Text}' and AuthPassword='{Password_TXB.Text}'");
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                idnewauth = int.Parse(reader.GetValue(0).ToString());
            }

            Employee em = new Employee();
            {
                em.Patronymic = Patronymic_TXB.Text;
                em.Surname = Surname_TXB.Text;
                em.FirstName=FirstName_TXB.Text;
                em.IdAuth = idnewauth;
            }; // Добавляет в БД  данные
            AppConnect.modelOdb.Employee.Add(em);
            MainWindow.SaveToBD();
            MessageBox.Show("Сотрудник добавлен в систему!", "Успешно!", MessageBoxButton.OK, MessageBoxImage.Information);
            Manager.MainFrame.Navigate(new PersonalAccountPage());
        }

        void DelData()
        {
            var res = MessageBox.Show("Вы действительно хотите удалить сотрудника(ов)?", "Удаление сотрудника(ов)", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                if (Employee_DT.SelectedItem != null)
                {
                    var selectedRow = Employee_DT.SelectedItems.Cast<Employee>().ToList();
                    var empl = Employee_DT.SelectedItem as Employee;

                    if (selectedRow != null)
                    {
                        DataBaseClass.AddEditDel($"delete EmployeeWorking where IdEmployee={empl.IdEmployee}");
                        AppConnect.modelOdb.Employee.RemoveRange(selectedRow);
                        AppConnect.modelOdb.SaveChanges();
                        Employee_DT.ItemsSource = AppConnect.modelOdb.Employee.ToList();
                        MessageBox.Show("Успешно!");
                    }
                    else
                        MessageBox.Show("Вы не выбрали сотрудника(ов) для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                else
                    MessageBox.Show("Вы не выбрали сотрудника(ов) для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Delete_BTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selempl = Employee_DT.SelectedItem as Employee;

                string[] RoleNameForDel=DataBaseClass.GetData($"select Role.RoleName from AuthorizationAcc, Role where AuthorizationAcc.IdAuth={selempl.IdAuth} and AuthorizationAcc.IdRole=Role.IdRole");
                string[] UserRoleName = DataBaseClass.GetData($"select Role.RoleName from AuthorizationAcc, Role where AuthorizationAcc.IdAuth={AuthorizationPage.UserAuthId} and AuthorizationAcc.IdRole=Role.IdRole");

                if (RoleNameForDel[0] == "Менеджер" && UserRoleName[0] == "Администратор")
                {
                    DelData();
                }
                else if (RoleNameForDel[0] == "Администратор" || RoleNameForDel[0] == "Менеджер")
                {
                    MessageBox.Show("Нет прав на удаление", "Нет прав", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else 
                {
                    DelData();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка :(", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
