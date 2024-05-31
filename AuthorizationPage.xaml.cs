using AppForEmployees.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        public static AuthorizationAcc userObj;
        public static int UserAuthId;
        public static int IdEmployee;
        public AuthorizationPage(string text = "5")
        {
            InitializeComponent();
            MainWindow.PageText.Text = "Авторизация";
            MainWindow._MenuRCC.Visibility = Visibility.Collapsed;
            MainWindow.GoBackBTN.Visibility = Visibility.Collapsed;
            if (text == "NewAcc")
            {
                var cmd = DataBaseClass.connectionOpen($"select AuthPassword, AuthLogin from AuthorizationAcc where IdAuth={RegistrationPage._IdUser}");
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Password_PWB.Password = reader.GetValue(0).ToString();
                    LogIn_TXB.Text = reader.GetValue(1).ToString();
                }
                Auth();
                // MainWindow.GoBackBTN.Visibility = Visibility.Hidden;
            }
        }

            private void LogIn_BTN_Click(object sender, RoutedEventArgs e) => Auth();

            private void Reg_BTN_Click(object sender, RoutedEventArgs e)
            {
                Manager.MainFrame.Navigate(new RegistrationPage(""));

            }

            void Auth()
            {
                string[] textToCheck = { LogIn_TXB.Text, Password_PWB.Password };
                if (MainMenuPage.CheckDataToEmpty(textToCheck))
                    return;

                var cmd = DataBaseClass.connectionOpen($"select AA.IdAuth from AuthorizationAcc AA, Employee em where AA.IdAuth=em.IdAuth");
                SqlDataReader reader = cmd.ExecuteReader();
                bool temp = false;
            

                 AppConnect.modelOdb = new RCCEntities();
                try
                { // Сверяет введённые данные с теми, которые имеются в БД
                    userObj = AppConnect.modelOdb.AuthorizationAcc.FirstOrDefault(x => x.AuthLogin == LogIn_TXB.Text && x.AuthPassword == Password_PWB.Password);
                    if (MainWindow.CheckBD(userObj, "Такого пользователя нет!", true))
                    {// Если такой пользователь есть, то перейдёт в каталог, иначе выдаст ошибку
                        if (userObj.AccIsValid == false)
                            MessageBox.Show("Для авторизации в систему необходимо дождаться подтверждения регистрации аккаунта от Менеджера (либо Администратора)", "Ожидайте подтверждения", MessageBoxButton.OK, MessageBoxImage.Error);
                        else
                        {
                            while (reader.Read())
                            {

                                if (userObj.IdAuth == int.Parse(reader.GetValue(0).ToString()))
                                {
                                    UserAuthId = userObj.IdAuth;

                                Manager.MainFrame.Navigate(new MainMenuPage());

                                    temp = true;
                                }
                            }
                            if (!temp)
                            {

                            MessageBox.Show("Для продолжения, пожалуйста, введите свои данные ФИО", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                                Manager.MainFrame.Navigate(new RegistrationPage("AddEmployee", userObj.IdAuth));
                            }

                        }
                    }
               
            }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }


        

