using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LlibrarySystem
{
    class Author
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public int Id { get; set; }
        public static Author Copy(Author author)
        {
            return new Author
            {
                Surname = author.Surname,
                Name = author.Name,
                Patronymic = author.Patronymic,
                Id = author.Id
            };
        }
    }
}
