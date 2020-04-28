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
        int searchLengthOldA = 0;
        int searchLengthOldB = 0;
        int searchLengthOldR = 0;
        int ReportAbonementIndex = 0;
        int ReportBooksIndex = 0;
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
            ViewAbonementPanel.Visibility = Visibility.Collapsed;
            ReportPanel.Visibility = Visibility.Collapsed;
            EditAbonementPanel.Visibility = Visibility.Visible;
            indexToEditA = -1;
        }

        private void EditAbonementButton_Click(object sender, RoutedEventArgs e)
        {
            if (AbonementsList.SelectedIndex >=0)
            {
                ViewAbonementPanel.Visibility = Visibility.Collapsed;
                ReportPanel.Visibility = Visibility.Collapsed;
                EditAbonementPanel.Visibility = Visibility.Visible;
                indexToEditA = AbonementsList.SelectedIndex;
                AbonementSurname.Text = Abonements[indexToEditA].Surname;
                AbonementName.Text = Abonements[indexToEditA].Name;
                AbonementPatronymic.Text = Abonements[indexToEditA].Patronymic;
                PassportCountry.Text = Abonements[indexToEditA].Passport.Country;
                Address.Text = Abonements[indexToEditA].Address;
                ContactPhoneNumber.Text = Abonements[indexToEditA].ContactPhoneNumber;
            }
        }

        private async void DeleteAbonementButton_Click(object sender, RoutedEventArgs e)
        {
            indexToEditA = AbonementsList.SelectedIndex;

            await sqlConnection.OpenAsync();
            int Check_Id = 0;
            try
            {
                SqlCommand sqlCommandSELECT = new SqlCommand($"SELECT [Id] FROM [Books] WHERE [Location]=N'{Convert.ToString(Abonements[indexToEditA].Id)}'", sqlConnection);
                SqlDataReader dataReader = null;
                dataReader = sqlCommandSELECT.ExecuteReader();
                dataReader.Read();
                Check_Id = Convert.ToInt32(dataReader["Id"]);
                dataReader.Close();
            }
            catch (Exception)
            {
            }
            sqlConnection.Close();

            if (Check_Id == 0)
            {
                if (MessageBox.Show($"Удалить абонемент? '{Abonements[indexToEditA].Surname} {Abonements[indexToEditA].Name} {Abonements[indexToEditA].Patronymic}'?", "Удаление абонемента", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await sqlConnection.OpenAsync();
                    try
                    {
                        SqlCommand sqlCommandDELETE = new SqlCommand($"DELETE FROM [Abonements] WHERE [Id]=@Id", sqlConnection);
                        sqlCommandDELETE.Parameters.AddWithValue("Id", Abonements[indexToEditA].Id);
                        await sqlCommandDELETE.ExecuteNonQueryAsync();
                    }
                    catch (Exception)
                    {
                    }
                    sqlConnection.Close();
                    LoadAbonements();
                    ReportPanel.Visibility = Visibility.Collapsed;
                    ViewAbonementPanel.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                MessageBox.Show("На абонементе '{Abonements[indexToEditA].Surname} {Abonements[indexToEditA].Name} {Abonements[indexToEditA].Patronymic}' есть невозвращённые книги!" +
                    "\nСначала верните книги, перед тем как удалить абонемент", "Удаление абонемента", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private async void SaveAbonementButton_Click(object sender, RoutedEventArgs e)
        {
            await sqlConnection.OpenAsync();
            
            if (indexToEditA != -1)
            {
                try
                {
                    SqlCommand sqlCommandUPDATE = new SqlCommand($"UPDATE [Abonements] SET [Name]=@Name, [Surname]=@Surname, [Patronymic]=@Patronymic, [Address]=@Address, [ContactPhoneNumber]=@ContactPhoneNumber WHERE [Id]=@Id", sqlConnection);
                    sqlCommandUPDATE.Parameters.AddWithValue("Id", Abonements[indexToEditA].Id);
                    sqlCommandUPDATE.Parameters.AddWithValue("Name", AbonementName.Text);
                    sqlCommandUPDATE.Parameters.AddWithValue("Surname", AbonementSurname.Text);
                    sqlCommandUPDATE.Parameters.AddWithValue("Patronymic", AbonementPatronymic.Text);
                    sqlCommandUPDATE.Parameters.AddWithValue("Address", Address.Text);
                    sqlCommandUPDATE.Parameters.AddWithValue("ContactPhoneNumber", ContactPhoneNumber.Text);
                    await sqlCommandUPDATE.ExecuteNonQueryAsync();

                    sqlCommandUPDATE = new SqlCommand($"UPDATE [Passports] SET [Country]=@Country WHERE [Id]=@Id", sqlConnection);
                    sqlCommandUPDATE.Parameters.AddWithValue("Id", Abonements[indexToEditA].Passport.Id);
                    sqlCommandUPDATE.Parameters.AddWithValue("Country", PassportCountry);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                SqlCommand sqlCommandINSERT = new SqlCommand("INSERT INTO [Passports] (Country) VALUES (@Country)", sqlConnection);
                sqlCommandINSERT.Parameters.AddWithValue("Country", PassportCountry.Text);
                await sqlCommandINSERT.ExecuteNonQueryAsync();

                int Passport_Id = 0;
                try
                {
                    SqlCommand sqlCommandSELECT = new SqlCommand($"SELECT [Id] FROM [Passports] WHERE [Country]=N'{PassportCountry.Text}'", sqlConnection);
                    SqlDataReader dataReader = null;
                    dataReader = sqlCommandSELECT.ExecuteReader();
                    dataReader.Read();
                    Passport_Id = Convert.ToInt32(dataReader["Id"]);
                    dataReader.Close();
                }
                catch (Exception)
                {
                }

                sqlCommandINSERT = new SqlCommand("INSERT INTO [Abonements] (Surname, Name, Patronymic, Address, ContactPhoneNumber, Passport_Id) VALUES (@Surname, @Name, @Patronymic, @Address, @ContactPhoneNumber, @Passport_Id)", sqlConnection);
                sqlCommandINSERT.Parameters.AddWithValue("Passport_Id", Passport_Id);
                sqlCommandINSERT.Parameters.AddWithValue("Surname", AbonementSurname.Text);
                sqlCommandINSERT.Parameters.AddWithValue("Name", AbonementName.Text);
                sqlCommandINSERT.Parameters.AddWithValue("Patronymic", AbonementPatronymic.Text);
                sqlCommandINSERT.Parameters.AddWithValue("Address", Address.Text);
                sqlCommandINSERT.Parameters.AddWithValue("ContactPhoneNumber", ContactPhoneNumber.Text);
                await sqlCommandINSERT.ExecuteNonQueryAsync();
            }

            sqlConnection.Close();
            LoadAbonements();

            ViewAbonementPanel.Visibility = Visibility.Collapsed;
            ReportPanel.Visibility = Visibility.Collapsed;
            EditAbonementPanel.Visibility = Visibility.Collapsed;
            AbonementSurname.Text = "";
            AbonementName.Text = "";
            AbonementPatronymic.Text = "";
            PassportCountry.Text = "";
            Address.Text = "";
            ContactPhoneNumber.Text = "";
        }

        private void CancelAbonementButton_Click(object sender, RoutedEventArgs e)
        {
            ViewAbonementPanel.Visibility = Visibility.Collapsed;
            ReportPanel.Visibility = Visibility.Collapsed;
            EditAbonementPanel.Visibility = Visibility.Collapsed;
            AbonementSurname.Text = "";
            AbonementName.Text = "";
            AbonementPatronymic.Text = "";
            PassportCountry.Text = "";
            Address.Text = "";
            ContactPhoneNumber.Text = "";

        }

        private void AbonementsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AbonementsList.SelectedIndex >= 0 && EditAbonementPanel.Visibility == Visibility.Collapsed && ReportPanel.Visibility == Visibility.Collapsed)
            {
                indexToReloadA = AbonementsList.SelectedIndex;
                ViewAbonementPanel.Visibility = Visibility.Visible;
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

        private void SearchAUpdate()
        {
            if (SearchA.Text.Length > 0)
            {
                if (SearchA.Text.Length < searchLengthOldA)
                {
                    LoadAbonements();
                }
                searchLengthOldA = SearchA.Text.Length;
                SearchAWatermark.Visibility = Visibility.Hidden;
                string search = SearchA.Text;
                for (int i = 0; i < Abonements.Count; i++)
                {
                    string str = "";
                    str += Abonements[i].Passport.Country;
                    str += Abonements[i].Id;
                    str += Abonements[i].Name;
                    str += Abonements[i].Patronymic;
                    str += Abonements[i].Surname;
                    str += Abonements[i].ContactPhoneNumber;
                    str += Abonements[i].Address;
                    str += str.ToLower() + str.ToUpper();
                    int ind = str.IndexOf(search);
                    if (ind == -1)
                    {
                        Abonements.RemoveAt(i);
                        i--;
                    }
                }
            }
            else
            {
                LoadAbonements();
                SearchAWatermark.Visibility = Visibility.Visible;
            }
        }

        private void LoadAbonementReport()
        {
            ReportList.Items.Clear();
            sqlConnection.Open();
            SqlDataReader dataReader = null;
            SqlCommand sqlCommandSELECT = new SqlCommand($"SELECT * From [Operations], [Books], [Authors] WHERE [Operations].[Abonement_Id]=N'{Abonements[ReportAbonementIndex].Id}' AND [Operations].[Book_Id]=[Books].[Id] AND [Books].[Author_Id]=[Authors].[Id] ORDER BY [Date]", sqlConnection);
            try
            {
                dataReader = sqlCommandSELECT.ExecuteReader();
                while (dataReader.Read())
                {
                    string str = "";
                    str += (String)(Convert.ToString(dataReader["Operation"])).Trim(' ') + "\n";
                    str += (String)(Convert.ToString(dataReader["Name"])).Trim(' ') + " ";
                    str += (String)(Convert.ToString(dataReader["Patronymic"])).Trim(' ') + " ";
                    str += (String)(Convert.ToString(dataReader["Surname"])).Trim(' ') + "\n";
                    str += (String)(Convert.ToString(dataReader["BookName"])).Trim(' ') + "\n";
                    str += (String)(Convert.ToString(dataReader["Date"])).Trim(' ') + "\n";
                    ReportList.Items.Add(str);
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

            dataReader = null;
            sqlCommandSELECT = new SqlCommand($"SELECT * From [Operations], [Librarians] WHERE [Operations].[Abonement_Id]=N'{Abonements[ReportAbonementIndex].Id}' AND [Operations].[Librarian_Id]=[Librarians].[Id]", sqlConnection);
            try
            {
                dataReader = sqlCommandSELECT.ExecuteReader();
                int i = 0;
                while (dataReader.Read())
                {
                    string Name = (String)(Convert.ToString(dataReader["Name"])).Trim(' ');
                    string Patronymic = (String)(Convert.ToString(dataReader["Patronymic"])).Trim(' ');
                    string Surname = (String)(Convert.ToString(dataReader["Surname"])).Trim(' ');
                    if (Name == "")
                        Name = "Не указано";
                    if (Surname == "")
                        Surname = "Не указана";
                    if (Patronymic == "")
                        Patronymic = "Не указано";
                    string str = "Выдано библиотекарем:\n";
                    str += Surname + " ";
                    str += Name + " ";
                    str += Patronymic + "\n";
                    ReportList.Items[i] = ReportList.Items[i] + str;
                    i++;
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

        private void ReportAbonementButton_Click(object sender, RoutedEventArgs e)
        {

            if (AbonementsList.SelectedIndex >= 0)
            {
                ReportAbonementIndex = AbonementsList.SelectedIndex;
                EditAbonementPanel.Visibility = Visibility.Collapsed;
                ViewAbonementPanel.Visibility = Visibility.Collapsed;
                ReportPanel.Visibility = Visibility.Visible;
                LoadAbonementReport();
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
                            Id = Convert.ToInt32(dataReader["Id"])
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
            if (ViewAbonementPanel.Visibility == Visibility.Visible && BooksList.SelectedIndex >=0)
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
            if (ViewAbonementPanel.Visibility == Visibility.Visible && AbonementBooksList.SelectedIndex >= 0)
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

        private void LoadBookReport()
        {
            ReportList.Items.Clear();
            sqlConnection.Open();
            SqlDataReader dataReader = null;
            SqlCommand sqlCommandSELECT = new SqlCommand($"SELECT * From [Operations], [Abonements] WHERE [Operations].[Book_Id]='{Books[ReportBooksIndex].Id}' AND [Operations].[Abonement_Id]=[Abonements].[Id] ORDER BY [Date]", sqlConnection);
            try
            {
                dataReader = sqlCommandSELECT.ExecuteReader();
                while (dataReader.Read())
                {
                    string str = "";
                    str += (String)(Convert.ToString(dataReader["Operation"])).Trim(' ') + "\n";
                    str += (String)(Convert.ToString(dataReader["Name"])).Trim(' ') + " ";
                    str += (String)(Convert.ToString(dataReader["Patronymic"])).Trim(' ') + " ";
                    str += (String)(Convert.ToString(dataReader["Surname"])).Trim(' ') + "\n";
                    str += (String)(Convert.ToString(dataReader["Date"])).Trim(' ') + "\n";
                    ReportList.Items.Add(str);
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

            dataReader = null;
            sqlCommandSELECT = new SqlCommand($"SELECT * From [Operations], [Librarians] WHERE [Operations].[Book_Id]=N'{Books[ReportBooksIndex].Id}' AND [Operations].[Librarian_Id]=[Librarians].[Id]", sqlConnection);
            try
            {
                dataReader = sqlCommandSELECT.ExecuteReader();
                int i = 0;
                while (dataReader.Read())
                {
                    string Name = (String)(Convert.ToString(dataReader["Name"])).Trim(' ');
                    string Patronymic = (String)(Convert.ToString(dataReader["Patronymic"])).Trim(' ');
                    string Surname = (String)(Convert.ToString(dataReader["Surname"])).Trim(' ');
                    if (Name == "")
                        Name = "Не указано";
                    if (Surname == "")
                        Surname = "Не указана";
                    if (Patronymic == "")
                        Patronymic = "Не указано";
                    string str = "Выдано библиотекарем:\n";
                    str += Surname + " ";
                    str += Name + " ";
                    str += Patronymic + "\n";
                    ReportList.Items[i] = ReportList.Items[i] + str;
                    i++;
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

        private void ReportBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksList.SelectedIndex >= 0)
            {
                ReportBooksIndex = BooksList.SelectedIndex;
                EditAbonementPanel.Visibility = Visibility.Collapsed;
                ViewAbonementPanel.Visibility = Visibility.Collapsed;
                ReportPanel.Visibility = Visibility.Visible;
                LoadBookReport();
            }
        }

        //////////////////////////////////////////////////////////////////////////
        //Операции общего назначения

        private void SearchRUpdate()
        {
            if (SearchR.Text.Length > 0)
            {
                if (SearchR.Text.Length < searchLengthOldR)
                {
                    LoadAbonementReport();
                }
                searchLengthOldR = SearchR.Text.Length;
                SearchRWatermark.Visibility = Visibility.Hidden;
                string search = SearchR.Text;
                for (int i = 0; i < ReportList.Items.Count; i++)
                {
                    string str = "";
                    str += ReportList.Items[i];
                    str += str.ToLower() + str.ToUpper();
                    int ind = str.IndexOf(search);
                    if (ind == -1)
                    {
                        ReportList.Items.RemoveAt(i);
                        i--;
                    }
                }
            }
            else
            {
                LoadAbonementReport();
                SearchRWatermark.Visibility = Visibility.Visible;
            }
        }

        private void EditField_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (SearchA.Equals(textBox))
            {
                SearchAUpdate();
            }
            else if (SearchB.Equals(textBox))
            {
                SearchBUpdate();
            }
            else if (SearchR.Equals(textBox))
            {
                SearchRUpdate();
            }
            else if (AbonementSurname.Equals(textBox))
            {
                if (AbonementSurname.Text.Length > 0)
                {
                    AbonementSurnameWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    AbonementSurnameWatermark.Visibility = Visibility.Visible;
                }
            }
            else if (AbonementName.Equals(textBox))
            {
                if (AbonementName.Text.Length > 0)
                {
                    AbonementNameWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    AbonementNameWatermark.Visibility = Visibility.Visible;
                }
            }
            else if (AbonementPatronymic.Equals(textBox))
            {
                if (AbonementPatronymic.Text.Length > 0)
                {
                    AbonementPatronymicWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    AbonementPatronymicWatermark.Visibility = Visibility.Visible;
                }
            }
            else if (PassportCountry.Equals(textBox))
            {
                if (PassportCountry.Text.Length > 0)
                {
                    PassportCountryWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    PassportCountryWatermark.Visibility = Visibility.Visible;
                }
            }
            else if (Address.Equals(textBox))
            {
                if (Address.Text.Length > 0)
                {
                    AddressWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    AddressWatermark.Visibility = Visibility.Visible;
                }
            }
            else if (ContactPhoneNumber.Equals(textBox))
            {
                if (ContactPhoneNumber.Text.Length > 0)
                {
                    ContactPhoneNumberWatermark.Visibility = Visibility.Hidden;
                }
                else
                {
                    ContactPhoneNumberWatermark.Visibility = Visibility.Visible;
                }
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
