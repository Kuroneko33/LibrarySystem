using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LlibrarySystem
{
    class Librarian
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Password { get; set; }
        public int Id { get; set; }
        public static Librarian Copy(Librarian librarian)
        {
            return new Librarian 
            { 
                Surname = librarian.Surname, 
                Name = librarian.Name, 
                Patronymic = librarian.Patronymic, 
                Password = librarian.Password,
                Id = librarian.Id
            };
        }
    }
}
