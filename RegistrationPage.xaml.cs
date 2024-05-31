using AppForEmployees.DataBase;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        private AuthorizationAcc _newUser = new AuthorizationAcc();
        public static AuthorizationAcc userObj;
        public static int IdNewUser;
        public static int _IdUser;
        public RegistrationPage(string text, int IdUser=0)
        {
            InitializeComponent();
            MainWindow.PageText.Text = "Регистрация";
            AppConnect.modelOdb = new RCCEntities();
            Role_CMB.FontSize = 18;
            Role_CMB.ItemsSource = AppConnect.modelOdb.Role.Where(r => r.RoleName != "Администратор").ToList();
            _IdUser = IdUser;
            MainWindow.GoBackBTN.Visibility = Visibility.Visible;
            DataContext = _newUser;
            if (text == "AddEmployee")
            {
                FIO_STP.Visibility = Visibility.Visible;
                Continue_BTN.Visibility = Visibility.Visible;
                Registration_BTN.Visibility = Visibility.Collapsed;
                Registration_STP.Visibility = Visibility.Collapsed;
            }
            else
            {
                FIO_STP.Visibility = Visibility.Collapsed;
                Continue_BTN.Visibility = Visibility.Collapsed;
                Registration_BTN.Visibility = Visibility.Visible;
                Registration_STP.Visibility = Visibility.Visible;
            }
        }

        void TestMet()
        {
            int a = 0;
            int b = a;
        }

        void AddEmployee(int IdUser)
        {
            AppConnect.modelOdb = new RCCEntities();
            Employee emp = new Employee();
               {
                   emp.FirstName = FirstName_TXB.Text;
                   emp.Surname = Surname_TXB.Text;
                   emp.Patronymic=Patronymic_TXB.Text;
                   emp.IdAuth = IdUser;
               }
            AppConnect.modelOdb.Employee.Add(emp);
            MainWindow.SaveToBD();
            Manager.MainFrame.Navigate(new AuthorizationPage("NewAcc"));
            Manager.MainFrame.Navigate(new MainMenuPage());
            MessageBox.Show($"{FirstName_TXB.Text} {Surname_TXB.Text} {Patronymic_TXB.Text}, добро пожаловать!", "Успешная регистрация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void Registration_BTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AppConnect.modelOdb = new RCCEntities();
                Role role=null;
                if (Role_CMB.SelectedItem != null)
                    role = Role_CMB.SelectedItem as Role;

                userObj = AppConnect.modelOdb.AuthorizationAcc.FirstOrDefault(x => x.AuthLogin == Login_TXB.Text);
                string[] textToCheck = { Login_TXB.Text, Password_PWB.Password};

                if (MainMenuPage.CheckDataToEmpty(textToCheck))
                    return;

                if (MainWindow.CheckBD(userObj, "Пользователь с таким логином уже существует!") == false)
                    return;

                if (PersonalAccountPage.CheckInput(Login_TXB, Password_PWB) == false)
                    return;

                AuthorizationAcc UA = new AuthorizationAcc();
                {
                    UA.AuthLogin = Login_TXB.Text;
                    UA.AuthPassword = Password_PWB.Password;
                    UA.AccIsValid = false;
                    UA.IdRole = role.IdRole;
                    IdNewUser = UA.IdAuth;

                }; // Добавляет в БД новому пользователю введённые данные
                AppConnect.modelOdb.AuthorizationAcc.Add(UA);
                MainWindow.SaveToBD();
               

                MessageBox.Show("Регистрация прошла успешна!\nДля авторизации в систему, пожалуйста, ожидайте подтверждение регистрации!", "Успешная регистрация", MessageBoxButton.OK, MessageBoxImage.Information);
                Manager.MainFrame.GoBack();
                MainWindow.GoBackBTN.Visibility = Visibility.Hidden;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Continue_BTN_Click(object sender, RoutedEventArgs e)
        {
            string[] textToCheck = { FirstName_TXB.Text , Surname_TXB.Text };

            if (!MainMenuPage.CheckDataToEmpty(textToCheck))
            {
                AddEmployee(_IdUser);
            }
        }
    }
}
