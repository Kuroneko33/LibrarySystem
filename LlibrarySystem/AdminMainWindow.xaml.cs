using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for AdminMainWindow.xaml
    /// </summary>
    public partial class AdminMainWindow : Window
    {
        SqlConnection sqlConnection;
        ObservableCollection<Librarian> Librarians { get; set; }
        int indexToEdit = 0;
        public AdminMainWindow()
        {
            string connectionString = Connection.connectionString;
            sqlConnection = new SqlConnection(connectionString);
            InitializeComponent();
            Librarians = new ObservableCollection<Librarian>();
            LibrariansList.ItemsSource = Librarians;
            LoadLibrarians();
        }

        private async void LoadLibrarians()
        {
            Librarians.Clear();
            await sqlConnection.OpenAsync();
            SqlDataReader dataReader = null;
            SqlCommand sqlCommandSELECT = new SqlCommand($"SELECT * From [Librarians]", sqlConnection);

            try
            {
                dataReader = await sqlCommandSELECT.ExecuteReaderAsync();
                while (await dataReader.ReadAsync())
                {
                    Librarians.Add(new Librarian() 
                    { 
                            Name = (String)(Convert.ToString(dataReader["Name"])).Trim(' '), 
                            Surname = (String)(Convert.ToString(dataReader["Surname"])).Trim(' '), 
                            Patronymic = (String)(Convert.ToString(dataReader["Patronymic"])).Trim(' '), 
                            Password = (String)(Convert.ToString(dataReader["Password"])).Trim(' '),
                    });
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
            sqlConnection.Close();
        }

        private void AddLibrarianButton_Click(object sender, RoutedEventArgs e)
        {
            EditLibrarianPanel.Visibility = Visibility.Visible;
        }

        private void EditLibrarianButton_Click(object sender, RoutedEventArgs e)
        {
            EditLibrarianPanel.Visibility = Visibility.Visible;
            indexToEdit = LibrariansList.SelectedIndex;
            Name.Text = Librarians[indexToEdit].Name;
            Surname.Text = Librarians[indexToEdit].Surname;
            Patronymic.Text = Librarians[indexToEdit].Patronymic;
            Password.Text = Librarians[indexToEdit].Password;
        }

        private async void DeleteLibrarianButton_Click(object sender, RoutedEventArgs e)
        {
            await sqlConnection.OpenAsync();

            try
            {
                SqlCommand sqlCommandDELETE = new SqlCommand($"DELETE FROM [Librarians] WHERE [Name]=@Name AND [Surname]=@Surname AND [Patronymic]=@Patronymic AND [Password]=@Password", sqlConnection);
                sqlCommandDELETE.Parameters.AddWithValue("Name", Librarians[indexToEdit].Name);
                sqlCommandDELETE.Parameters.AddWithValue("Surname", Librarians[indexToEdit].Surname);
                sqlCommandDELETE.Parameters.AddWithValue("Patronymic", Librarians[indexToEdit].Patronymic);
                sqlCommandDELETE.Parameters.AddWithValue("Password", Librarians[indexToEdit].Password);
                await sqlCommandDELETE.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
            }

            sqlConnection.Close();
            LoadLibrarians();
        }

        private async void SaveLibrarianButton_Click(object sender, RoutedEventArgs e)
        {
            await sqlConnection.OpenAsync();

            try
            {
                SqlCommand sqlCommandDELETE = new SqlCommand($"DELETE FROM [Librarians] WHERE [Name]=@Name AND [Surname]=@Surname AND [Patronymic]=@Patronymic AND [Password]=@Password", sqlConnection);
                sqlCommandDELETE.Parameters.AddWithValue("Name", Librarians[indexToEdit].Name);
                sqlCommandDELETE.Parameters.AddWithValue("Surname", Librarians[indexToEdit].Surname);
                sqlCommandDELETE.Parameters.AddWithValue("Patronymic", Librarians[indexToEdit].Patronymic);
                sqlCommandDELETE.Parameters.AddWithValue("Password", Librarians[indexToEdit].Password);
                await sqlCommandDELETE.ExecuteNonQueryAsync();
            }
            catch(Exception)
            {
            }

            SqlCommand sqlCommandINSERT = new SqlCommand("INSERT INTO [Librarians] (Name, Surname, Patronymic, Password) VALUES (@Name, @Surname, @Patronymic, @Password)", sqlConnection);
            sqlCommandINSERT.Parameters.AddWithValue("Name", Name.Text);
            sqlCommandINSERT.Parameters.AddWithValue("Surname", Surname.Text);
            sqlCommandINSERT.Parameters.AddWithValue("Patronymic", Patronymic.Text);
            sqlCommandINSERT.Parameters.AddWithValue("Password", Password.Text);
            await sqlCommandINSERT.ExecuteNonQueryAsync();

            Name.Text = "";
            Surname.Text = "";
            Patronymic.Text = "";
            Password.Text = "";

            sqlConnection.Close();
            LoadLibrarians();
            EditLibrarianPanel.Visibility = Visibility.Collapsed;
        }

        private void CancelLibrarianButton_Click(object sender, RoutedEventArgs e)
        {
            EditLibrarianPanel.Visibility = Visibility.Collapsed;
            Name.Text = "";
            Surname.Text = "";
            Patronymic.Text = "";
            Password.Text = "";
        }




        private void EditField_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (Name.Equals(textBox))
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
            else if (Surname.Equals(textBox))
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
            else if (Patronymic.Equals(textBox))
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
            else if (Password.Equals(textBox))
            {
                if (Password.Text.Length > 0)
                {
                    PasswordWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    PasswordWatermark.Visibility = Visibility.Visible;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            new Authorization().Show();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
