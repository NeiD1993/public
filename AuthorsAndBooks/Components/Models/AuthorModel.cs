using AuthorsAndBooks.Components.ResourceModels;
using System;
using System.Collections.Generic;

namespace AuthorsAndBooks.Models
{
    public class AuthorModel
    {
        public AuthorModel() { }

        public AuthorModel(AuthorResourceModel author)
        {
            Name = author.Name;
            Surname = author.Surname;
            Patronymic = author.Patronymic;
            Birthdate = author.Birthdate;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }

        public string Fullname { get; }

        public DateTime Birthdate { get; set; }

        public IEnumerable<BookModel> Books { get; set; }
    }
}