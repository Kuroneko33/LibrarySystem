using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LlibrarySystem
{
    class Abonement
    {
        public Passport Passport { get; set; } = new Passport();
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string Address { get; set; }
        public string ContactPhoneNumber { get; set; }
        public int  Id { get; set; }
        public static Abonement Copy(Abonement abonement)
        {
            return new Abonement
            {
                Passport = new Passport() { Country = abonement.Passport.Country },
                Surname = abonement.Surname,
                Name = abonement.Name,
                Patronymic = abonement.Patronymic,
                Address = abonement.Address,
                ContactPhoneNumber = abonement.ContactPhoneNumber,
                Id = abonement.Id
            };
        }
    }
}
