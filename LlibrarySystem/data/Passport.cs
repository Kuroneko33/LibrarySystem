using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LlibrarySystem
{
    class Passport
    {
        public string Country { get; set; }
        public int Id { get; set; }
        public static Passport Copy(Passport passport)
        {
            return new Passport
            {
                Country = passport.Country,
                Id = passport.Id
            };
        }
    }
}
