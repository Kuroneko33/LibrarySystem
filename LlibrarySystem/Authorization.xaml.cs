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
using System.Windows.Shapes;

namespace LlibrarySystem
{
    /// <summary>
    /// Interaction logic for Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        SqlConnection sqlConnection;
        public Authorization()
        {
            InitializeComponent();
            string connectionString = Connection.connectionString;
            sqlConnection = new SqlConnection(connectionString);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (RBLibrarian.IsChecked == true && RBAdmin.IsChecked == false)
            {
                LoginGrid.Visibility = Visibility.Collapsed;
                SurnameGrid.Visibility = Visibility.Visible;
                NameGrid.Visibility = Visibility.Visible;
                PatronymicGrid.Visibility = Visibility.Visible;
            }
            else if (RBAdmin.IsChecked == true && RBLibrarian.IsChecked == false)
            {
                LoginGrid.Visibility = Visibility.Visible;
                SurnameGrid.Visibility = Visibility.Collapsed;
                NameGrid.Visibility = Visibility.Collapsed;
                PatronymicGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBlock = (TextBox)sender;
            if (textBlock.Equals(Login))
            {
                if (Login.Text.Length > 0)
                {
                    LoginWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    LoginWatermark.Visibility = Visibility.Visible;
                }
            }
            else if (textBlock.Equals(Name))
            {
                if (Name.Text.Length > 0)
                {
                    NameWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    NameWatermark.Visibility = Visibility.Visible;
                }
            }
            else if (textBlock.Equals(Surname))
            {
                if (Surname.Text.Length > 0)
                {
                    SurnameWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    SurnameWatermark.Visibility = Visibility.Visible;
                }
            }
            else if (textBlock.Equals(Patronymic))
            {
                if (Patronymic.Text.Length > 0)
                {
                    PatronymicWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    PatronymicWatermark.Visibility = Visibility.Visible;
                }
            }
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Password.Password.Length > 0)
            {
                PasswordWatermark.Visibility = Visibility.Hidden;
            }
            else
            {
                PasswordWatermark.Visibility = Visibility.Visible;
            }
        }

        private async void Enter_Click(object sender, RoutedEventArgs e)
        {
            if (RBLibrarian.IsChecked == true && RBAdmin.IsChecked == false)
            {

            }
            else if (RBAdmin.IsChecked == true && RBLibrarian.IsChecked == false)
            {
                bool authorization = false;

                await sqlConnection.OpenAsync();
                SqlDataReader dataReader = null;
                SqlCommand sqlCommandSELECT = new SqlCommand($"SELECT * From [Admins]", sqlConnection);

                try
                {
                    dataReader = await sqlCommandSELECT.ExecuteReaderAsync();
                    while (await dataReader.ReadAsync())
                    {
                        if (Login.Text.Equals((String)(Convert.ToString(dataReader["Login"])).Trim(' ')) && Password.Password.Equals((String)(Convert.ToString(dataReader["Password"])).Trim(' ')))
                        {
                            authorization = true;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    dataReader.Close();
                }

                if (authorization)
                {
                    new AdminMainWindow().Show();
                }
                else
                    MessageBox.Show("Введены неверные данные!", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                sqlConnection.Close();
                this.Close();
            }
        }
    }
}
