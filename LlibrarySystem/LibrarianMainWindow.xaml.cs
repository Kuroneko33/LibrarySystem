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
    /// Interaction logic for LibrarianMainWindow.xaml
    /// </summary>
    public partial class LibrarianMainWindow : Window
    {
        SqlConnection sqlConnection; 
        ObservableCollection<Abonement> Abonements { get; set; }
        ObservableCollection<Book> Books { get; set; }
        ObservableCollection<Book> BooksOnAbonement { get; set; }
        int indexToReloadA = 0;
        int indexToEditA = 0;
        int indexToEditB = 0;
        int searchLengthOldA = 0;
        int searchLengthOldB = 0;
        int Librarian_Id = 0;
        public LibrarianMainWindow(int Librarian_Id)
        {
            InitializeComponent();
            string connectionString = Connection.connectionString;
            sqlConnection = new SqlConnection(connectionString);
            Books = new ObservableCollection<Book>();
            BooksList.ItemsSource = Books;
            LoadBooks();
            Abonements = new ObservableCollection<Abonement>();
            AbonementsList.ItemsSource = Abonements;
            LoadAbonements();
            BooksOnAbonement = new ObservableCollection<Book>();
            AbonementBooksList.ItemsSource = BooksOnAbonement;
            this.Librarian_Id = Librarian_Id;
        }

        //////////////////////////////////////////////////////////////////////////
        //Операции с абонементами

        private void LoadAbonements()
        {
            Abonements.Clear();
            sqlConnection.Open();
            SqlDataReader dataReader = null;
            SqlCommand sqlCommandSELECT = new SqlCommand($"SELECT * From [Abonements], [Passports] WHERE [Abonements].[Passport_Id] = [Passports].[Id] ORDER BY [Surname]", sqlConnection);

            try
            {
                dataReader = sqlCommandSELECT.ExecuteReader();
                while (dataReader.Read())
                {
                    Abonements.Add(new Abonement()
                    {
                        Passport = new Passport()
                        {
                            Country = (String)(Convert.ToString(dataReader["Country"])).Trim(' '),
                        },
                        Surname = (String)(Convert.ToString(dataReader["Surname"])).Trim(' '),
                        Name = (String)(Convert.ToString(dataReader["Name"])).Trim(' '),
                        Patronymic = (String)(Convert.ToString(dataReader["Patronymic"])).Trim(' '),
                        Address = (String)(Convert.ToString(dataReader["Address"])).Trim(' '),
                        ContactPhoneNumber = (String)(Convert.ToString(dataReader["ContactPhoneNumber"])).Trim(' '),
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

        private void AddAbonementButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditAbonementButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteAbonementButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReportAbonementButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AbonementsList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (AbonementsList.SelectedIndex >= 0)
            {
                indexToReloadA = AbonementsList.SelectedIndex;
                EditAbonementPanel.Visibility = Visibility.Visible;
                string str = "";
                str += Abonements[AbonementsList.SelectedIndex].Surname + " ";
                str += Abonements[AbonementsList.SelectedIndex].Name + " ";
                str += Abonements[AbonementsList.SelectedIndex].Patronymic + "\n";
                str += Abonements[AbonementsList.SelectedIndex].Passport.Country + "\n";
                str += Abonements[AbonementsList.SelectedIndex].Address + "\n";
                str += Abonements[AbonementsList.SelectedIndex].ContactPhoneNumber;
                AbonementTextBlock.Text = str;
                LoadBooksToAbonement(Abonements[indexToReloadA].Id);
            }
        }

        //////////////////////////////////////////////////////////////////////////
        //Операции с книгами на абонементе

        private void LoadBooksToAbonement(int Id)
        {
            BooksOnAbonement.Clear();
            sqlConnection.Open();
            SqlDataReader dataReader = null;
            SqlCommand sqlCommandSELECT = new SqlCommand($"SELECT * From [Books], [Authors] WHERE [Books].[Author_Id] = [Authors].[Id] ORDER BY [Surname], [Name], [Patronymic], [BookName]", sqlConnection);

            try
            {
                dataReader = sqlCommandSELECT.ExecuteReader();
                while (dataReader.Read())
                {
                    string locationStr = (String)(Convert.ToString(dataReader["Location"])).Trim(' ');
                    int locationInt = 0;
                    if (Int32.TryParse(locationStr, out locationInt))
                    {
                        if (locationInt == Id)
                        {
                            BooksOnAbonement.Add(new Book()
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
                            });
                        }
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
            sqlConnection.Close();
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
                    string locationStr = (String)(Convert.ToString(dataReader["Location"])).Trim(' ');
                    int locationInt = 0;
                    if (!Int32.TryParse(locationStr, out locationInt))
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
                        });
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
            sqlConnection.Close();
        }

        private async void IssueBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditAbonementPanel.Visibility == Visibility.Visible && BooksList.SelectedIndex >=0)
            {
                DateTime dateTime = DateTime.Now;
                MessageBox.Show($"Зарегистрирован акт выдачи книги:\n" +
                    $"'{Books[BooksList.SelectedIndex].BookName}'\n" +
                    $"На абонемент:\n" +
                    $"{Abonements[indexToReloadA].Surname} {Abonements[indexToReloadA].Name} {Abonements[indexToReloadA].Patronymic}\n" +
                    $"Дата и время выдачи: {dateTime}", "Выдача книги", MessageBoxButton.OK, MessageBoxImage.Question);

                await sqlConnection.OpenAsync();
                SqlDataReader dataReader = null;
                SqlCommand sqlCommandSELECT = new SqlCommand($"SELECT [Id] From [Books] WHERE [BookName]=N'{Books[BooksList.SelectedIndex].BookName}' AND [Publisher]=N'{Books[BooksList.SelectedIndex].Publisher}' AND [PublicationDate]=N'{Books[BooksList.SelectedIndex].PublicationDate}' AND [PageCount]=N'{Books[BooksList.SelectedIndex].PageCount}'", sqlConnection);
                dataReader = sqlCommandSELECT.ExecuteReader();
                await dataReader.ReadAsync();
                int Book_Id = Convert.ToInt32(dataReader["Id"]);
                dataReader.Close();

                SqlCommand sqlCommandINSERT = new SqlCommand("INSERT INTO [Operations] (Date, Abonement_Id, Book_Id, Librarian_Id, Operation) VALUES (@Date, @Abonement_Id, @Book_Id, @Librarian_Id, @Operation)", sqlConnection);
                sqlCommandINSERT.Parameters.AddWithValue("Date", dateTime);
                sqlCommandINSERT.Parameters.AddWithValue("Abonement_Id", Abonements[indexToReloadA].Id);
                sqlCommandINSERT.Parameters.AddWithValue("Book_Id", Book_Id);
                sqlCommandINSERT.Parameters.AddWithValue("Librarian_Id", Librarian_Id);
                sqlCommandINSERT.Parameters.AddWithValue("Operation", "Акт выдачи");
                await sqlCommandINSERT.ExecuteNonQueryAsync();


                SqlCommand sqlCommandUPDATE = new SqlCommand($"UPDATE [Books] SET [Location]=@Location WHERE [BookName]=@BookName AND [Publisher]=@Publisher AND [PublicationDate]=@PublicationDate AND [PageCount]=@PageCount", sqlConnection);
                sqlCommandUPDATE.Parameters.AddWithValue("Location", Abonements[indexToReloadA].Id.ToString());
                sqlCommandUPDATE.Parameters.AddWithValue("BookName", Books[BooksList.SelectedIndex].BookName);
                sqlCommandUPDATE.Parameters.AddWithValue("Publisher", Books[BooksList.SelectedIndex].Publisher);
                sqlCommandUPDATE.Parameters.AddWithValue("PublicationDate", Books[BooksList.SelectedIndex].PublicationDate);
                sqlCommandUPDATE.Parameters.AddWithValue("PageCount", Books[BooksList.SelectedIndex].PageCount);
                await sqlCommandUPDATE.ExecuteNonQueryAsync();
                sqlConnection.Close();

                LoadBooksToAbonement(Abonements[indexToReloadA].Id);
                LoadBooks();
                SearchBUpdate();
            }
        }

        private async void RefundBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (EditAbonementPanel.Visibility == Visibility.Visible && AbonementBooksList.SelectedIndex >= 0)
            {
                DateTime dateTime = DateTime.Now;
                MessageBox.Show($"Зарегистрирован акт возврата книги:\n" +
                    $"'{BooksOnAbonement[AbonementBooksList.SelectedIndex].BookName}'\n" +
                    $"От абонемента:\n" +
                    $"{Abonements[indexToReloadA].Surname} {Abonements[indexToReloadA].Name} {Abonements[indexToReloadA].Patronymic}\n" +
                    $"Дата и время возврата: {dateTime}", "Возврат книги", MessageBoxButton.OK, MessageBoxImage.Question);

                await sqlConnection.OpenAsync();
                SqlDataReader dataReader = null;
                SqlCommand sqlCommandSELECT = new SqlCommand($"SELECT [Id] From [Books] WHERE [BookName]=N'{BooksOnAbonement[AbonementBooksList.SelectedIndex].BookName}' AND [Publisher]=N'{BooksOnAbonement[AbonementBooksList.SelectedIndex].Publisher}' AND [PublicationDate]=N'{BooksOnAbonement[AbonementBooksList.SelectedIndex].PublicationDate}' AND [PageCount]=N'{BooksOnAbonement[AbonementBooksList.SelectedIndex].PageCount}'", sqlConnection);
                dataReader = sqlCommandSELECT.ExecuteReader();
                await dataReader.ReadAsync();
                int Book_Id = Convert.ToInt32(dataReader["Id"]);
                dataReader.Close();

                SqlCommand sqlCommandINSERT = new SqlCommand("INSERT INTO [Operations] (Date, Abonement_Id, Book_Id, Librarian_Id, Operation) VALUES (@Date, @Abonement_Id, @Book_Id, @Librarian_Id, @Operation)", sqlConnection);
                sqlCommandINSERT.Parameters.AddWithValue("Date", dateTime);
                sqlCommandINSERT.Parameters.AddWithValue("Abonement_Id", Abonements[indexToReloadA].Id);
                sqlCommandINSERT.Parameters.AddWithValue("Book_Id", Book_Id);
                sqlCommandINSERT.Parameters.AddWithValue("Librarian_Id", Librarian_Id);
                sqlCommandINSERT.Parameters.AddWithValue("Operation", "Акт возврата");
                await sqlCommandINSERT.ExecuteNonQueryAsync();

                SqlCommand sqlCommandUPDATE = new SqlCommand($"UPDATE [Books] SET [Location]=@Location WHERE [BookName]=@BookName AND [Publisher]=@Publisher AND [PublicationDate]=@PublicationDate AND [PageCount]=@PageCount", sqlConnection);
                sqlCommandUPDATE.Parameters.AddWithValue("Location", "Склад");
                sqlCommandUPDATE.Parameters.AddWithValue("BookName", BooksOnAbonement[AbonementBooksList.SelectedIndex].BookName);
                sqlCommandUPDATE.Parameters.AddWithValue("Publisher", BooksOnAbonement[AbonementBooksList.SelectedIndex].Publisher);
                sqlCommandUPDATE.Parameters.AddWithValue("PublicationDate", BooksOnAbonement[AbonementBooksList.SelectedIndex].PublicationDate);
                sqlCommandUPDATE.Parameters.AddWithValue("PageCount", BooksOnAbonement[AbonementBooksList.SelectedIndex].PageCount);
                sqlCommandUPDATE.ExecuteNonQuery();
                sqlConnection.Close();

                LoadBooksToAbonement(Abonements[indexToReloadA].Id);
                LoadBooks();
                SearchBUpdate();
            }
        }

        private void SearchBUpdate()
        {
            if (SearchB.Text.Length > 0)
            {
                if (SearchB.Text.Length < searchLengthOldB)
                {
                    LoadBooks();
                }
                searchLengthOldB = SearchB.Text.Length;
                SearchBWatermark.Visibility = Visibility.Hidden;
                string search = SearchB.Text;
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
                SearchBWatermark.Visibility = Visibility.Visible;
            }
        }

        private void ReportBookButton_Click(object sender, RoutedEventArgs e)
        {

        }

        //////////////////////////////////////////////////////////////////////////
        //Операции общего назначения

        private void EditField_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (SearchB.Equals(textBox))
            {
                SearchBUpdate();
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

        private void SearchA_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
