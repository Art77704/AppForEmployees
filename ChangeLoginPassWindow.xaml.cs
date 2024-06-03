using AppForEmployees.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
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
using System.Windows.Shapes;

namespace AppForEmployees
{
    /// <summary>
    /// Логика взаимодействия для ChangeLoginPassWindow.xaml
    /// </summary>
    public partial class ChangeLoginPassWindow : Window
    {
        string _text;
        public ChangeLoginPassWindow(string text)
        {
            InitializeComponent();
            _text = text;
            if (text == "Login")
            {
                ChangePass_STP.Visibility = Visibility.Collapsed;
                this.Title = "Изменение логина";
            }
            else
            {
                ChangeLogin_STP.Visibility = Visibility.Collapsed;
                this.Title = "Изменение пароля";
            }

        }


        void ChangeLogin()
        {
            if (Login_TXB.Text.Length < 4)
            {
                MessageBox.Show("Минимальная длина логина - 4 символа", "Ненадежный логин", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DataBaseClass.AddEditDel($"update AuthorizationAcc set AuthLogin='{Login_TXB.Text}' where IdAuth={AuthorizationPage.UserAuthId}");
            MessageBox.Show("Введённые данные были успешно обновлены!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
            Manager.MainFrame.Navigate(new PersonalAccountPage());
        }

        void ChangePassword()
        {
            if (ConfirmNewPassword_TXB.Text != NewPassword_TXB.Text)
            {
                MessageBox.Show("Введённые пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (ConfirmNewPassword_TXB.Text.Length < 8)
            {
                MessageBox.Show("Минимальная длина пароля - 8 символов", "Ненадежный пароль", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DataBaseClass.AddEditDel($"update AuthorizationAcc set AuthPassword='{NewPassword_TXB.Text}' where IdAuth={AuthorizationPage.UserAuthId}");
            MessageBox.Show("Введённые данные были успешно обновлены!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
            Manager.MainFrame.Navigate(new PersonalAccountPage());

        }

        private void SaveData_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (_text == "Login")
                ChangeLogin();
            else
                ChangePassword();

        }
    }
}
