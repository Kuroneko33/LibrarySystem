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
        ObservableCollection<Book> Books { get; set; }
        int indexToEditL = 0;
        int indexToEditB = 0;
        int searchLengthOld = 0;
        public AdminMainWindow()
        {
            InitializeComponent();
            string connectionString = Connection.connectionString;
            sqlConnection = new SqlConnection(connectionString);
            Librarians = new ObservableCollection<Librarian>();
            LibrariansList.ItemsSource = Librarians;
            LoadLibrarians();
            Books = new ObservableCollection<Book>();
            BooksList.ItemsSource = Books;
            LoadBooks();
        }

        //////////////////////////////////////////////////////////////////////////
        //Операции с библиотекарями

        private void LoadLibrarians()
        {
            Librarians.Clear();
            sqlConnection.Open();
            SqlDataReader dataReader = null;
            SqlCommand sqlCommandSELECT = new SqlCommand($"SELECT * From [Librarians]", sqlConnection);

            try
            {
                dataReader = sqlCommandSELECT.ExecuteReader();
                while (dataReader.Read())
                {
                    Librarians.Add(new Librarian() 
                    { 
                            Name = (String)(Convert.ToString(dataReader["Name"])).Trim(' '), 
                            Surname = (String)(Convert.ToString(dataReader["Surname"])).Trim(' '), 
                            Patronymic = (String)(Convert.ToString(dataReader["Patronymic"])).Trim(' '), 
                            Password = (String)(Convert.ToString(dataReader["Password"])).Trim(' '),
                            Id = Convert.ToInt32(dataReader["Id"])
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
            EditBookPanel.Visibility = Visibility.Collapsed;
            EditLibrarianPanel.Visibility = Visibility.Visible;
            indexToEditL = -1;
            Name.Text = "";
            Surname.Text = "";
            Patronymic.Text = "";
            Password.Text = "";
        }

        private void EditLibrarianButton_Click(object sender, RoutedEventArgs e)
        {
            if (LibrariansList.SelectedIndex >= 0)
            {
                EditBookPanel.Visibility = Visibility.Collapsed;
                EditLibrarianPanel.Visibility = Visibility.Visible;
                indexToEditL = LibrariansList.SelectedIndex;
                Name.Text = Librarians[indexToEditL].Name;
                Surname.Text = Librarians[indexToEditL].Surname;
                Patronymic.Text = Librarians[indexToEditL].Patronymic;
                Password.Text = Librarians[indexToEditL].Password;
            }
        }

        private async void DeleteLibrarianButton_Click(object sender, RoutedEventArgs e)
        {
            indexToEditL = LibrariansList.SelectedIndex;
            if (MessageBox.Show($"Удалить библиотекаря '{Librarians[indexToEditL].Surname} {Librarians[indexToEditL].Name} {Librarians[indexToEditL].Patronymic}'?", "Удаление библиотекаря", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await sqlConnection.OpenAsync();

                try
                {
                    SqlCommand sqlCommandDELETE = new SqlCommand($"DELETE FROM [Librarians] WHERE [Id]=@Id", sqlConnection);
                    sqlCommandDELETE.Parameters.AddWithValue("Id", Librarians[indexToEditL].Id);
                    await sqlCommandDELETE.ExecuteNonQueryAsync();
                }
                catch (Exception)
                {
                }

                sqlConnection.Close();
                LoadLibrarians();
            }
        }

        private async void SaveLibrarianButton_Click(object sender, RoutedEventArgs e)
        {
            await sqlConnection.OpenAsync();

            if (indexToEditL != -1)
            {
                try
                {
                    SqlCommand sqlCommandUPDATE = new SqlCommand($"UPDATE [Librarians] SET [Name]=@Name, [Surname]=@Surname, [Patronymic]=@Patronymic, [Password]=@Password WHERE [Id]=@Id", sqlConnection);
                    sqlCommandUPDATE.Parameters.AddWithValue("Id", Librarians[indexToEditL].Id);
                    sqlCommandUPDATE.Parameters.AddWithValue("Name", Name.Text);
                    sqlCommandUPDATE.Parameters.AddWithValue("Surname", Surname.Text);
                    sqlCommandUPDATE.Parameters.AddWithValue("Patronymic", Patronymic.Text);
                    sqlCommandUPDATE.Parameters.AddWithValue("Password", Password.Text);
                    await sqlCommandUPDATE.ExecuteNonQueryAsync();
                }
                catch(Exception)
                {
                }
            }
            else
            {
                SqlCommand sqlCommandINSERT = new SqlCommand("INSERT INTO [Librarians] (Name, Surname, Patronymic, Password) VALUES (@Name, @Surname, @Patronymic, @Password)", sqlConnection);
                sqlCommandINSERT.Parameters.AddWithValue("Name", Name.Text);
                sqlCommandINSERT.Parameters.AddWithValue("Surname", Surname.Text);
                sqlCommandINSERT.Parameters.AddWithValue("Patronymic", Patronymic.Text);
                sqlCommandINSERT.Parameters.AddWithValue("Password", Password.Text);
                await sqlCommandINSERT.ExecuteNonQueryAsync();
            }

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

        //////////////////////////////////////////////////////////////////////////
        //Операции с книгами

        private void LoadBooks()
        {
            Books.Clear();
            sqlConnection.Open();
            SqlDataReader dataReader = null;
            SqlCommand sqlCommandSELECT = new SqlCommand($"SELECT * From [Books], [Authors] WHERE [Books].[Author_Id] = [Authors].[Id] ORDER BY [Surname], [Name], [Patronymic], [BookName]", sqlConnection);

            try
            {
                dataReader = sqlCommandSELECT.ExecuteReader();
                while (dataReader.Read())
                {
                    Books.Add(new Book()
                    {
                        Author = new Author() 
                        { 
                            Surname = (String)(Convert.ToString(dataReader["Surname"])).Trim(' '), 
                            Name = (String)(Convert.ToString(dataReader["Name"])).Trim(' '),
                            Patronymic = (String)(Convert.ToString(dataReader["Patronymic"])).Trim(' '),

                        },
                        BookName = (String)(Convert.ToString(dataReader["BookName"])).Trim(' '),
                        Publisher = (String)(Convert.ToString(dataReader["Publisher"])).Trim(' '),
                        PublicationDate = Convert.ToInt32(dataReader["PublicationDate"]),
                        PageCount = Convert.ToInt32(dataReader["PageCount"]),
                        Location = (String)(Convert.ToString(dataReader["Location"])).Trim(' '),
                        Id = Convert.ToInt32(dataReader["Id"]),
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

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            indexToEditB = -1;
            EditLibrarianPanel.Visibility = Visibility.Collapsed;
            EditBookPanel.Visibility = Visibility.Visible;
            AuthorName.Text = "";
            AuthorSurname.Text = "";
            AuthorPatronymic.Text = "";
            BookName.Text = "";
            Publisher.Text = "";
            PublicationDate.Text = "";
            PageCount.Text = "";
            Location.Text = "";
        }

        private void EditBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksList.SelectedIndex >= 0)
            {
                EditLibrarianPanel.Visibility = Visibility.Collapsed;
                EditBookPanel.Visibility = Visibility.Visible;
                indexToEditL = BooksList.SelectedIndex;
                AuthorSurname.Text = Books[indexToEditL].Author.Surname;
                AuthorName.Text = Books[indexToEditL].Author.Name;
                AuthorPatronymic.Text = Books[indexToEditL].Author.Patronymic;
                BookName.Text = Books[indexToEditL].BookName;
                Publisher.Text = Books[indexToEditL].Publisher;
                PublicationDate.Text = Convert.ToString(Books[indexToEditL].PublicationDate);
                PageCount.Text = Convert.ToString(Books[indexToEditL].PageCount);
                Location.Text = Books[indexToEditL].Location;
                indexToEditB = BooksList.SelectedIndex;
            }
        }

        private async void SaveBookButton_Click(object sender, RoutedEventArgs e)
        {
            await sqlConnection.OpenAsync();
            int Author_Id = 0; 
            int publicationDate = 0;
            Int32.TryParse(PublicationDate.Text, out publicationDate);
            int pageCount = 0;
            Int32.TryParse(PageCount.Text, out pageCount);
            try
            {
                SqlCommand sqlCommandSELECT = new SqlCommand($"SELECT [Id] FROM [Authors] WHERE [Name]=N'{Books[indexToEditB].Author.Name}' AND [Surname]=N'{Books[indexToEditB].Author.Surname}' AND [Patronymic]=N'{Books[indexToEditB].Author.Patronymic}'", sqlConnection);
                SqlDataReader dataReader = null;
                dataReader = sqlCommandSELECT.ExecuteReader();
                dataReader.Read();
                Author_Id = Convert.ToInt32(dataReader["Id"]);
                dataReader.Close();
            }
            catch (Exception)
            {
            }

            if (Author_Id == 0)
            {
                SqlCommand sqlCommandINSERTAuthors = new SqlCommand("INSERT INTO [Authors] (Name, Surname, Patronymic) VALUES (@Name, @Surname, @Patronymic)", sqlConnection);
                sqlCommandINSERTAuthors.Parameters.AddWithValue("Name", Name.Text);
                sqlCommandINSERTAuthors.Parameters.AddWithValue("Surname", Surname.Text);
                sqlCommandINSERTAuthors.Parameters.AddWithValue("Patronymic", Patronymic.Text);
                await sqlCommandINSERTAuthors.ExecuteNonQueryAsync();
                try
                {
                    SqlCommand sqlCommandSELECT = new SqlCommand($"SELECT [Id] From [Authors] WHERE [Name] = {Name.Text} AND [Surname] = {Surname.Text} Patronymic [Name] = {Patronymic.Text}", sqlConnection);
                    SqlDataReader dataReader = null;
                    dataReader = sqlCommandSELECT.ExecuteReader();
                    dataReader.Read();
                    Author_Id = Convert.ToInt32(dataReader["Id"]);
                    dataReader.Close();
                }
                catch (Exception)
                {
                }
            }
            if (indexToEditB != -1)
            {
                
                try
                {
                    SqlCommand sqlCommandUPDATE = new SqlCommand($"UPDATE [Books] SET [BookName]=@BookName, [Publisher]=@Publisher, [PublicationDate]=@PublicationDate, [PageCount]=@PageCount, [Location]=@Location WHERE [Id]=@Id", sqlConnection);
                    sqlCommandUPDATE.Parameters.AddWithValue("Id", Books[indexToEditB].Id);
                    sqlCommandUPDATE.Parameters.AddWithValue("Author_Id", Author_Id);
                    sqlCommandUPDATE.Parameters.AddWithValue("BookName", BookName.Text);
                    sqlCommandUPDATE.Parameters.AddWithValue("Publisher", Publisher.Text);
                    sqlCommandUPDATE.Parameters.AddWithValue("PublicationDate", publicationDate);
                    sqlCommandUPDATE.Parameters.AddWithValue("PageCount", pageCount);
                    sqlCommandUPDATE.Parameters.AddWithValue("Location", Location.Text);
                    await sqlCommandUPDATE.ExecuteNonQueryAsync();
                }
                catch (Exception)
                {
                }
            }
            else
            {
                SqlCommand sqlCommandINSERT = new SqlCommand("INSERT INTO [Books] (Author_Id, BookName, Publisher, PublicationDate, PageCount, Location) VALUES (@Author_Id, @BookName, @Publisher, @PublicationDate, @PageCount, @Location)", sqlConnection);
                sqlCommandINSERT.Parameters.AddWithValue("Author_Id", Author_Id);
                sqlCommandINSERT.Parameters.AddWithValue("BookName", BookName.Text);
                sqlCommandINSERT.Parameters.AddWithValue("Publisher", Publisher.Text);
                sqlCommandINSERT.Parameters.AddWithValue("PublicationDate", publicationDate);
                sqlCommandINSERT.Parameters.AddWithValue("PageCount", pageCount);
                sqlCommandINSERT.Parameters.AddWithValue("Location", Location.Text);
                await sqlCommandINSERT.ExecuteNonQueryAsync();
            }    

            AuthorName.Text = "";
            Surname.Text = "";
            Patronymic.Text = "";
            Password.Text = "";
            BookName.Text = "";
            Publisher.Text = "";
            PublicationDate.Text = "";
            PageCount.Text = "";
            Location.Text = "";

            sqlConnection.Close();
            LoadBooks();
            SearchUpdate();
            EditBookPanel.Visibility = Visibility.Collapsed;
        }

        private void SearchUpdate()
        {
            if (Search.Text.Length > 0)
            {
                if (Search.Text.Length < searchLengthOld)
                {
                    LoadBooks();
                }
                searchLengthOld = Search.Text.Length;
                SearchWatermark.Visibility = Visibility.Hidden;
                string search = Search.Text;
                for (int i = 0; i < Books.Count; i++)
                {
                    string str = "";
                    str += Books[i].Author.Name;
                    str += Books[i].Author.Surname;
                    str += Books[i].Author.Patronymic;
                    str += Books[i].BookName;
                    str += Books[i].Location;
                    str += Books[i].PageCount;
                    str += Books[i].PublicationDate;
                    str += Books[i].Publisher;
                    str += str.ToLower() + str.ToUpper();
                    int ind = str.IndexOf(search);
                    if (ind == -1)
                    {
                        Books.RemoveAt(i);
                        i--;
                    }
                }
            }
            else
            {
                LoadBooks();
                SearchWatermark.Visibility = Visibility.Visible;
            }
        }

        private void CancelBookButton_Click(object sender, RoutedEventArgs e)
        {
            EditBookPanel.Visibility = Visibility.Collapsed;
            AuthorName.Text = "";
            AuthorSurname.Text = "";
            AuthorPatronymic.Text = "";
            BookName.Text = "";
            Publisher.Text = "";
            PublicationDate.Text = "";
            PageCount.Text = "";
            Location.Text = "";
        }

        private async void DeleteBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Удалить книгу '{Books[BooksList.SelectedIndex].BookName}'?", "Удаление книги", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            { 
                await sqlConnection.OpenAsync();

                try
                {
                    SqlCommand sqlCommandDELETE = new SqlCommand($"DELETE FROM [Books] WHERE [Id]=@Id", sqlConnection);
                    sqlCommandDELETE.Parameters.AddWithValue("Id", Books[BooksList.SelectedIndex].Id);
                    await sqlCommandDELETE.ExecuteNonQueryAsync();
                }
                catch (Exception)
                {
                }

                sqlConnection.Close();
                LoadBooks();
                if (Search.Text.Length > 0)
                {
                    searchLengthOld = Search.Text.Length;
                    SearchWatermark.Visibility = Visibility.Hidden;
                    string search = Search.Text;
                    for (int i = 0; i < Books.Count; i++)
                    {
                        string str = "";
                        str += Books[i].Author.Name;
                        str += Books[i].Author.Surname;
                        str += Books[i].Author.Patronymic;
                        str += Books[i].BookName;
                        str += Books[i].Location;
                        str += Books[i].PageCount;
                        str += Books[i].PublicationDate;
                        str += Books[i].Publisher;
                        str += str.ToLower() + str.ToUpper();
                        int ind = str.IndexOf(search);
                        if (ind == -1)
                        {
                            Books.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////
        //Операции общего назначения

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
            else if (AuthorSurname.Equals(textBox))
            {
                if (AuthorSurname.Text.Length > 0)
                {
                    AuthorSurnameWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    AuthorSurnameWatermark.Visibility = Visibility.Visible;
                }
            }
            else if (AuthorName.Equals(textBox))
            {
                if (AuthorName.Text.Length > 0)
                {
                    AuthorNameWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    AuthorNameWatermark.Visibility = Visibility.Visible;
                }
            }
            else if (AuthorPatronymic.Equals(textBox))
            {
                if (AuthorPatronymic.Text.Length > 0)
                {
                    AuthorPatronymicWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    AuthorPatronymicWatermark.Visibility = Visibility.Visible;
                }
            }
            else if (BookName.Equals(textBox))
            {
                if (BookName.Text.Length > 0)
                {
                    BookNameWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    BookNameWatermark.Visibility = Visibility.Visible;
                }
            }
            else if (Publisher.Equals(textBox))
            {
                if (Publisher.Text.Length > 0)
                {
                    PublisherWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    PublisherWatermark.Visibility = Visibility.Visible;
                }
            }
            else if (PublicationDate.Equals(textBox))
            {
                if (PublicationDate.Text.Length > 0)
                {
                    PublicationDateWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    PublicationDateWatermark.Visibility = Visibility.Visible;
                }
            }
            else if (PageCount.Equals(textBox))
            {
                if (PageCount.Text.Length > 0)
                {
                    PageCountWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    PageCountWatermark.Visibility = Visibility.Visible;
                }
            }
            else if (Location.Equals(textBox))
            {
                if (Location.Text.Length > 0)
                {
                    LocationWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    LocationWatermark.Visibility = Visibility.Visible;
                }
            }
            else if (Search.Equals(textBox))
            {
                SearchUpdate();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            new Authorization().Show();
        }
    }
}
