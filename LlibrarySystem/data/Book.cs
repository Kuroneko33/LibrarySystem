using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LlibrarySystem
{
    class Book
    {
        public Author Author { get; set; } = new Author();
        public string BookName { get; set; }
        public string Publisher { get; set; }
        public int PublicationDate { get; set; }
        public int PageCount { get; set; }
        public string Location { get; set; }
        public int Id { get; set; }
        public static Book Copy(Book book)
        {
            return new Book 
            {
                Author = new Author() { Surname = book.Author.Surname, Name = book.Author.Name, Patronymic = book.Author.Patronymic},
                BookName = book.BookName,
                Publisher = book.Publisher,
                PublicationDate = book.PublicationDate,
                PageCount = book.PageCount,
                Location = book.Location,
                Id = book.Id
            };
        }
    }
}
