using AppForEmployees.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppForEmployees
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Button GoBackBTN;
        public static MainWindow Window;
        public static TextBlock PageText;
        public static Menu _MenuRCC;
        public static MenuItem[] menuItems ;
        
        public MainWindow()
        {
            InitializeComponent();
            Window = this;
            menuItems = new MenuItem[]{ AllEmployee_BTN, AllClient_BTN, AllAddress_BTN};
            _MenuRCC = MenuRCC_Menu;
            Manager.MainFrame = MainFrame;
            PageText = PageText_TB;
            GoBackBTN = GoBack_BTN;
            MainFrame.Content = new AuthorizationPage();
        }
        private void MovingWin(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton==MouseButtonState.Pressed)
            {
                MainWindow.Window.DragMove();
            }
        }

        public static void HideButton(MenuItem[] buttons=null)
        {
            foreach (MenuItem button in buttons)
            {
                button.Visibility = Visibility.Collapsed;
            }
        }
       
        

        private void GoBack_BTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentpage = Manager.MainFrame.Content;
                if (MainMenuPage._currentpage == null && currentpage.ToString() == "AppForEmployees.AddClientPage")
                {
                    Manager.MainFrame.Navigate(new AddRequestPage());
                }
                else if (AddRequestPage._currentpage == null && currentpage.ToString() == "AppForEmployees.AddClientPage")
                    Manager.MainFrame.Navigate(new AllClientsPage());

                else if (currentpage.ToString() == "AppForEmployees.AddRequestPage")
                {
                    Manager.MainFrame.Navigate(new MainMenuPage());
                    AddRequestPage._currentpage = null;
                }
                else if (currentpage.ToString() == "AppForEmployees.AddAddressPage")
                {
                    Manager.MainFrame.Navigate(new AddRequestPage());
                }
                else if (currentpage.ToString() == "AppForEmployees.RequestPage")
                    Manager.MainFrame.Navigate(new MainMenuPage());
                else if (currentpage.ToString() == "AppForEmployees.AddClientPage")
                {
                    Manager.MainFrame.Navigate(new AddRequestPage());
                }
                else if (currentpage.ToString() == "AppForEmployees.PersonalAccountPage")
                    Manager.MainFrame.Navigate(new MainMenuPage());
                else if (currentpage.ToString() == "AppForEmployees.ArchivePage")
                    Manager.MainFrame.Navigate(new MainMenuPage());
                else if (currentpage.ToString() == "AppForEmployees.AllClientsPage")
                {
                    if (AllClientsPage.GoToAddRequestPage)
                        Manager.MainFrame.Navigate(new AddRequestPage());
                    else
                        Manager.MainFrame.Navigate(new MainMenuPage());
                }
                else if (currentpage.ToString() == "AppForEmployees.RegistrationPage")
                {
                    Manager.MainFrame.Navigate(new AuthorizationPage());
                    GoBack_BTN.Visibility = Visibility.Hidden;
                }
                else if (currentpage.ToString() == "AppForEmployees.AddEmployeePage")
                {
                    Manager.MainFrame.Navigate(new PersonalAccountPage());

                }
                else if (currentpage.ToString() == "AppForEmployees.MainMenuPage")
                {
                    MainMenuPage._currentpage = null;
                    Manager.MainFrame.Navigate(new AuthorizationPage());
                    GoBack_BTN.Visibility = Visibility.Hidden;
                }
                else
                    Manager.MainFrame.GoBack();
            }
            catch
            {
                GoBackBTN.Visibility = Visibility.Hidden;
            }
           
        }

        public static bool CheckBD(AuthorizationAcc userObj, string errorMessage, bool inversion = false)
        {
            AppConnect.modelOdb = new RCCEntities();

            try
            { // Проверяет, нет ли в БД уже такого пользователя с таким логином/почтой
                if (!inversion) // Если нужно, чтобы объект из БД был null
                {
                    if (userObj == null)
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                else
                {
                    if (userObj == null)
                    {
                        MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public static void SaveToBD(string resultMessage="")
        {//Сохранение проделанных действий в БД
            try
            {
                AppConnect.modelOdb.SaveChanges();
                //MessageBox.Show(resultMessage, "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseWin_BTN_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Wrap_BTN_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void Request_BTN_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new MainMenuPage());
        }

        private void AllClient_BTN_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AllClientsPage());
        }

        private void AddEmployee_BTN_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEmployeePage());
        }

        private void AllEmployee_BTN_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEmployeePage());
        }

        private void PersonalAcc_BTN_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new PersonalAccountPage());
        }

        private void Exit_BTN_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AuthorizationPage());
        }

        private void AllAddress_BTN_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddAddressPage());
        }

        private void Archive_BTN_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ArchivePage());

        }
       
    }
}
